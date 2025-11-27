using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Combat;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.PcStatusXML;
using System.Threading;

namespace kakia_lime_odyssey_server.Network;

public class PlayerClient : IPlayerClient, IEntity
{
	public Func<PlayerClient, byte[], CancellationToken, Task>? SendGlobal;
	public Func<PlayerClient, CancellationToken, Task>? RequestZonePresence;
	public Func<long, COMMON_STATUS>? RequestStatus;
	public Func<INpc, bool>? AddNpc;

	private SocketClient _socketClient { get; set; }
	private int _clientRevision { get; set; }
	private string _accountId { get; set; } = default!;
	private ModClientPC _currentCharacter { get; set; } = default!;
	private uint _objInstID = 0;
	private long _target = 0;
	private bool _inCombat = false;
	private DateTime _lastCombatTime = DateTime.MinValue;
	private int _jobId = 1;

	private WorldPosition _worldPosition = new();
	private VELOCITIES _velocities;
	private ModCommonStatus _status = default!;

	private PlayerInventory _inventory { get; set; } = new();
	private PlayerEquips _equipment { get; set; } = new();
	private PlayerQuestsTracker _quests { get; set; } = new();

	private bool _inMotion = false;

	// Death state tracking
	private bool _isDead = false;
	private DateTime _deathTime = DateTime.MinValue;

	// Quest turn-in tracking (set when player opens quest turn-in dialog)
	private uint _pendingTurnInQuestId = 0;

	// Anti-cheat tracking fields
	private uint _lastClientTick = 0;
	private FPOS _lastPosition = new();
	private DateTime _lastMoveTime = DateTime.Now;
	private uint _movePacketCount = 0;
	private bool _isJumping = false;
	private DateTime _jumpStartTime = DateTime.MinValue;
	private bool _isUnderwater = false;

	// Skill cooldown tracking (server-side validation)
	private SkillCooldownTracker? _skillCooldownTracker = null;

	private List<uint> _knownPc { get; set; }

	private bool _isLoaded;

	public PlayerClient(SocketClient socketClient)
	{
		_velocities = new()
		{
			ratio = 1.0F,
			walk = 80,
			walkAccel = 0,
			backSwim = 90,
			backSwimAccel = 0,
			backwalkAccel = 0,
			backwalk = 70,			
			run = 90,
			runAccel = 0,
			swim = 90,
			swimAccel = 0
		};
		_knownPc = new List<uint>();

		_accountId = string.Empty;
		_socketClient = socketClient;
		_socketClient.PacketReceived += PacketRecieved;
		Logger.Log($"Client connected [{_socketClient.GetIP()}]");
		_= _socketClient.BeginRead();
	}

	public void Save()
	{
		if (!_isLoaded) return;
		var db = DatabaseFactory.Instance;
		db.SaveWorldPosition(_accountId, _currentCharacter.appearance.name, _worldPosition);
		db.SavePlayerInventory(_accountId, _currentCharacter.appearance.name, _inventory);
		db.SavePlayerEquipment(_accountId, _currentCharacter.appearance.name, _equipment);
		db.SavePlayerQuests(_accountId, _currentCharacter.appearance.name, _quests.GetPersistenceData());
	}

	/// <summary>
	/// Resets the player state when returning to lobby (character select).
	/// </summary>
	public void ResetToLobby()
	{
		_isLoaded = false;
		_target = 0;
		_inCombat = false;
		_inMotion = false;
		_isJumping = false;
		_isUnderwater = false;
		_knownPc.Clear();
	}

	public void Load()
	{
		var db = DatabaseFactory.Instance;
		_worldPosition = db.GetWorldPosition(_accountId, _currentCharacter.appearance.name);
		_inventory = db.GetPlayerInventory(_accountId, _currentCharacter.appearance.name);
		_equipment = db.GetPlayerEquipment(_accountId, _currentCharacter.appearance.name);
		_quests = new PlayerQuestsTracker(db.GetPlayerQuests(_accountId, _currentCharacter.appearance.name));
		_currentCharacter.appearance.equiped = new ModEquipped(_equipment.Combat.GetEquipped());
	}

	public int GetClientRevision() => _clientRevision;

	public void SetClientRevision(int rev)
	{
		_clientRevision = rev;
	}

	public bool IsConnected()
	{
		return _socketClient.IsAlive;
	}

	public bool IsLoaded()
	{
		return _isLoaded;
	}

	public void SetUnloaded()
	{
		_isLoaded = false;
	}

	public void SetLoaded()
	{
		_isLoaded = true;
	}

	public async Task PacketRecieved(RawPacket packet)
	{
		PacketHandler handler = kakia_lime_odyssey_network.Handler.PacketHandlers.GetHandlerFor(packet.PacketId);
		//Logger.Log($"Recieved [{packet.PacketId}]", LogLevel.Debug);
		if (handler != null)
		{
			try
			{
				handler.HandlePacket(this, packet);
				return;
			}
			catch (Exception e)
			{
				Logger.Log(e);
			}
		}
		else
		{
			Logger.Log($"NOT IMPLEMENTED [{packet.PacketId}]", LogLevel.Warning);
			//Logger.LogPck(packet.Payload);
		}
	}

	public async Task<bool> Send(byte[] packet, CancellationToken token)
	{
		//Logger.Log($"Sending [{((PacketType)BitConverter.ToUInt16(packet, 0))}]", LogLevel.Debug);
		await _socketClient!.Send(packet);
		return true;
	}

	public async Task<bool> SendGlobalPacket(byte[] packet, CancellationToken token)
	{
		await SendGlobal!.Invoke(this, packet, token);
		return true;
	}

	public async Task<bool> Send(PacketType header, byte[] packet, CancellationToken token)
	{
		byte[] bytes = new byte[packet.Length + 4];
		Buffer.BlockCopy(packet, 0, bytes, 4, packet.Length);
		Buffer.BlockCopy(BitConverter.GetBytes((ushort)header), 0, bytes, 0, 2);
		await _socketClient!.Send(bytes);
		return true;
	}

	public void SetAccountId(string accountId)
	{
		_accountId = accountId;
	}

	public string GetAccountId()
	{
		return _accountId;
	}

	public uint GetObjInstID()
	{
		if (_objInstID == 0)
			Logger.Log("Requesting obj id before its set!", LogLevel.Error);

		return _objInstID;
	}

	public void SetCurrentCharacter(CLIENT_PC pc)
	{
		_currentCharacter = new ModClientPC(pc);
		//var hash = MD5.HashData(Encoding.UTF8.GetBytes(pc.appearance.name + Guid.NewGuid()));
		//_objInstID = BitConverter.ToUInt32(hash);

		Random rnd = new Random();
		_objInstID = (uint)(20000 + rnd.Next(0, 9999));

		_status = new()
		{
			lv = pc.status.combatJob.lv,
			mhp = pc.status.hp,
			hp = pc.status.hp,
			mmp = pc.status.mp,
			mp = pc.status.mp
		};

		// Initialize skill cooldown tracker
		_skillCooldownTracker = new SkillCooldownTracker(_objInstID, global::System.Text.Encoding.ASCII.GetString(pc.appearance.name).TrimEnd('\0'));

		Load();
		SetEquipToCurrentJob();
	}

	public void ChangeJob(int jobId)
	{
		_jobId = jobId;
		SetEquipToCurrentJob();
	}

	public void SetEquipToCurrentJob()
	{
		switch(_jobId)
		{
			case 1:
				_currentCharacter.appearance.equiped = new ModEquipped(_equipment.Combat.GetEquipped());
				break;

			default:
				_currentCharacter.appearance.equiped = new ModEquipped(_equipment.Life.GetEquipped());
				break;
		}
	}

	private BaseStats CombatBaseStats()
	{
		return new BaseStats()
		{
			Strength = _currentCharacter.status.combatJob.strength,
			Intelligence = _currentCharacter.status.combatJob.intelligence,
			Dexterity = _currentCharacter.status.combatJob.dexterity,
			Agility = _currentCharacter.status.combatJob.agility,
			Vitality = _currentCharacter.status.combatJob.vitality,
			Spirit = _currentCharacter.status.combatJob.spirit,
			Lucky = _currentCharacter.status.combatJob.lucky
		};
	}

	private AttackStatus GetMeleeAttack()
	{
		var stats = CombatBaseStats();
		var level = _currentCharacter.status.combatJob.lv;
		var equip = GetEquipment(true);
		var weapon = equip.GetItemInSlot(EQUIP_SLOT.MAIN_EQUIP) as Item;

		// Get equipment bonuses
		int equipAtk = equip.GetInheritBonus(InheritType.ExtraMeleeAtk);
		int equipDef = equip.GetInheritBonus(InheritType.ExtraMeleeDefense);
		int equipHit = equip.GetInheritBonus(InheritType.HitAccurate);
		int equipCrit = equip.GetInheritBonus(InheritType.ExtraCriticalRate);
		int equipDodge = equip.GetInheritBonus(InheritType.ExtraDodge);

		// Equipment stat bonuses
		int equipStr = equip.GetInheritBonus(InheritType.ExtraSTR);
		int equipDex = equip.GetInheritBonus(InheritType.ExtraDEX);
		int equipAgi = equip.GetInheritBonus(InheritType.ExtraAGI);
		int equipVit = equip.GetInheritBonus(InheritType.ExtraVIT);
		int equipLuk = equip.GetInheritBonus(InheritType.ExtraLUK);

		// Total stats including equipment
		int totalStr = stats.Strength + equipStr;
		int totalDex = stats.Dexterity + equipDex;
		int totalAgi = stats.Agility + equipAgi;
		int totalVit = stats.Vitality + equipVit;
		int totalLuk = stats.Lucky + equipLuk;

		// Base calculations from stats
		int baseAtk = 3 * ((level * 5) + (totalStr + (totalStr / 5)) + (totalDex / 8) + (totalLuk / 10));
		int baseDef = (level * 3) + (totalVit + (totalVit / 5));
		int baseHit = (level * 2) + (totalDex + (totalDex / 5));
		int baseCrit = 1 + (totalLuk / 5);
		int baseFlee = (level * 3) + (totalAgi + (totalAgi / 5));

		return new AttackStatus()
		{
			WeaponTypeId = weapon is null ? 0 : (uint)weapon.WeaponType,
			Atk = (ushort)(baseAtk + equipAtk),
			CritRate = (ushort)(baseCrit + equipCrit),
			Def = (ushort)(baseDef + equipDef),
			Hit = (ushort)(baseHit + equipHit),
			FleeRate = (ushort)(baseFlee + equipDodge)
		};
	}

	/// <summary>
	/// Gets the off-hand weapon attack stats for dual-wielding.
	/// Returns null if no off-hand weapon is equipped.
	/// </summary>
	private AttackStatus? GetSubAttack()
	{
		var equip = GetEquipment(true);
		var subWeapon = equip.GetItemInSlot(EQUIP_SLOT.SUB_EQUIP) as Item;

		// Check if sub slot has a weapon (not a shield)
		if (subWeapon == null || !IsOffHandWeapon(subWeapon))
			return null;

		var stats = CombatBaseStats();
		var level = _currentCharacter.status.combatJob.lv;

		// Equipment stat bonuses (shared across both weapons)
		int equipStr = equip.GetInheritBonus(InheritType.ExtraSTR);
		int equipDex = equip.GetInheritBonus(InheritType.ExtraDEX);
		int equipAgi = equip.GetInheritBonus(InheritType.ExtraAGI);
		int equipLuk = equip.GetInheritBonus(InheritType.ExtraLUK);

		int totalStr = stats.Strength + equipStr;
		int totalDex = stats.Dexterity + equipDex;
		int totalLuk = stats.Lucky + equipLuk;

		// Off-hand weapon uses same formula but with weapon-specific bonuses
		// Get inherits from the off-hand weapon specifically
		int subWeaponAtk = 0;
		int subWeaponHit = 0;
		int subWeaponCrit = 0;
		if (subWeapon.Inherits != null)
		{
			foreach (var inherit in subWeapon.Inherits)
			{
				switch ((InheritType)inherit.typeID)
				{
					case InheritType.ExtraMeleeAtk:
						subWeaponAtk += inherit.val;
						break;
					case InheritType.HitAccurate:
						subWeaponHit += inherit.val;
						break;
					case InheritType.ExtraCriticalRate:
						subWeaponCrit += inherit.val;
						break;
				}
			}
		}

		int baseAtk = 3 * ((level * 5) + (totalStr + (totalStr / 5)) + (totalDex / 8) + (totalLuk / 10));
		int baseHit = (level * 2) + (totalDex + (totalDex / 5));
		int baseCrit = 1 + (totalLuk / 5);

		return new AttackStatus()
		{
			WeaponTypeId = (uint)subWeapon.WeaponType,
			Atk = (ushort)(baseAtk + subWeaponAtk),
			CritRate = (ushort)(baseCrit + subWeaponCrit),
			Def = 0, // Off-hand doesn't contribute to defense
			Hit = (ushort)(baseHit + subWeaponHit),
			FleeRate = 0 // Off-hand doesn't contribute to flee
		};
	}

	/// <summary>
	/// Checks if the item in the off-hand slot is a weapon (not a shield).
	/// </summary>
	private static bool IsOffHandWeapon(Item item)
	{
		// Shields are not weapons for dual-wield purposes
		var weaponType = (WeaponType)item.WeaponType;
		return weaponType != WeaponType.WoodenShield &&
		       weaponType != WeaponType.MetalShield &&
		       weaponType != WeaponType.BareHand &&
		       item.Type == (int)ItemType.AuxiliaryEquipment;
	}

	/// <summary>
	/// Checks if the main weapon is a ranged weapon.
	/// </summary>
	private bool IsUsingRangedWeapon()
	{
		var equip = GetEquipment(true);
		var weapon = equip.GetItemInSlot(EQUIP_SLOT.MAIN_EQUIP) as Item;
		if (weapon == null)
			return false;

		var weaponType = (WeaponType)weapon.WeaponType;
		return weaponType == WeaponType.Pistol ||
		       weaponType == WeaponType.LongGun ||
		       weaponType == WeaponType.Bow ||
		       weaponType == WeaponType.Crossbow;
	}

	private AttackStatus GetSpellAttack()
	{
		var stats = CombatBaseStats();
		var level = _currentCharacter.status.combatJob.lv;
		var equip = GetEquipment(true);
		var weapon = equip.GetItemInSlot(EQUIP_SLOT.MAIN_EQUIP) as Item;

		// Get equipment bonuses
		int equipAtk = equip.GetInheritBonus(InheritType.ExtraSpellAtk);
		int equipDef = equip.GetInheritBonus(InheritType.ExtraSpellDefense);
		int equipHit = equip.GetInheritBonus(InheritType.HitAccurate);
		int equipCrit = equip.GetInheritBonus(InheritType.ExtraCriticalRate);
		int equipDodge = equip.GetInheritBonus(InheritType.ExtraDodge);

		// Equipment stat bonuses
		int equipInt = equip.GetInheritBonus(InheritType.ExtraINT);
		int equipDex = equip.GetInheritBonus(InheritType.ExtraDEX);
		int equipAgi = equip.GetInheritBonus(InheritType.ExtraAGI);
		int equipVit = equip.GetInheritBonus(InheritType.ExtraVIT);
		int equipSpi = equip.GetInheritBonus(InheritType.ExtraSPI);
		int equipLuk = equip.GetInheritBonus(InheritType.ExtraLUK);

		// Total stats including equipment
		int totalInt = stats.Intelligence + equipInt;
		int totalDex = stats.Dexterity + equipDex;
		int totalAgi = stats.Agility + equipAgi;
		int totalVit = stats.Vitality + equipVit;
		int totalSpi = stats.Spirit + equipSpi;
		int totalLuk = stats.Lucky + equipLuk;

		// Base calculations from stats (magic uses INT and SPI instead of STR)
		int baseAtk = (level * 5) + (totalInt + (totalInt / 5)) + (totalDex / 8) + (totalSpi / 10);
		int baseDef = (level * 3) + (totalVit + (totalVit / 5));
		int baseHit = (level * 2) + (totalDex + (totalDex / 5));
		int baseCrit = 1 + (totalLuk / 5);
		int baseFlee = (level * 3) + (totalAgi + (totalAgi / 5));

		return new AttackStatus()
		{
			WeaponTypeId = weapon is null ? 0 : (uint)weapon.WeaponType,
			Atk = (ushort)(baseAtk + equipAtk),
			CritRate = (ushort)(baseCrit + equipCrit),
			Def = (ushort)(baseDef + equipDef),
			Hit = (ushort)(baseHit + equipHit),
			FleeRate = (ushort)(baseFlee + equipDodge)
		};
	}

	private BasicStatus GetBasicStatus()
	{
		return new BasicStatus()
		{
			Hp = _status.hp,
			MaxHp = _status.mhp,
			Mp = _status.mp,
			MaxMp = _status.mmp
		};
	}

	/// <summary>
	/// Gets the player's life job stats with equipment bonuses applied.
	/// Used for crafting, gathering, and other life skill activities.
	/// </summary>
	public LifeJobStats GetLifeJobStats()
	{
		var lifeStatus = _currentCharacter.status.lifeJob;
		var equip = (PlayerEquipment)GetEquipment(false); // Life job equipment

		// Get equipment stat bonuses (using life job inherit types)
		int equipIdea = equip.GetInheritBonus(InheritType.ExtraIDE);
		int equipMind = equip.GetInheritBonus(InheritType.ExtraMID);
		int equipCraft = equip.GetInheritBonus(InheritType.ExtraCRT);
		int equipSense = equip.GetInheritBonus(InheritType.ExtraSES);

		// Get equipment-based derived stat bonuses
		int collectSuccessRate = equip.GetInheritBonus(InheritType.TransMeleeHitRate); // Note: No direct mapping, using placeholder
		int collectionIncreaseRate = equip.GetInheritBonus(InheritType.TransDodge); // Note: No direct mapping, using placeholder
		int makeTimeDecrease = equip.GetInheritBonus(InheritType.ExtraCastingTimeDecrease);

		return new LifeJobStats()
		{
			Lv = lifeStatus.lv,
			Exp = lifeStatus.exp,
			StatusPoint = lifeStatus.statusPoint,
			Idea = (ushort)(lifeStatus.idea + equipIdea),
			Mind = (ushort)(lifeStatus.mind + equipMind),
			Craft = (ushort)(lifeStatus.craft + equipCraft),
			Sense = (ushort)(lifeStatus.sense + equipSense),
			CollectSuccessRate = collectSuccessRate,
			CollectionIncreaseRate = collectionIncreaseRate,
			MakeTimeDecrease = makeTimeDecrease
		};
	}

	public FPOS GetPosition()
	{
		return _worldPosition.Position;
	}

	public FPOS GetDirection()
	{
		return _worldPosition.Direction;
	}

	public void UpdatePosition(FPOS pos)
	{
		_worldPosition.Position = pos;
	}

	public void UpdateDirection(FPOS dir)
	{
		_worldPosition.Direction = dir;
	}

	public uint GetZone()
	{
		return _worldPosition.ZoneID;
	}

	public void SetZone(uint zone)
	{
		_worldPosition.ZoneID = zone;
	}

	public ModClientPC GetCurrentCharacter()
	{
		return _currentCharacter;
	}

	public REGION_PC GetRegionPC()
	{
		//_worldPosition.Position = new FPOS() { x = 1000, y = 1000, z = 850 };
		return new()
		{
			objInstID = _objInstID,
			pos = _worldPosition.Position,
			dir = _worldPosition.Direction,
			deltaLookAtRadian = 2,
			status = new()
			{
				commonStatus = _status.AsStruct(),
				lp = _currentCharacter.status.lp,
				mlp = _currentCharacter.status.lp,
				streamPoint = _currentCharacter.status.streamPoint,
				meleeHitRate = 1,
				dodge = 1,
				meleeAtk = 1,
				meleeDefense = 1,
				spellAtk = 1,
				spellDefense = 1,
				parry = 1,
				block = 1,
				resist = 1,
				criticalRate = 1,
				hitSpeedRatio = 1,
				
				lifeJob = new()
				{
					lv = _currentCharacter.status.lifeJob.lv,
					exp = _currentCharacter.status.lifeJob.exp,
					statusPoint = _currentCharacter.status.lifeJob.statusPoint,
					craft = _currentCharacter.status.lifeJob.craft,
					idea = _currentCharacter.status.lifeJob.idea,
					mind = _currentCharacter.status.lifeJob.mind,
					sense = _currentCharacter.status.lifeJob.sense
				},
				combatJob = new()
				{
					lv = _currentCharacter.status.combatJob.lv,
					exp = _currentCharacter.status.combatJob.exp,
					strength = _currentCharacter.status.combatJob.strength,
					intelligence = _currentCharacter.status.combatJob.intelligence,
					dexterity = _currentCharacter.status.combatJob.dexterity,
					agility = _currentCharacter.status.combatJob.agility,
					vitality = _currentCharacter.status.combatJob.vitality,
					spirit = _currentCharacter.status.combatJob.spirit,
					lucky = _currentCharacter.status.combatJob.lucky
				},
				velocities = _velocities,
				collectSucessRate = 1,
				collectionIncreaseRate = 1,
				makeTimeDecreaseAmount = 1
			},
			name = _currentCharacter.appearance.name,
			raceTypeID = _currentCharacter.appearance.raceTypeID,
			lifeJobTypeID = _currentCharacter.appearance.lifeJobTypeID,
			combatJobTypeID = _currentCharacter.appearance.combatJobTypeID,
			genderType = _currentCharacter.appearance.genderType,
			headType = _currentCharacter.appearance.headType,
			hairType = _currentCharacter.appearance.hairType,
			eyeType = _currentCharacter.appearance.eyeType,
			earType = _currentCharacter.appearance.earType,
			playingJobClass = _currentCharacter.appearance.playingJobClass,
			underwearType = _currentCharacter.appearance.underwearType,
			familyNameType = _currentCharacter.appearance.familyNameType,
			streamPoint = _currentCharacter.status.streamPoint,
			transparent = _currentCharacter.appearance.transparent,
			scale = _currentCharacter.appearance.scale,
			guildName = "Test Guild",
			showHelm = _currentCharacter.appearance.showHelm,
			inventoryGrade = _inventory.Grade,
			skinColorType = _currentCharacter.appearance.skinColorType,
			hairColorType = _currentCharacter.appearance.hairColorType,
			eyeColorType = _currentCharacter.appearance.eyeColorType,
			eyeBrowColorType = _currentCharacter.appearance.eyeBrowColorType
		};
	}

	public SC_ENTER_SIGHT_PC GetEnterSight()
	{
		var guildNameBytes = new byte[51];
		var guildNameStr = GetRegionPC().guildName;
		if (!string.IsNullOrEmpty(guildNameStr))
		{
			var guildNameData = global::System.Text.Encoding.ASCII.GetBytes(guildNameStr);
			Array.Copy(guildNameData, guildNameBytes, Math.Min(guildNameData.Length, 50));
		}

		return new()
		{
			enter_zone = new()
			{
				objInstID = GetObjInstID(),
				pos = GetPosition(),
				dir = GetDirection()
			},
			deltaLookAtRadian = 2,
			appearance = GetCurrentCharacter().appearance.AsStruct(),
			guildName = guildNameBytes,
			raceRelationState = 1,
			stopType = (byte)STOP_TYPE.STOP_TYPE_GROUND
		};
	}

	public async Task RequestPresence(CancellationToken token)
	{
		await RequestZonePresence!.Invoke(this, token);
	}

	public COMMON_STATUS RequestCommonStatus(long id)
	{
		return RequestStatus!.Invoke(id);
	}

	public bool KnowOf(uint id)
	{
		return _knownPc.Contains(id);
	}

	public void Seen(uint id)
	{
		_knownPc.Add(id);
	}

	public void PcLeft(uint id)
	{
		_knownPc.Remove(id);
	}

	public VELOCITIES GetVelocities()
	{
		return _velocities;
	}

	public void UpdateVelocities(VELOCITIES vel)
	{
		_velocities = vel;
	}

	public void SetInMotion(bool inMotion)
	{
		_inMotion = inMotion;
	}

	public bool IsInMotion()
	{
		return _inMotion;
	}

	public COMMON_STATUS GetStatus()
	{
		return _status.AsStruct();
	}

	public void UpdateStatus(ModCommonStatus status)
	{
		_status = status;
	}

	public void AddNpcOrMob(INpc npc)
	{
		AddNpc!.Invoke(npc);
	}

	public void SetCurrentTarget(long target)
	{
		_target = target;
	}

	public long GetCurrentTarget()
	{
		return _target;
	}

	public void InitCombat()
	{
		_inCombat = true;
		_lastCombatTime = DateTime.Now;
	}

	public bool InCombat()
	{
		return _inCombat;
	}

	public void StopCombat()
	{
		_inCombat = false;
		_lastCombatTime = DateTime.MinValue;
	}

	/// <summary>
	/// Checks if player is currently in combat or was recently in combat.
	/// Used for natural regeneration checks.
	/// </summary>
	public bool IsInCombat()
	{
		if (_inCombat)
			return true;

		// Consider "in combat" if combat ended less than 10 seconds ago
		if (_lastCombatTime != DateTime.MinValue &&
		    (DateTime.Now - _lastCombatTime).TotalSeconds < 10)
			return true;

		return false;
	}

	/// <summary>
	/// Records the last combat time for out-of-combat regen checks.
	/// </summary>
	public void RecordCombatActivity()
	{
		_lastCombatTime = DateTime.Now;
	}

	/// <summary>
	/// Sends a full status update packet to the client and nearby players.
	/// </summary>
	public void SendStatusUpdate()
	{
		SC_COMMON_STATUS statusPacket = new()
		{
			objInstID = GetObjInstID(),
			status = _status.AsStruct()
		};

		using PacketWriter pw = new();
		pw.Write(statusPacket);
		byte[] packet = pw.ToPacket();

		Send(packet, default).Wait();
	}

	public async Task Update(ulong tick)
	{
		return;
	}

	public IPlayerInventory GetInventory()
	{
		return _inventory;
	}

	public IPlayerEquipment GetEquipment(bool combat)
	{
		return combat ? _equipment.Combat : _equipment.Life;
	}

	public IPlayerQuests GetQuests()
	{
		return _quests;
	}

	public void SendInventory()
	{
		using PacketWriter pw = new();
		pw.Write(_inventory.AsInventoryPacket());
		Send(pw.ToSizedPacket(), default).Wait();
	}

	public void SendEquipment()
	{
		using (PacketWriter pw = new())
		{
			pw.Write(_equipment.Combat.GetCombatEquipList());
			Send(pw.ToSizedPacket(), default).Wait();
		}

		using (PacketWriter pw = new())
		{
			pw.Write(_equipment.Life.GetLifeEquipList());
			Send(pw.ToSizedPacket(), default).Wait();
		}
	}

	/// <summary>
	/// Sends the list of active buffs/debuffs to the client.
	/// </summary>
	public void SendBuffList()
	{
		var buffPacket = LimeServer.BuffService.BuildDefListPacket(this);
		Send(buffPacket, default).Wait();
	}

	public long GetId()
	{
		return GetObjInstID();
	}

	/// <summary>
	/// Gets the entity type ID. Returns 0 for players (not a monster/NPC type).
	/// </summary>
	/// <returns>0 for players.</returns>
	public int GetEntityTypeId() => 0;

	public EntityStatus GetEntityStatus()
	{
		// Combat job (jobId 1) - return combat-focused stats
		if (_jobId == 1)
		{
			return new EntityStatus()
			{
				Lv = _currentCharacter.status.combatJob.lv,
				Exp = _currentCharacter.status.combatJob.exp,
				BaseStats = CombatBaseStats(),
				BasicStatus = GetBasicStatus(),
				MeleeAttack = GetMeleeAttack(),
				SpellAttack = GetSpellAttack(),
				SubAttack = GetSubAttack(),
				IsRanged = IsUsingRangedWeapon(),
				LifeJobStats = null
			};
		}

		// Life job (any other jobId) - return life job stats with basic combat
		// Life jobs still have combat stats for when attacked, but primarily use life stats
		return new EntityStatus()
		{
			Lv = _currentCharacter.status.lifeJob.lv,
			Exp = _currentCharacter.status.lifeJob.exp,
			BaseStats = CombatBaseStats(), // Combat stats still apply for defense
			BasicStatus = GetBasicStatus(),
			MeleeAttack = GetMeleeAttack(), // Reduced combat effectiveness could be applied here
			SpellAttack = GetSpellAttack(),
			SubAttack = GetSubAttack(),
			IsRanged = IsUsingRangedWeapon(),
			LifeJobStats = GetLifeJobStats()
		};
	}

	public DamageResult UpdateHealth(int healthChange)
	{
		var newHealth = (_status.hp + healthChange);
		_status.hp = newHealth <= 0 ? 0 : (uint)newHealth;

		return new DamageResult()
		{
			TargetKilled = _status.hp == 0,
			ExpReward = 0
		};
	}

	/// <summary>
	/// Updates the player's HP and optionally broadcasts the change to nearby players
	/// </summary>
	/// <param name="newHP">The new HP value</param>
	/// <param name="broadcast">Whether to broadcast the change to nearby players</param>
	public void UpdateHP(int newHP, bool broadcast = true)
	{
		int oldHP = (int)_status.hp;
		_status.hp = newHP < 0 ? 0 : (uint)newHP;
		if (_status.hp > _status.mhp)
			_status.hp = _status.mhp;

		SC_CHANGED_HP hpPacket = new()
		{
			objInstID = GetObjInstID(),
			current = (int)_status.hp,
			update = (int)_status.hp - oldHP,
			fromID = 0
		};

		using PacketWriter pw = new();
		pw.Write(hpPacket);
		byte[] packet = pw.ToPacket();

		Send(packet, default).Wait();
		if (broadcast)
			SendGlobalPacket(packet, default).Wait();
	}

	/// <summary>
	/// Updates the player's MP and optionally broadcasts the change to nearby players
	/// </summary>
	/// <param name="newMP">The new MP value</param>
	/// <param name="broadcast">Whether to broadcast the change to nearby players</param>
	public void UpdateMP(int newMP, bool broadcast = true)
	{
		int oldMP = (int)_status.mp;
		_status.mp = newMP < 0 ? 0 : (uint)newMP;
		if (_status.mp > _status.mmp)
			_status.mp = _status.mmp;

		SC_CHANGED_MP mpPacket = new()
		{
			objInstID = GetObjInstID(),
			current = (int)_status.mp,
			update = (int)_status.mp - oldMP
		};

		using PacketWriter pw = new();
		pw.Write(mpPacket);
		byte[] packet = pw.ToPacket();

		Send(packet, default).Wait();
		if (broadcast)
			SendGlobalPacket(packet, default).Wait();
	}

	/// <summary>
	/// Handles taking damage, updating HP and handling death if HP reaches 0
	/// </summary>
	/// <param name="damage">Amount of damage to take</param>
	/// <returns>DamageResult containing whether target was killed</returns>
	public DamageResult TakeDamage(int damage)
	{
		int newHP = (int)_status.hp - damage;
		UpdateHP(newHP, true);

		bool killed = _status.hp == 0;
		if (killed)
		{
			SendDeath();
		}

		return new DamageResult()
		{
			TargetKilled = killed,
			ExpReward = 0
		};
	}

	/// <summary>
	/// Sends death notification to player and nearby players.
	/// </summary>
	/// <remarks>
	/// Sets the player's death state, applies death penalty (EXP loss),
	/// and broadcasts the death to all nearby players.
	/// </remarks>
	public void SendDeath()
	{
		// Mark player as dead
		SetDead(true);

		// Apply death penalty (lose 5% of current level EXP)
		ApplyDeathPenalty();

		SC_DEAD deathPacket = new()
		{
			objInstID = GetObjInstID()
		};

		using PacketWriter pw = new();
		pw.Write(deathPacket);
		byte[] packet = pw.ToPacket();

		Send(packet, default).Wait();
		SendGlobalPacket(packet, default).Wait();

		Logger.Log($"[DEATH] Player {GetCurrentCharacter().appearance.name} ({GetObjInstID()}) has died", LogLevel.Information);
	}

	/// <summary>
	/// Applies the death penalty to the player (EXP loss).
	/// </summary>
	/// <remarks>
	/// Players lose 5% of their current EXP on death.
	/// EXP cannot go below 0.
	/// </remarks>
	private void ApplyDeathPenalty()
	{
		uint currentExp = _currentCharacter.status.combatJob.exp;
		uint expLoss = currentExp / 20; // 5% loss

		if (expLoss > 0)
		{
			_currentCharacter.status.combatJob.exp = currentExp - expLoss;
			Logger.Log($"[DEATH] {GetCurrentCharacter().appearance.name} lost {expLoss} EXP (death penalty)", LogLevel.Debug);
		}
	}

	/// <summary>
	/// Resurrects the player with the specified HP.
	/// </summary>
	/// <param name="hp">HP amount after resurrection.</param>
	/// <remarks>
	/// Clears the death state and restores player HP.
	/// Does not send packets - caller is responsible for broadcasting SC_RESURRECTED.
	/// </remarks>
	public void Resurrect(uint hp)
	{
		if (!_isDead)
		{
			Logger.Log($"[RESURRECT] Attempted to resurrect {GetCurrentCharacter().appearance.name} but player is not dead", LogLevel.Warning);
			return;
		}

		// Clear death state
		SetDead(false);

		// Restore HP
		UpdateHP((int)hp, false);

		Logger.Log($"[RESURRECT] {GetCurrentCharacter().appearance.name} resurrection complete with {hp} HP", LogLevel.Debug);
	}

	public bool AddExp(ulong exp)
	{
		var currentLevel = _currentCharacter.status.combatJob.lv;
		_currentCharacter.status.combatJob.exp += (uint)exp;
		var expList = PcStatus.GetEntries();

		if (expList[currentLevel].CombatExp >= _currentCharacter.status.combatJob.exp)
			return false;

		_currentCharacter.status.combatJob.lv++;
		_currentCharacter.status.combatJob.exp = (uint)(_currentCharacter.status.combatJob.exp - expList[currentLevel].CombatExp);
		return true;
	}

	public List<Item> GetLoot()
	{
		throw new NotImplementedException();
	}

	public void Loot(Item item)
	{
		throw new NotImplementedException();
	}

	// Anti-cheat helper methods
	public uint GetLastClientTick() => _lastClientTick;
	public void UpdateClientTick(uint tick) => _lastClientTick = tick;

	public FPOS GetLastPosition() => _lastPosition;
	public void UpdateLastPosition(FPOS pos) => _lastPosition = pos;

	public DateTime GetLastMoveTime() => _lastMoveTime;
	public void UpdateLastMoveTime() => _lastMoveTime = DateTime.Now;

	public uint GetMovePacketCount() => _movePacketCount;
	public void IncrementMovePacketCount() => _movePacketCount++;

	public bool IsJumping() => _isJumping;
	public void SetJumping(bool jumping)
	{
		_isJumping = jumping;
		if (jumping)
		{
			_jumpStartTime = DateTime.Now;
		}
	}

	public TimeSpan GetJumpDuration()
	{
		if (!_isJumping || _jumpStartTime == DateTime.MinValue)
			return TimeSpan.Zero;
		return DateTime.Now - _jumpStartTime;
	}

	/// <summary>
	/// Gets whether the player is currently underwater.
	/// </summary>
	public bool IsUnderwater() => _isUnderwater;

	/// <summary>
	/// Sets the player's underwater state.
	/// </summary>
	public void SetUnderwater(bool underwater) => _isUnderwater = underwater;

	/// <summary>
	/// Gets whether the player is currently dead.
	/// </summary>
	/// <returns>True if the player is dead (HP = 0).</returns>
	public bool IsDead() => _isDead;

	/// <summary>
	/// Gets the time when the player died.
	/// </summary>
	/// <returns>DateTime of death, or DateTime.MinValue if not dead.</returns>
	public DateTime GetDeathTime() => _deathTime;

	/// <summary>
	/// Sets the player's death state.
	/// </summary>
	/// <param name="dead">Whether the player is dead.</param>
	private void SetDead(bool dead)
	{
		_isDead = dead;
		if (dead)
		{
			_deathTime = DateTime.UtcNow;
			_inCombat = false;
		}
		else
		{
			_deathTime = DateTime.MinValue;
		}
	}

	/// <summary>
	/// Sets the quest ID that the player is about to turn in.
	/// </summary>
	/// <remarks>
	/// Called when server sends SC_QUEST_REPORT_TALK to track which quest
	/// is being turned in. CS_QUEST_COMPLETE does not include quest ID.
	/// </remarks>
	/// <param name="questTypeID">Quest type ID being turned in, or 0 to clear.</param>
	public void SetPendingTurnInQuest(uint questTypeID) => _pendingTurnInQuestId = questTypeID;

	/// <summary>
	/// Gets the quest ID that the player is currently turning in.
	/// </summary>
	/// <returns>Quest type ID, or 0 if no pending turn-in.</returns>
	public uint GetPendingTurnInQuest() => _pendingTurnInQuestId;

	/// <summary>
	/// Clears the pending turn-in quest (after completion or cancel).
	/// </summary>
	public void ClearPendingTurnInQuest() => _pendingTurnInQuestId = 0;

	/// <summary>
	/// Get the skill cooldown tracker for this player
	/// </summary>
	public SkillCooldownTracker? GetSkillCooldownTracker() => _skillCooldownTracker;

	/// <summary>
	/// Calculate maximum allowed speed based on movement type and player stats
	/// </summary>
	public float GetMaxSpeed(byte moveType)
	{
		// Base speed from constants
		float baseSpeed = moveType switch
		{
			1 => Constants.GameConstants.Movement.PC_RUN_SPEED,           // MOVE_RUN
			2 => Constants.GameConstants.Movement.PC_WALK_SPEED,          // MOVE_WALK
			3 => Constants.GameConstants.Movement.PC_WALK_BACK_SPEED,     // MOVE_WALK_BACK
			4 => Constants.GameConstants.Movement.PC_SWIMMING_SPEED,      // MOVE_SWIM
			5 => Constants.GameConstants.Movement.PC_SWIMMING_BACK_SPEED, // MOVE_SWIM_BACK
			_ => Constants.GameConstants.Movement.PC_RUN_SPEED
		};

		// Apply velocity ratio from player stats
		return baseSpeed * _velocities.ratio;
	}

	/// <summary>
	/// Get the client's IP address
	/// </summary>
	public string GetIpAddress()
	{
		return _socketClient.GetIP();
	}

	/// <summary>
	/// Forcibly disconnect this player
	/// </summary>
	public void Disconnect()
	{
		Save();
		_socketClient.Disconnect();
	}
}
