using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.MonsterXML;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.Entities.Monsters;

public partial class Monster : INpc, IEntity
{
	private readonly XmlMonster _mobInfo;

	public PosCalc mobPosition;

	public FPOS Position { get; set; }
	public FPOS Direction { get; set; }
	public uint Zone { get; set; }

	public uint Id;
	public uint ModelId;
	public string Name;

	private uint Lv { get; set; }
	public uint MHP { get; set; }
	public uint HP { get; set; }
	public uint MMP { get; set; }
	public uint MP { get; set; }

	private MobState _currentState { get; set; }

	public PlayerClient? CurrentTarget { get; set; }
	public bool Aggro { get; set; }

	private FPOS _destination;
	private FPOS _originalPosition;

	private static readonly Random _rng = Random.Shared;
	private uint _nextRoamDecisionTick = 0;

	private uint _actionStartTick;
	private uint _lastTick;
	public bool IsMoving;

	public bool Despawned = false;

	private MOVE_TYPE _moveType = MOVE_TYPE.MOVE_TYPE_WALK;

	private readonly uint update_distance = 1500;
	private readonly uint aggro_range = 150;

	private List<LootableItem> Lootable { get; set; } = new();
	private List<Item>? Loot { get; set; }

	public Monster(XmlMonster mobInfo, uint id, FPOS pos, FPOS dir, uint zone, bool aggro, int lootTable)
	{
		_mobInfo = mobInfo;

		mobPosition = new PosCalc();

		Position = pos;
		Direction = dir;
		Zone = zone;
		Id = id;
		_originalPosition = pos with { };

		Lv = (uint)_mobInfo.Level;
		MMP = (uint)_mobInfo.MMP;
		MHP = (uint)_mobInfo.MHP;
		MP = (uint)_mobInfo.MP;
		HP = (uint)_mobInfo.HP;
		Aggro = aggro;

		Name = mobInfo.Name;
		ModelId = (uint)_mobInfo.ModelTypeID;

		CurrentTarget = null;
		_currentState = MobState.Roaming;

		var dropTable = WorldDataLoader.LoadDropTables();
		if (dropTable.TryGetValue(lootTable, out var loot))
		{
			Lootable.AddRange(loot);
		}
	}

	public SC_ENTER_SIGHT_MONSTER GetEnterSight()
	{
		return GetMob().GetEnterSight();
	}

	public SC_LEAVE_SIGHT_MONSTER GetLeaveSight()
	{
		return GetMob().GetLeaveSight();
	}

	public Mob GetMob()
	{
		byte[] nameBytes = new byte[31];
		byte[] sourceBytes = Encoding.GetEncoding(949).GetBytes(Name);
		Array.Copy(sourceBytes, nameBytes, Math.Min(sourceBytes.Length, 30));

		return new Mob()
		{
			Id = (int)Id,
			Pos = Position,
			Dir = Direction,
			Appearance = new()
			{
				appearance = new()
				{
					name = nameBytes,
					action = _mobInfo.Subjects.First().EventID,
					actionStartTick = 0,
					scale = 1,
					transparent = 1,
					modelTypeID = ModelId,
					color = new()
					{
						r = 0,
						g = 0,
						b = 0
					},
					typeID = (uint)_mobInfo.TypeID
				},
				aggresive = Aggro,
				shineWhenHitted = _mobInfo.ShineWhenHitted != 0
			},
			RaceRelationState = 0,
			StopType = 0,
			ZoneId = Zone,
			Status = new()
			{
				lv = (byte)Lv,
				mhp = MHP,
				hp = HP,
				mmp = MMP,
				mp = MP
			}
		};
	}

	public void Update(uint serverTick, ReadOnlySpan<PlayerClient> playerClients)
	{
		if (serverTick - _lastTick < 100)
			return;
		_lastTick = serverTick;

		// Monster dead
		if (HP == 0)
		{
			if (CurrentTarget != null)
				CurrentTarget = null;

			if (!Despawned && serverTick - _actionStartTick > 30_000)
			{
				_actionStartTick = serverTick;
				Despawned = true;
				using PacketWriter pw = new();
				pw.Write(GetLeaveSight());
				SendToNearbyPlayers(pw.ToPacket(), playerClients);
			}
			else if (Despawned && serverTick - _actionStartTick > 3 * 30_000)
			{
				Spawn();
				_actionStartTick = serverTick;
				Despawned = false;

				using PacketWriter pw = new();
				pw.Write(GetEnterSight());
				SendToNearbyPlayers(pw.ToSizedPacket(), playerClients);
			}

			return;
		}


		if (CurrentTarget != null)
		{
			bool updated = false;
			foreach(PlayerClient client in playerClients)
			{
				if (client.GetObjInstID() == CurrentTarget.GetObjInstID())
				{
					CurrentTarget = client;
					updated = true;
					break;
				}
			}

			if (!updated)
				CurrentTarget = null;
		}


		switch(_currentState)
		{
			case MobState.Roaming:
				if (IsPlayerWithinAggroZone(serverTick, playerClients))
					return;

				Roam(serverTick, playerClients);
				break;


			case MobState.Chasing:
				ChasePlayer(serverTick, playerClients);
				break;


			case MobState.Attacking:
				AttackPlayer(serverTick, playerClients);
				break;
		}
	}

	private void MoveTowardsDestination(uint serverTick, ReadOnlySpan<PlayerClient> playerClients, double distance = 0.1)
	{
		// Use the same position interpolation used elsewhere
		var currentPos = GetMobCurrentPosition(serverTick);
		if (currentPos.IsNaN())
			currentPos = Position;

		double distanceToTarget = currentPos.CalculateDistance(_destination);

		// Arrival threshold: at least the provided 'distance' but scaled with velocity so
		// faster monsters get a larger acceptance window and avoid overshoot/oscillation.
		double velocity = GetCurrentVelocity();
		double arrivalThreshold = Math.Max(distance, velocity * 0.11); // multiplier tuned experimentally

		// If we've reached (or are sufficiently close), snap to exact destination and stop.
		if (currentPos.Compare(_destination) || distanceToTarget <= arrivalThreshold)
		{
			// Snap to the destination exactly to avoid floating-point jitter
			if (!_destination.IsNaN())
				Position = _destination;
			else
				Position = currentPos;

			_destination = default;

			IsMoving = false;
			_actionStartTick = 0;

			// Optionally reset mobPosition state to a non-moving state to keep systems consistent
			// (PosCalc.Start is called when movement begins; leaving mobPosition as-is is acceptable,
			// but if you observe drift you can reinitialize it here.)
			var sc_stop = GetStopPacket(Position, serverTick);
			SendToNearbyPlayers(sc_stop, playerClients);
			return;
		}

		// While moving, keep facing direction updated toward the destination for smooth visuals.
		var dir = currentPos.CalculateDirection(_destination);
		if (!dir.IsNaN())
			Direction = dir;

		// Build and broadcast movement packet using the same current position.
		var pck = GetMovePacket(currentPos, serverTick);
		SendToNearbyPlayers(pck, playerClients);
	}

	private void SendToNearbyPlayers(byte[] packet, ReadOnlySpan<PlayerClient> playerClients)
	{
		foreach (var client in playerClients)
		{
			if (!client.IsLoaded() || client.GetPosition().CalculateDistance(Position) > update_distance)
				continue;

			client.Send(packet, default).Wait();
		}
	}

	private FPOS GetMobCurrentPosition(uint serverTick)
	{
		if (!IsMoving)
			return Position;

		var currentPos = Position.CalculateCurrentPosition(_destination, GetCurrentVelocity(), (serverTick - _actionStartTick) / 1000);
		if (currentPos.IsNaN())
			return _destination;

		return currentPos;
	}

	private void ReturnHome(uint serverTick)
	{
		CurrentTarget = null;
		_currentState = MobState.Roaming;
		_moveType = MOVE_TYPE.MOVE_TYPE_WALK;
		Position = GetMobCurrentPosition(serverTick);
		IsMoving = false;
		SetNewDestination(_originalPosition, serverTick);
	}

	private void SetNewDestination(FPOS destination, uint serverTick)
	{
		// Calculate direction towards the provided destination
		var dirToDest = Position.CalculateDirection(destination);
		var angel = (float)PosCalc.GetAngleRadian(dirToDest.x, dirToDest.y);

		mobPosition.Start(serverTick, Position, GetCurrentVelocity(), GetCurrentAccel(), angel, 1.0f);
		_destination = destination;

		// Update facing direction to point at the new destination if valid
		if (!dirToDest.IsNaN())
			Direction = dirToDest;

		// store the action start time (ms)
		if (!IsMoving)
			_actionStartTick = serverTick;

		IsMoving = true;
	}

	private byte[] GetMovePacket(FPOS currentPosition, uint serverTick)
	{
		var vel = GetVelocities();
		SC_MOVE sc_move = new()
		{
			objInstID = Id,
			pos = currentPosition,
			dir = Direction,
			deltaLookAtRadian = 0,
			tick = serverTick,
			moveType = (byte)_moveType,
			turningSpeed = 1,
			accel = 0,
			velocity = GetCurrentVelocity(),
			velocityRatio = 1
		};

		using PacketWriter pw = new();
		pw.Write(sc_move);
		return pw.ToPacket();
	}

	private byte[] GetStopPacket(FPOS position, uint serverTick)
	{
		SC_STOP sc_stop = new()
		{
			objInstID = Id,
			pos = position,
			dir = Direction,
			tick = serverTick,
			stopType = 0
		};

		using PacketWriter pw = new();
		pw.Write(sc_stop);

		return pw.ToPacket();
	}

	private float GetCurrentVelocity()
	{
		var velocities = GetVelocities();
		return _moveType switch
		{
			MOVE_TYPE.MOVE_TYPE_RUN => velocities.run,
			MOVE_TYPE.MOVE_TYPE_WALK => velocities.walk,
			_ => velocities.walk
		};
	}

	private float GetCurrentAccel()
	{
		var velocities = GetVelocities();
		return _moveType switch
		{
			MOVE_TYPE.MOVE_TYPE_RUN => velocities.runAccel,
			MOVE_TYPE.MOVE_TYPE_WALK => velocities.walkAccel,
			_ => velocities.walkAccel
		};
	}

	private VELOCITIES GetVelocities()
	{
		return new VELOCITIES()
		{
			ratio = 1,
			run = (float)_mobInfo.RunVelocity,
			runAccel = (float)_mobInfo.RunAccel,
			walk = (float)_mobInfo.WalkVelocity,
			walkAccel = (float)_mobInfo.WalkAccel,
			backwalk = (float)_mobInfo.BackwalkVelocity,
			backwalkAccel = (float)_mobInfo.BackwalkAccel,
			swim = (float)_mobInfo.FastSwimVelocity,
			swimAccel = (float)_mobInfo.FastSwimAccel,
			backSwim = (float)_mobInfo.BackSwimVelocity,
			backSwimAccel = (float)_mobInfo.BackSwimAccel
		};
	}

	public NpcType GetNpcType()
	{
		return NpcType.Mob;
	}

	public long GetId()
	{
		return Id;
	}

	/// <summary>
	/// Gets the monster type ID from XML definition.
	/// </summary>
	/// <returns>Monster type ID.</returns>
	public int GetEntityTypeId()
	{
		return _mobInfo.TypeID;
	}

	public FPOS GetPosition()
	{
		return GetMobCurrentPosition(LimeServer.GetCurrentTick());
	}

	public FPOS GetDirection()
	{
		return Direction;
	}

	public EntityStatus GetEntityStatus()
	{
		return new EntityStatus()
		{
			Lv = (byte)this.Lv,
			BaseStats = new(),
			BasicStatus = new()
			{
				Hp = this.HP,
				MaxHp = this.MHP,
				Mp = this.MP,
				MaxMp = this.MMP
			},
			MeleeAttack = new()
			{
				WeaponTypeId = (uint)(_mobInfo.Model?.WeaponType ?? 0),
				Atk = (ushort)_mobInfo.BaseMeleeAtk,
				Def = (ushort)_mobInfo.BaseMeleeDefense,
				Hit = (ushort)_mobInfo.BaseMeleeHitRate,
				CritRate = (ushort)_mobInfo.BaseCriticalRate,
				FleeRate = (ushort)_mobInfo.BaseDodge
			},
			SpellAttack = new()
			{
				Atk = (ushort)_mobInfo.BaseSpellAtk,
				Def = (ushort)_mobInfo.BaseSpellDefense,
				Hit = (ushort)_mobInfo.BaseSpellHitRate,
				CritRate = (ushort)_mobInfo.BaseCriticalRate,
				FleeRate = (ushort)_mobInfo.BaseDodge
			}
		};
	}

	public DamageResult UpdateHealth(int healthChange)
	{
		var newHealth = (HP + healthChange);
		HP = (uint)(newHealth <= 0 ? 0 : newHealth);

		if (HP == 0)
			_actionStartTick = LimeServer.GetCurrentTick();

		return new DamageResult()
		{
			TargetKilled = HP == 0,
			ExpReward = _mobInfo.CombatJobEXP + _mobInfo.EXP
		};
	}

	/// <summary>
	/// Adds experience to monster. Monsters don't gain experience.
	/// </summary>
	/// <param name="exp">Experience amount.</param>
	/// <returns>False since monsters don't gain experience.</returns>
	public bool AddExp(ulong exp)
	{
		// Monsters don't gain experience
		return false;
	}

	public List<Item> GetLoot()
	{
		if (Loot == null)
		{
			if (Lootable.Count == 0)
				return new List<Item>();

			Loot = new List<Item>();
			Random rnd = new Random();
			foreach(var lootItem in Lootable)
			{
				var odds = rnd.Next(0, 10000);
				if (lootItem.DropRate * 100 < odds)
					continue;

				Loot.Add(LimeServer.ItemDB.First(item => item.Id == lootItem.Id));
			}
		}

		return Loot;
	}

	void IEntity.Loot(Item item)
	{
		if (Loot != null && Loot.Contains(item))
			Loot.Remove(item);
	}

	private void Spawn()
	{
		Id = LimeServer.GenerateUniqueObjectId();
		_currentState = MobState.Roaming;
		IsMoving = false;
		mobPosition = new PosCalc();
		Position = _originalPosition with { };
		_destination = new FPOS() { x = 0, y = 0, z = 0 };
		Loot = null!;
		MP = (uint)_mobInfo.MP;
		HP = (uint)_mobInfo.HP;
		CurrentTarget = null;
	}
}
