using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Entities.Monsters;
using kakia_lime_odyssey_server.Entities.Npcs;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.MonsterXML;
using kakia_lime_odyssey_server.Models.QuestXML;
using kakia_lime_odyssey_server.Models.SkillXML;
using kakia_lime_odyssey_server.Services.Ban;
using kakia_lime_odyssey_server.Services.Combat;
using kakia_lime_odyssey_server.Services.Guild;
using kakia_lime_odyssey_server.Services.Party;
using kakia_lime_odyssey_server.Services.Post;
using kakia_lime_odyssey_server.Services.Exchange;
using kakia_lime_odyssey_server.Services.Quest;
using kakia_lime_odyssey_server.Services.Chatroom;
using kakia_lime_odyssey_server.Services.Skill;
using kakia_lime_odyssey_server.Services.Item;
using kakia_lime_odyssey_server.Services.Trade;
using kakia_lime_odyssey_server.Services.Crafting;
using kakia_lime_odyssey_server.Services.Currency;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace kakia_lime_odyssey_server.Network;

public class LimeServer : SocketServer
{
	public static List<XmlMonster> MonsterDB = new List<XmlMonster>();
	public static List<Item> ItemDB = ItemInfo.GetItems();
	public static List<XmlSkill> SkillDB = SkillInfo.GetSkills();
	public static List<XmlQuest> QuestDB = new List<XmlQuest>();

	public static List<PlayerClient> PlayerClients = new();
	public static Dictionary<uint, List<Npc>> Npcs = new();
	public static Dictionary<uint, List<Monster>> Mobs = new();
	public static DateTime StartTime = DateTime.Now;

	/// <summary>Service for managing buffs/debuffs on entities</summary>
	public static BuffService BuffService { get; } = new();

	/// <summary>Service for managing parties</summary>
	public static PartyService PartyService { get; } = new();

	/// <summary>Service for managing guilds</summary>
	public static GuildService GuildService { get; } = new();

	/// <summary>Service for managing mail/post messages</summary>
	public static PostService PostService { get; } = new();

	/// <summary>Service for managing player-to-player exchanges</summary>
	public static ExchangeService ExchangeService { get; } = new();

	/// <summary>Service for managing player quests</summary>
	public static QuestService QuestService { get; } = new();

	/// <summary>Service for managing private chatrooms</summary>
	public static ChatroomService ChatroomService { get; } = new();

	/// <summary>Service for managing skill learning and progression</summary>
	public static SkillService SkillService { get; } = new();

	/// <summary>Service for managing item usage and consumption</summary>
	public static ItemService ItemService { get; } = new();

	/// <summary>Service for managing NPC merchant transactions</summary>
	public static TradeService TradeService { get; } = new();

	/// <summary>Service for managing item crafting and material processing</summary>
	public static CraftingService CraftingService { get; } = new();

	/// <summary>Service for managing player currency (Peder and Lant)</summary>
	public static CurrencyService CurrencyService { get; } = new();

	public Config Config { get; set; }

	public BackgroundTask BackgroundTask { get; set; }

	private static uint _currentObjInstID = 200000;
	private uint _lastTickTime;
	private uint _regenTickAccumulator;
	private const uint RegenTickIntervalMs = 5000; // Natural regen every 5 seconds

	public LimeServer(Config cfg) : base(cfg.ServerIP, cfg.Port)
	{
		Config = cfg;
		var villagers = WorldDataLoader.LoadNpcSpawns();
		foreach(var v in villagers)
		{
			if(!Npcs.ContainsKey(v.ZoneId))
				Npcs.Add(v.ZoneId, new List<Npc>());
			Npcs[v.ZoneId].Add(v);
		}
		Logger.Log($"NPCs loaded: {villagers.Count}", LogLevel.Information);

		MonsterDB.AddRange(MonsterInfo.GetEntries());
		Logger.Log("Monster DB loaded.", LogLevel.Information);

		QuestDB.AddRange(QuestInfo.GetQuests());
		Logger.Log($"Quest DB loaded: {QuestDB.Count} quests.", LogLevel.Information);

		var mapMobs = WorldDataLoader.LoadMobSpawns();
		foreach (var mob in  mapMobs)
		{
			var monster = MonsterDB.FirstOrDefault(mDb => mDb.ModelTypeID == mob.ModelTypeId);
			if (monster == null)
				continue;
			Monster newMob = new Monster(monster, GenerateUniqueObjectId(), mob.Pos, new FPOS() { x = 0.9f, y = 0, z = 0}, (uint)mob.ZoneId, false, mob.LootTableId);
			AddNPC(newMob);
		}

		BackgroundTask = new BackgroundTask(TimeSpan.FromMilliseconds(1000/60));
		BackgroundTask.Run += ServerTick;
		BackgroundTask.Start();

		// Subscribe to ban service kick events
		BanService.OnPlayerBanned += HandlePlayerBan;
	}

	/// <summary>
	/// Handle player ban/kick requests from the anti-cheat system
	/// </summary>
	private void HandlePlayerBan(uint playerId, string reason)
	{
		var player = PlayerClients.FirstOrDefault(p => p.GetObjInstID() == playerId);
		if (player != null)
		{
			Logger.Log($"[BAN] Kicking player {player.GetCurrentCharacter()?.appearance.name ?? "Unknown"} ({playerId}): {reason}", LogLevel.Warning);
			player.Disconnect();
		}
	}

	public ReadOnlySpan<PlayerClient> GetReadonlyPlayers()
	{
		return CollectionsMarshal.AsSpan<PlayerClient>(PlayerClients);
	}

	public static bool TryGetEntity(long id, out IEntity? entity)
	{
		entity = null;

		var pc = PlayerClients.FirstOrDefault(pc => pc.GetObjInstID() == id);
		if (pc is not null)
		{
			entity = pc;
			return true;
		}
		var mob = Mobs.SelectMany(pair => pair.Value)
			.FirstOrDefault(mob => mob.Id == id);

		if (mob is not null)
		{
			entity = mob;
			return true;
		}
		return false;
	}

	public void Stop()
	{
		try
		{
			BackgroundTask.StopAsync().Wait();
		} catch { }
	}

	public static uint GetCurrentTick()
	{
		return (uint)DateTime.Now.Subtract(StartTime).TotalMilliseconds;
	}

	private async Task ServerTick()
	{
		uint currentTick = GetCurrentTick();
		uint deltaMs = currentTick - _lastTickTime;
		_lastTickTime = currentTick;

		// Update buff timers and handle expirations
		var expiredBuffs = BuffService.UpdateBuffTimers((int)deltaMs);
		await SendBuffExpirationPackets(expiredBuffs);

		// Natural HP/MP regeneration tick (every 5 seconds)
		_regenTickAccumulator += deltaMs;
		if (_regenTickAccumulator >= RegenTickIntervalMs)
		{
			_regenTickAccumulator -= RegenTickIntervalMs;
			await ProcessNaturalRegeneration();
		}

		await UnloadLoggedOutPCs();
		DetectPlayersEnteringSight();

		// MONSTER STUFF
		foreach (var kv in Mobs)
		{
			foreach(var mob in kv.Value)
			{
				mob.Update(currentTick, GetReadonlyPlayers());
			}
		}
	}

	private async Task UnloadLoggedOutPCs()
	{
		List<PlayerClient> remove = new();

		foreach (var pc in PlayerClients)
		{
			if (!pc.IsConnected())
			{
				remove.Add(pc);
				continue;
			}

			await pc.Update(GetCurrentTick());
		}

		foreach (var pc in remove)
		{
			PlayerClients.Remove(pc);

			pc.SendGlobal -= SendGlobal;
			pc.RequestZonePresence -= LoadOthersInZone;
			pc.RequestStatus -= GetStatusFor;
			pc.AddNpc -= AddNPC;
			pc.Save();

			SC_LEAVE_SIGHT_PC leave_pc = new()
			{
				leave_zone = new SC_LEAVE_ZONEOBJ { objInstID = pc.GetObjInstID() }
			};

			using PacketWriter pw = new();
			pw.Write(leave_pc);
			await SendGlobal(pc, pw.ToPacket(), default);
		}
	}

	private void DetectPlayersEnteringSight()
	{
		var players = GetReadonlyPlayers();
		foreach (var requester in players)
		{
			if (!requester.IsLoaded())
				continue;

			foreach (var pc in players)
			{
				if (!pc.IsLoaded() || pc == requester || pc.GetZone() != requester.GetZone() || requester.KnowOf(pc.GetObjInstID())) continue;

				requester.Seen(pc.GetObjInstID());

				var loadPC = pc.GetEnterSight();
				using PacketWriter pw = new();
				pw.Write(loadPC);
				requester.Send(pw.ToSizedPacket(), default).Wait();
			}
		}
	}

	public override void OnConnect(SocketClient s)
	{
		var pc = new PlayerClient(s);
		pc.SendGlobal += SendGlobal;
		pc.RequestZonePresence += LoadOthersInZone;
		pc.RequestStatus += GetStatusFor;
		pc.AddNpc += AddNPC;
		PlayerClients.Add(pc);
	}

	public bool AddNPC(INpc npc)
	{
		uint zone = 0;

		switch(npc.GetNpcType())
		{
			case NpcType.Mob:
				if (npc is Monster mob)
				{
					zone = mob.Zone;
					if (!Mobs.ContainsKey(zone)) Mobs.Add(zone, new List<Monster>());
					Mobs[zone].Add(mob);
				}
				break;

			case NpcType.Npc:
				if (npc is Npc np)
				{
					zone = np.ZoneId;
					if (!Npcs.ContainsKey(zone)) Npcs.Add(zone, new List<Npc>());
					Npcs[zone].Add(np);
				}
				break;
		}

		foreach(PlayerClient pc in GetReadonlyPlayers())
		{
			if (!pc.IsLoaded() || pc.GetZone() != zone)
				continue;

			using PacketWriter pw = new();
			switch (npc.GetNpcType())
			{
				case NpcType.Mob:
					if (npc is Monster monster)
					{
						if (monster.Despawned)
							continue;
						pw.Write(monster.GetEnterSight());
					}
					break;

				case NpcType.Npc:
					if (npc is Npc npcEntity)
						pw.Write(npcEntity.GetEnterSight());
					break;
			}
			pc.Send(pw.ToSizedPacket(), default).Wait();
		}

		return true;
	}

	public COMMON_STATUS GetStatusFor(long objInstID)
	{
		var player = PlayerClients.FirstOrDefault(pc => pc.GetObjInstID() == (uint)objInstID);
		if (player is not null)
			return player.GetStatus();

		foreach(var kv in Npcs)
		{
			var npc = kv.Value.FirstOrDefault(n => n.Id == (uint)objInstID);
			if (npc is not null)
				return npc.Status;
		}

		foreach (var kv in Mobs)
		{
			var mob = kv.Value.FirstOrDefault(m => m.Id == (uint)objInstID);
			if (mob is not null)
				return mob.GetMob().Status;
		}

		return new COMMON_STATUS();
	}

	public async Task LoadOthersInZone(PlayerClient requester, CancellationToken token)
	{
		if (Npcs.ContainsKey(requester.GetZone()))
		{
			foreach(var npc in Npcs[requester.GetZone()].Where(n => !requester.KnowOf((uint)n.Id)))
			{
				var loadNPC = npc.GetEnterSight();
				using PacketWriter pw = new();
				pw.Write(loadNPC);
				await requester.Send(pw.ToSizedPacket(), token);
			}
		}

		if (Mobs.ContainsKey(requester.GetZone()))
		{
			foreach (var mob in Mobs[requester.GetZone()].Where(n => !requester.KnowOf((uint)n.Id)))
			{
				var loadMob = mob.GetEnterSight();
				using PacketWriter pw = new();
				pw.Write(loadMob);
				await requester.Send(pw.ToSizedPacket(), token);
			}
		}
	}

	public async Task SendGlobal(PlayerClient sender, byte[] packet, CancellationToken token)
	{
		foreach(var pc in GetReadonlyPlayers())
		{
			if (pc == sender || pc.GetZone() != sender.GetZone()) continue;
			pc.Send(packet, token).Wait();
		}
	}

	public static uint GenerateUniqueObjectId()
	{
		return Interlocked.Increment(ref _currentObjInstID);
	}

	private async Task SendBuffExpirationPackets(List<(IEntity Entity, ActiveBuff Buff)> expiredBuffs)
	{
		foreach (var (entity, buff) in expiredBuffs)
		{
			var removePacket = BuffService.BuildRemoveDefPacket(buff.InstId);

			if (entity is PlayerClient pc && pc.IsConnected())
			{
				await pc.Send(removePacket, default);
			}
		}
	}

	private async Task ProcessNaturalRegeneration()
	{
		var healingService = new HealingService();

		foreach (var pc in GetReadonlyPlayers())
		{
			if (!pc.IsLoaded() || !pc.IsConnected())
				continue;

			var status = pc.GetStatus();

			// Skip regen if dead
			if (status.hp <= 0)
				continue;

			// Skip regen if in combat (has recent combat activity)
			if (pc.IsInCombat())
				continue;

			// HP regeneration
			if (status.hp < status.mhp)
			{
				uint hpRegen = healingService.CalculateNaturalRegenHP(pc);
				if (hpRegen > 0)
				{
					uint newHP = Math.Min(status.hp + hpRegen, status.mhp);
					pc.UpdateHP((int)newHP, false);
				}
			}

			// MP regeneration
			if (status.mp < status.mmp)
			{
				uint mpRegen = healingService.CalculateNaturalRegenMP(pc);
				if (mpRegen > 0)
				{
					uint newMP = Math.Min(status.mp + mpRegen, status.mmp);
					pc.UpdateMP((int)newMP, false);
				}
			}
		}
	}
}
