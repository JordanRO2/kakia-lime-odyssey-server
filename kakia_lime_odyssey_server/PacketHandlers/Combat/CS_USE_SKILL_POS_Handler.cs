/// <summary>
/// Handles CS_USE_SKILL_POS packet - player uses a skill at a specific position (ground target).
/// </summary>
/// <remarks>
/// Triggered by: Player using an AoE or ground-targeted skill
/// Response packets: SC_START_CASTING_SKILL_POS (if cast time > 0), SC_USE_SKILL_POS_RESULT_LIST,
///                   SC_WEAPON_HIT_RESULT (per target), SC_STOP/SC_DEAD (for killed targets)
/// Database: None directly
/// Note: Used for AoE skills like fireballs, ground effects, etc.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Combat;
using kakia_lime_odyssey_server.Entities.Monsters;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_USE_SKILL_POS)]
class CS_USE_SKILL_POS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var useSkill = PacketConverter.Extract<CS_USE_SKILL_POS>(p.Payload);

		var skill = LimeServer.SkillDB.FirstOrDefault(skill => skill.Id == useSkill.typeID);
		if (skill is null)
		{
			Logger.Log($"Skill not found with ID: {useSkill.typeID}!", LogLevel.Error);
			return;
		}

		// SERVER-SIDE COOLDOWN VALIDATION
		var playerClient = client as PlayerClient;
		var cooldownTracker = playerClient?.GetSkillCooldownTracker();
		if (cooldownTracker != null)
		{
			if (!cooldownTracker.ValidateAndTrackSkillUse((int)useSkill.typeID, skill))
			{
				Logger.Log(
					$"[COOLDOWN REJECT] Player {playerClient?.GetCurrentCharacter()?.appearance.name} " +
					$"attempted to use position skill {useSkill.typeID} while on cooldown",
					LogLevel.Warning
				);
				return;
			}
		}

		string playerName = playerClient?.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[COMBAT] {playerName} using skill {skill.Name} at position ({useSkill.pos.x}, {useSkill.pos.y}, {useSkill.pos.z})", LogLevel.Debug);

		// Get skill level (use level 1 for now)
		var skillLv1 = skill.Subject.SubjectLists.FirstOrDefault();
		uint castTime = (uint)(skillLv1 != null ? skillLv1.CastingTime : skill.CastingTime);

		// Send cast start if there's a cast time
		if (castTime > 0)
		{
			SC_START_CASTING_SKILL_POS castSkill = new()
			{
				fromInstID = client.GetObjInstID(),
				pos = useSkill.pos,
				typeID = useSkill.typeID,
				castTime = castTime
			};

			using (PacketWriter pw = new())
			{
				pw.Write(castSkill);
				client.Send(pw.ToPacket(), default).Wait();
				client.SendGlobalPacket(pw.ToPacket(), default).Wait();
			}
		}

		// Send skill result
		SC_USE_SKILL_POS_RESULT_LIST actionSkill = new()
		{
			fromInstID = client.GetObjInstID(),
			toPos = useSkill.pos,
			typeID = useSkill.typeID,
			useHP = (ushort)(skillLv1 != null ? skillLv1.UseHP : 0),
			useMP = (ushort)(skillLv1 != null ? skillLv1.UseMP : 0),
			useLP = (ushort)(skillLv1 != null ? skillLv1.UseLP : 0),
			useSP = (ushort)(skillLv1 != null ? skillLv1.UseSP : 0),
			coolTime = (uint)(skillLv1 != null ? skillLv1.CoolTime : skill.CoolTime)
		};

		using (PacketWriter pw = new())
		{
			pw.Write(actionSkill);
			client.Send(pw.ToSizedPacket(), default).Wait();
			client.SendGlobalPacket(pw.ToSizedPacket(), default).Wait();
		}

		// Get AOE radius from skill (use skill-level range if available, otherwise base range)
		float aoeRadius = (float)(skillLv1?.Range ?? skill.Range);
		if (aoeRadius <= 0) aoeRadius = 5.0f; // Default AOE radius if not specified

		// Find all entities within AOE radius
		var targets = GetEntitiesInRange(client.GetZone(), useSkill.pos, aoeRadius, client.GetObjInstID());

		Logger.Log($"[AOE] {playerName} skill {skill.Name} - found {targets.Count} targets in {aoeRadius} radius", LogLevel.Debug);

		// Apply damage to each target
		foreach (var target in targets)
		{
			ApplyAoEDamageToTarget(client, target, playerClient);
		}

		Logger.Log($"[COMBAT] {playerName} skill {skill.Name} executed at position, hit {targets.Count} targets", LogLevel.Debug);
	}

	/// <summary>
	/// Gets all entities within range of a position, excluding the caster.
	/// </summary>
	/// <param name="zoneId">Zone to search in</param>
	/// <param name="position">Center position for AOE</param>
	/// <param name="radius">AOE radius</param>
	/// <param name="excludeId">Entity ID to exclude (caster)</param>
	/// <returns>List of entities in range</returns>
	private static List<IEntity> GetEntitiesInRange(uint zoneId, FPOS position, float radius, long excludeId)
	{
		var entities = new List<IEntity>();
		float radiusSq = radius * radius;

		// Check monsters in the zone
		if (LimeServer.Mobs.TryGetValue(zoneId, out var mobs))
		{
			foreach (var mob in mobs)
			{
				if (mob.GetId() == excludeId) continue;
				if (mob.GetEntityStatus().BasicStatus.Hp <= 0) continue;

				if (DistanceSquared(position, mob.Position) <= radiusSq)
					entities.Add(mob);
			}
		}

		// Check players in the zone (for PvP scenarios)
		foreach (var player in LimeServer.PlayerClients)
		{
			if (player.GetObjInstID() == excludeId) continue;
			if (player.GetZone() != zoneId) continue;
			if (player.IsDead()) continue;

			if (DistanceSquared(position, player.GetPosition()) <= radiusSq)
				entities.Add(player);
		}

		return entities;
	}

	/// <summary>
	/// Calculates squared distance between two positions.
	/// </summary>
	private static float DistanceSquared(FPOS a, FPOS b)
	{
		float dx = a.x - b.x;
		float dy = a.y - b.y;
		float dz = a.z - b.z;
		return dx * dx + dy * dy + dz * dz;
	}

	/// <summary>
	/// Applies AOE damage to a single target and handles death/rewards.
	/// </summary>
	private static void ApplyAoEDamageToTarget(IPlayerClient client, IEntity target, PlayerClient? killerPlayer)
	{
		// Calculate damage
		var damage = kakia_lime_odyssey_server.Combat.DamageHandler.DealWeaponHitDamage((client as IEntity)!, target);
		if (damage.Packet is null) return;

		// Send damage packet
		client.Send(damage.Packet, default).Wait();
		client.SendGlobalPacket(damage.Packet, default).Wait();

		// Apply damage to target
		DamageResult result;
		if (target is PlayerClient playerTarget)
		{
			result = playerTarget.TakeDamage((int)damage.Damage);
		}
		else
		{
			result = target.UpdateHealth((int)(damage.Damage * -1));

			if (result.TargetKilled)
			{
				SendDeathPackets(target, client);

				// Update quest hunt objectives for monster kills
				if (killerPlayer != null)
				{
					int monsterTypeId = target.GetEntityTypeId();
					LimeServer.QuestService.OnMonsterKilled(killerPlayer, monsterTypeId);
				}
			}
		}

		// Handle experience reward for killed monsters
		if (result.TargetKilled && result.ExpReward > 0 && client is IEntity pcEntity)
		{
			HandleExpReward(pcEntity, result.ExpReward, client);
		}
	}

	/// <summary>
	/// Sends death-related packets for a killed target.
	/// </summary>
	private static void SendDeathPackets(IEntity target, IPlayerClient client)
	{
		SC_STOP sc_stop = new()
		{
			objInstID = target.GetId(),
			pos = target.GetPosition(),
			dir = target.GetDirection(),
			tick = LimeServer.GetCurrentTick(),
			stopType = 0
		};

		using PacketWriter pw_stop = new();
		pw_stop.Write(sc_stop);
		client.Send(pw_stop.ToPacket(), default).Wait();
		client.SendGlobalPacket(pw_stop.ToPacket(), default).Wait();

		using PacketWriter pw_dead = new();
		SC_DEAD dead = new()
		{
			objInstID = target.GetId()
		};

		pw_dead.Write(dead);
		client.Send(pw_dead.ToPacket(), default).Wait();
		client.SendGlobalPacket(pw_dead.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Handles experience reward for killing a target.
	/// </summary>
	private static void HandleExpReward(IEntity pcEntity, int expReward, IPlayerClient client)
	{
		var levelUp = pcEntity.AddExp((ulong)expReward);
		var currentStatus = pcEntity.GetEntityStatus();

		using (PacketWriter pw = new())
		{
			SC_GOT_COMBAT_JOB_EXP addExp = new()
			{
				exp = (uint)currentStatus.Exp,
				addExp = (uint)expReward
			};
			pw.Write(addExp);
			client.Send(pw.ToPacket(), default).Wait();
		}

		if (levelUp)
		{
			using PacketWriter pw = new();
			SC_PC_COMBAT_JOB_LEVEL_UP lvUp = new()
			{
				objInstID = pcEntity.GetId(),
				lv = currentStatus.Lv,
				exp = (uint)currentStatus.Exp,
				newStr = 5,
				newInl = 5,
				newAgi = 5,
				newDex = 5,
				newSpi = 5,
				newVit = 5
			};
			pw.Write(lvUp);
			client.Send(pw.ToPacket(), default).Wait();
			client.SendGlobalPacket(pw.ToPacket(), default).Wait();
		}
	}
}
