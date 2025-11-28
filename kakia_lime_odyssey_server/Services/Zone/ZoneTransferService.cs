/// <summary>
/// Service for managing zone transfers, teleportation, and portal operations.
/// </summary>
/// <remarks>
/// Handles in-zone warps (SC_WARP), cross-zone transfers (SC_ZONE_TRANSFERING),
/// transfer point (portal) management, and zone spawn points.
/// Uses: SC_WARP, SC_ZONE_TRANSFERING, SC_ENTER_ZONE, SC_ENTER_SIGHT_TRANSFER packets.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;
using System.Collections.Concurrent;

namespace kakia_lime_odyssey_server.Services.Zone;

/// <summary>
/// Zone definition containing spawn points and transfer restrictions.
/// </summary>
public class ZoneDefinition
{
	/// <summary>Zone type ID.</summary>
	public uint ZoneTypeId { get; set; }

	/// <summary>Zone display name.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Default spawn point for the zone.</summary>
	public FPOS DefaultSpawn { get; set; }

	/// <summary>Resurrection spawn point (may differ from default).</summary>
	public FPOS ResurrectionSpawn { get; set; }

	/// <summary>Minimum level to enter this zone.</summary>
	public int MinLevel { get; set; } = 1;

	/// <summary>Maximum level to enter (0 = no limit).</summary>
	public int MaxLevel { get; set; } = 0;

	/// <summary>Whether this is a PvP zone.</summary>
	public bool IsPvPZone { get; set; }

	/// <summary>Whether mounts are allowed in this zone.</summary>
	public bool AllowMounts { get; set; } = true;

	/// <summary>Whether this is a safe zone (no combat).</summary>
	public bool IsSafeZone { get; set; }

	/// <summary>Required quest ID to access (0 = none).</summary>
	public int RequiredQuestId { get; set; }
}

/// <summary>
/// Transfer point (portal/teleporter) definition.
/// </summary>
public class TransferPoint
{
	/// <summary>Unique transfer point ID.</summary>
	public long Id { get; set; }

	/// <summary>Transfer point name.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Zone where this transfer point is located.</summary>
	public uint SourceZoneId { get; set; }

	/// <summary>Position of the transfer point.</summary>
	public FPOS Position { get; set; }

	/// <summary>Target zone ID.</summary>
	public uint TargetZoneId { get; set; }

	/// <summary>Target position in destination zone.</summary>
	public FPOS TargetPosition { get; set; }

	/// <summary>Stop type flag for movement.</summary>
	public byte StopType { get; set; }

	/// <summary>Interaction radius.</summary>
	public float InteractionRadius { get; set; } = 3.0f;

	/// <summary>Whether this is a two-way portal.</summary>
	public bool IsBidirectional { get; set; }

	/// <summary>Cost in Peder to use (0 = free).</summary>
	public int UseCost { get; set; }

	/// <summary>Minimum level to use.</summary>
	public int MinLevel { get; set; } = 1;

	/// <summary>Whether this portal is currently active.</summary>
	public bool IsActive { get; set; } = true;
}

/// <summary>
/// Pending zone transfer state for a player.
/// </summary>
public class PendingTransfer
{
	/// <summary>Player ID.</summary>
	public long PlayerId { get; set; }

	/// <summary>Target zone ID.</summary>
	public uint TargetZoneId { get; set; }

	/// <summary>Target position.</summary>
	public FPOS TargetPosition { get; set; }

	/// <summary>When the transfer was initiated.</summary>
	public DateTime InitiatedAt { get; set; }

	/// <summary>Transfer timeout in seconds.</summary>
	public int TimeoutSeconds { get; set; } = 30;

	/// <summary>Whether transfer is cross-zone.</summary>
	public bool IsCrossZone { get; set; }
}

/// <summary>
/// Result of a transfer operation.
/// </summary>
public class TransferResult
{
	/// <summary>Whether the transfer succeeded.</summary>
	public bool Success { get; set; }

	/// <summary>Error message if failed.</summary>
	public string ErrorMessage { get; set; } = string.Empty;

	/// <summary>Creates a success result.</summary>
	public static TransferResult Succeeded() => new() { Success = true };

	/// <summary>Creates a failure result.</summary>
	public static TransferResult Failed(string message) => new()
	{
		Success = false,
		ErrorMessage = message
	};
}

/// <summary>
/// Service for managing zone transfers and teleportation.
/// </summary>
public class ZoneTransferService
{
	/// <summary>Zone definitions by zone type ID.</summary>
	private readonly Dictionary<uint, ZoneDefinition> _zones = new();

	/// <summary>Transfer points by ID.</summary>
	private readonly Dictionary<long, TransferPoint> _transferPoints = new();

	/// <summary>Transfer points indexed by source zone.</summary>
	private readonly Dictionary<uint, List<TransferPoint>> _transferPointsByZone = new();

	/// <summary>Pending transfers by player ID.</summary>
	private readonly ConcurrentDictionary<long, PendingTransfer> _pendingTransfers = new();

	/// <summary>Next transfer point ID.</summary>
	private long _nextTransferPointId = 1;

	/// <summary>
	/// Initializes the zone transfer service with default zones.
	/// </summary>
	public ZoneTransferService()
	{
		InitializeDefaultZones();
		InitializeDefaultTransferPoints();
	}

	/// <summary>
	/// Initializes default zone definitions.
	/// </summary>
	/// <remarks>
	/// Coordinates sourced from MapTargetInfo.xml:
	/// - worldIndex=1 = Muris Village area (worldMapTypeId=0)
	/// - Guild positions: x=-545 to -575, y=-3858 to -3867, z=~495
	/// </remarks>
	private void InitializeDefaultZones()
	{
		// Starting zone - Muris Village (from MapTargetInfo.xml worldIndex=1)
		// Combat Guild NPC position: posX="-545" posY="-3867" posZ="495.80"
		RegisterZone(new ZoneDefinition
		{
			ZoneTypeId = 1,
			Name = "Muris Village",
			DefaultSpawn = new FPOS { x = -545.0f, y = -3867.0f, z = 495.80f },
			ResurrectionSpawn = new FPOS { x = -575.0f, y = -3858.0f, z = 495.80f },
			MinLevel = 1,
			IsSafeZone = true,
			AllowMounts = true
		});

		// Field zone - Near village outskirts
		// From MapTargetInfo: various monster spawn areas around -1500 to -3000 range
		RegisterZone(new ZoneDefinition
		{
			ZoneTypeId = 2,
			Name = "Muris Fields",
			DefaultSpawn = new FPOS { x = -1560.0f, y = -3294.0f, z = 273.54f },
			ResurrectionSpawn = new FPOS { x = -545.0f, y = -3867.0f, z = 495.80f },
			MinLevel = 1,
			AllowMounts = true
		});

		// Dungeon zone
		RegisterZone(new ZoneDefinition
		{
			ZoneTypeId = 10,
			Name = "Ancient Ruins",
			DefaultSpawn = new FPOS { x = 500.0f, y = 0.0f, z = 500.0f },
			ResurrectionSpawn = new FPOS { x = 500.0f, y = 0.0f, z = 500.0f },
			MinLevel = 15,
			AllowMounts = false
		});

		// PvP zone
		RegisterZone(new ZoneDefinition
		{
			ZoneTypeId = 100,
			Name = "Arena",
			DefaultSpawn = new FPOS { x = 100.0f, y = 0.0f, z = 100.0f },
			ResurrectionSpawn = new FPOS { x = 100.0f, y = 0.0f, z = 100.0f },
			MinLevel = 20,
			IsPvPZone = true,
			AllowMounts = false
		});

		Logger.Log($"[ZONE] Initialized {_zones.Count} zone definitions", LogLevel.Information);
	}

	/// <summary>
	/// Initializes default transfer points (portals).
	/// </summary>
	private void InitializeDefaultTransferPoints()
	{
		// Village to Fields portal
		RegisterTransferPoint(new TransferPoint
		{
			Name = "To Muris Fields",
			SourceZoneId = 1,
			Position = new FPOS { x = -700.0f, y = -3586.0f, z = 520.0f },
			TargetZoneId = 2,
			TargetPosition = new FPOS { x = -1560.0f, y = -3294.0f, z = 273.54f },
			IsBidirectional = true
		});

		// Fields to Village portal
		RegisterTransferPoint(new TransferPoint
		{
			Name = "To Muris Village",
			SourceZoneId = 2,
			Position = new FPOS { x = -1560.0f, y = -3294.0f, z = 280.0f },
			TargetZoneId = 1,
			TargetPosition = new FPOS { x = -545.0f, y = -3867.0f, z = 495.80f },
			IsBidirectional = true
		});

		// Fields to Dungeon portal
		RegisterTransferPoint(new TransferPoint
		{
			Name = "Ancient Ruins Entrance",
			SourceZoneId = 2,
			Position = new FPOS { x = 6000.0f, y = 100.0f, z = 6000.0f },
			TargetZoneId = 10,
			TargetPosition = new FPOS { x = 500.0f, y = 0.0f, z = 500.0f },
			MinLevel = 15
		});

		Logger.Log($"[ZONE] Initialized {_transferPoints.Count} transfer points", LogLevel.Information);
	}

	/// <summary>
	/// Registers a zone definition.
	/// </summary>
	public void RegisterZone(ZoneDefinition zone)
	{
		_zones[zone.ZoneTypeId] = zone;
	}

	/// <summary>
	/// Gets a zone definition by ID.
	/// </summary>
	public ZoneDefinition? GetZone(uint zoneTypeId)
	{
		return _zones.TryGetValue(zoneTypeId, out var zone) ? zone : null;
	}

	/// <summary>
	/// Registers a transfer point (portal).
	/// </summary>
	public void RegisterTransferPoint(TransferPoint point)
	{
		point.Id = _nextTransferPointId++;
		_transferPoints[point.Id] = point;

		if (!_transferPointsByZone.TryGetValue(point.SourceZoneId, out var list))
		{
			list = new List<TransferPoint>();
			_transferPointsByZone[point.SourceZoneId] = list;
		}
		list.Add(point);
	}

	/// <summary>
	/// Gets transfer points in a zone.
	/// </summary>
	public List<TransferPoint> GetTransferPointsInZone(uint zoneId)
	{
		return _transferPointsByZone.TryGetValue(zoneId, out var points)
			? points.Where(p => p.IsActive).ToList()
			: new List<TransferPoint>();
	}

	/// <summary>
	/// Gets the default spawn point for a zone.
	/// </summary>
	public FPOS GetSpawnPoint(uint zoneTypeId)
	{
		if (_zones.TryGetValue(zoneTypeId, out var zone))
		{
			return zone.DefaultSpawn;
		}
		// Default fallback spawn
		// Fallback to Muris Village spawn (from MapTargetInfo.xml)
		return new FPOS { x = -545.0f, y = -3867.0f, z = 495.80f };
	}

	/// <summary>
	/// Gets the resurrection spawn point for a zone.
	/// </summary>
	public FPOS GetResurrectionSpawn(uint zoneTypeId)
	{
		if (_zones.TryGetValue(zoneTypeId, out var zone))
		{
			return zone.ResurrectionSpawn;
		}
		return GetSpawnPoint(zoneTypeId);
	}

	/// <summary>
	/// Validates if a player can enter a zone.
	/// </summary>
	/// <param name="pc">Player client.</param>
	/// <param name="zoneTypeId">Target zone ID.</param>
	/// <returns>Validation result.</returns>
	public (bool CanEnter, string Reason) CanEnterZone(PlayerClient pc, uint zoneTypeId)
	{
		var zone = GetZone(zoneTypeId);
		if (zone == null)
		{
			return (false, "Zone does not exist.");
		}

		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			return (false, "No character loaded.");
		}

		int playerLevel = character.status.combatJob.lv;

		// Check minimum level
		if (playerLevel < zone.MinLevel)
		{
			return (false, $"Minimum level {zone.MinLevel} required.");
		}

		// Check maximum level
		if (zone.MaxLevel > 0 && playerLevel > zone.MaxLevel)
		{
			return (false, $"Maximum level {zone.MaxLevel} for this zone.");
		}

		// Check quest requirement
		if (zone.RequiredQuestId > 0)
		{
			if (!LimeServer.QuestService.HasCompletedQuest(pc, (uint)zone.RequiredQuestId))
			{
				return (false, $"You must complete a required quest before entering this zone.");
			}
		}

		return (true, string.Empty);
	}

	/// <summary>
	/// Validates if a player can use a transfer point.
	/// </summary>
	public (bool CanUse, string Reason) CanUseTransferPoint(PlayerClient pc, TransferPoint point)
	{
		if (!point.IsActive)
		{
			return (false, "This portal is not active.");
		}

		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			return (false, "No character loaded.");
		}

		int playerLevel = character.status.combatJob.lv;

		// Check level requirement
		if (playerLevel < point.MinLevel)
		{
			return (false, $"Minimum level {point.MinLevel} required to use this portal.");
		}

		// Check cost
		if (point.UseCost > 0)
		{
			var inventory = pc.GetInventory();
			if (inventory.WalletPeder < point.UseCost)
			{
				return (false, $"Not enough Peder. Need {point.UseCost}.");
			}
		}

		// Check target zone entry
		var (canEnter, reason) = CanEnterZone(pc, point.TargetZoneId);
		if (!canEnter)
		{
			return (false, reason);
		}

		return (true, string.Empty);
	}

	/// <summary>
	/// Performs an in-zone warp (teleport within same zone).
	/// </summary>
	/// <param name="pc">Player client.</param>
	/// <param name="targetPos">Target position.</param>
	/// <param name="targetDir">Target direction (optional).</param>
	/// <returns>Transfer result.</returns>
	public TransferResult WarpInZone(PlayerClient pc, FPOS targetPos, FPOS? targetDir = null)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Update player position
		pc.UpdatePosition(targetPos);

		// Prepare direction
		FPOS dir = targetDir ?? pc.GetDirection();

		// Send warp packet
		SC_WARP warp = new()
		{
			objInstID = pc.GetObjInstID(),
			pos = targetPos,
			dir = dir
		};

		using PacketWriter pw = new();
		pw.Write(warp);
		var data = pw.ToPacket();

		// Send to self and broadcast
		pc.Send(data, default).Wait();
		pc.SendGlobalPacket(data, default).Wait();

		Logger.Log($"[ZONE] {playerName} warped to ({targetPos.x:F1}, {targetPos.y:F1}, {targetPos.z:F1})", LogLevel.Debug);

		return TransferResult.Succeeded();
	}

	/// <summary>
	/// Initiates a cross-zone transfer.
	/// </summary>
	/// <param name="pc">Player client.</param>
	/// <param name="targetZoneId">Target zone type ID.</param>
	/// <param name="targetPos">Target position in destination zone.</param>
	/// <returns>Transfer result.</returns>
	public TransferResult TransferToZone(PlayerClient pc, uint targetZoneId, FPOS targetPos)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		long playerId = pc.GetId();

		// Validate zone entry
		var (canEnter, reason) = CanEnterZone(pc, targetZoneId);
		if (!canEnter)
		{
			Logger.Log($"[ZONE] {playerName} transfer denied to zone {targetZoneId}: {reason}", LogLevel.Warning);
			return TransferResult.Failed(reason);
		}

		// Check for existing pending transfer
		if (_pendingTransfers.ContainsKey(playerId))
		{
			return TransferResult.Failed("Transfer already in progress.");
		}

		// Register pending transfer
		var pending = new PendingTransfer
		{
			PlayerId = playerId,
			TargetZoneId = targetZoneId,
			TargetPosition = targetPos,
			InitiatedAt = DateTime.UtcNow,
			IsCrossZone = true
		};
		_pendingTransfers[playerId] = pending;

		// Get target zone area instance ID (for now, use zone type ID)
		uint areaInstId = targetZoneId;

		// Send zone transfering packet
		SC_ZONE_TRANSFERING transferPacket = new()
		{
			areaInstID = areaInstId,
			pos = targetPos
		};

		using PacketWriter pw = new();
		pw.Write(transferPacket);
		pc.Send(pw.ToPacket(), default).Wait();

		// Update player zone
		uint oldZone = pc.GetZone();
		pc.SetZone(targetZoneId);
		pc.UpdatePosition(targetPos);

		// Notify players in old zone that this player left
		BroadcastPlayerLeftZone(pc, oldZone);

		// Send enter zone packet
		SC_ENTER_ZONE enterPacket = new()
		{
			objInstID = pc.GetObjInstID()
		};

		using PacketWriter enterPw = new();
		enterPw.Write(enterPacket);
		pc.Send(enterPw.ToPacket(), default).Wait();

		// Clear pending transfer
		_pendingTransfers.TryRemove(playerId, out _);

		var zoneDef = GetZone(targetZoneId);
		string zoneName = zoneDef?.Name ?? $"Zone {targetZoneId}";

		Logger.Log($"[ZONE] {playerName} transferred from zone {oldZone} to {zoneName} ({targetZoneId})", LogLevel.Information);

		return TransferResult.Succeeded();
	}

	/// <summary>
	/// Uses a transfer point (portal) to travel to another zone.
	/// </summary>
	/// <param name="pc">Player client.</param>
	/// <param name="transferPointId">Transfer point ID.</param>
	/// <returns>Transfer result.</returns>
	public TransferResult UseTransferPoint(PlayerClient pc, long transferPointId)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_transferPoints.TryGetValue(transferPointId, out var point))
		{
			return TransferResult.Failed("Transfer point not found.");
		}

		// Validate player can use the transfer point
		var (canUse, reason) = CanUseTransferPoint(pc, point);
		if (!canUse)
		{
			Logger.Log($"[ZONE] {playerName} cannot use transfer point: {reason}", LogLevel.Warning);
			return TransferResult.Failed(reason);
		}

		// Check player is in source zone
		if (pc.GetZone() != point.SourceZoneId)
		{
			return TransferResult.Failed("Not in the correct zone.");
		}

		// Check player is near the transfer point
		var playerPos = pc.GetPosition();
		float distance = CalculateDistance(playerPos, point.Position);
		if (distance > point.InteractionRadius * 2)
		{
			return TransferResult.Failed("Too far from the portal.");
		}

		// Deduct cost if any
		if (point.UseCost > 0)
		{
			var inventory = pc.GetInventory();
			inventory.WalletPeder -= point.UseCost;
			Logger.Log($"[ZONE] {playerName} paid {point.UseCost} Peder for portal", LogLevel.Debug);
		}

		// Perform the transfer
		if (point.SourceZoneId == point.TargetZoneId)
		{
			// Same zone warp
			return WarpInZone(pc, point.TargetPosition);
		}
		else
		{
			// Cross-zone transfer
			return TransferToZone(pc, point.TargetZoneId, point.TargetPosition);
		}
	}

	/// <summary>
	/// Teleports a player to a specific zone's spawn point.
	/// </summary>
	/// <param name="pc">Player client.</param>
	/// <param name="zoneTypeId">Target zone ID.</param>
	/// <returns>Transfer result.</returns>
	public TransferResult TeleportToZoneSpawn(PlayerClient pc, uint zoneTypeId)
	{
		var spawnPoint = GetSpawnPoint(zoneTypeId);

		if (pc.GetZone() == zoneTypeId)
		{
			return WarpInZone(pc, spawnPoint);
		}
		else
		{
			return TransferToZone(pc, zoneTypeId, spawnPoint);
		}
	}

	/// <summary>
	/// Handles the CS_FINISH_WARP acknowledgment.
	/// </summary>
	/// <param name="pc">Player client.</param>
	public void HandleFinishWarp(PlayerClient pc)
	{
		long playerId = pc.GetId();

		// Clear any pending transfer
		if (_pendingTransfers.TryRemove(playerId, out var pending))
		{
			string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
			var elapsed = DateTime.UtcNow - pending.InitiatedAt;
			Logger.Log($"[ZONE] {playerName} completed warp (took {elapsed.TotalMilliseconds:F0}ms)", LogLevel.Debug);
		}

		// Request presence update (load nearby entities)
		pc.RequestPresence(default).Wait();
	}

	/// <summary>
	/// Sends transfer point (portal) data to a player entering a zone.
	/// </summary>
	/// <param name="pc">Player client.</param>
	public void SendTransferPointsToPlayer(PlayerClient pc)
	{
		uint zoneId = pc.GetZone();
		var points = GetTransferPointsInZone(zoneId);

		foreach (var point in points)
		{
			SendTransferPointEnterSight(pc, point);
		}
	}

	/// <summary>
	/// Sends SC_ENTER_SIGHT_TRANSFER for a transfer point.
	/// </summary>
	private void SendTransferPointEnterSight(PlayerClient pc, TransferPoint point)
	{
		SC_ENTER_SIGHT_TRANSFER packet = new()
		{
			objInstID = point.Id,
			name = GetNameBytes(point.Name, 31),
			pos = point.Position,
			targetZoneTypeID = point.TargetZoneId,
			targetPos = point.TargetPosition,
			stopType = point.StopType
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Broadcasts that a player left a zone.
	/// </summary>
	private void BroadcastPlayerLeftZone(PlayerClient pc, uint oldZone)
	{
		// Send leave sight to all players in old zone using SC_LEAVE_ZONEOBJ base packet
		SC_LEAVE_ZONEOBJ leavePacket = new()
		{
			objInstID = pc.GetObjInstID()
		};

		using PacketWriter pw = new();
		pw.Write(leavePacket);
		var data = pw.ToPacket();

		// This would be broadcast via LimeServer to all players in oldZone
		// For now, broadcast globally - server will filter by zone
		pc.SendGlobalPacket(data, default).Wait();
	}

	/// <summary>
	/// Gets a transfer point near a position.
	/// </summary>
	/// <param name="zoneId">Zone ID.</param>
	/// <param name="pos">Position.</param>
	/// <param name="radius">Search radius.</param>
	/// <returns>Nearest transfer point or null.</returns>
	public TransferPoint? GetTransferPointNear(uint zoneId, FPOS pos, float radius = 5.0f)
	{
		var points = GetTransferPointsInZone(zoneId);

		TransferPoint? nearest = null;
		float nearestDist = float.MaxValue;

		foreach (var point in points)
		{
			float dist = CalculateDistance(pos, point.Position);
			if (dist <= radius && dist < nearestDist)
			{
				nearest = point;
				nearestDist = dist;
			}
		}

		return nearest;
	}

	/// <summary>
	/// Checks if mounts are allowed in a zone.
	/// </summary>
	public bool AreMountsAllowed(uint zoneTypeId)
	{
		var zone = GetZone(zoneTypeId);
		return zone?.AllowMounts ?? true;
	}

	/// <summary>
	/// Checks if a zone is a safe zone.
	/// </summary>
	public bool IsSafeZone(uint zoneTypeId)
	{
		var zone = GetZone(zoneTypeId);
		return zone?.IsSafeZone ?? false;
	}

	/// <summary>
	/// Checks if a zone is a PvP zone.
	/// </summary>
	public bool IsPvPZone(uint zoneTypeId)
	{
		var zone = GetZone(zoneTypeId);
		return zone?.IsPvPZone ?? false;
	}

	/// <summary>
	/// Cleans up expired pending transfers.
	/// </summary>
	public void CleanupExpiredTransfers()
	{
		var now = DateTime.UtcNow;
		var expired = _pendingTransfers
			.Where(kvp => (now - kvp.Value.InitiatedAt).TotalSeconds > kvp.Value.TimeoutSeconds)
			.Select(kvp => kvp.Key)
			.ToList();

		foreach (var playerId in expired)
		{
			if (_pendingTransfers.TryRemove(playerId, out var pending))
			{
				Logger.Log($"[ZONE] Cleaned up expired transfer for player {playerId}", LogLevel.Debug);
			}
		}
	}

	/// <summary>
	/// Gets zone info for display.
	/// </summary>
	public (string Name, int MinLevel, int MaxLevel, bool IsPvP, bool IsSafe) GetZoneInfo(uint zoneTypeId)
	{
		var zone = GetZone(zoneTypeId);
		if (zone == null)
		{
			return ($"Unknown Zone {zoneTypeId}", 1, 0, false, false);
		}
		return (zone.Name, zone.MinLevel, zone.MaxLevel, zone.IsPvPZone, zone.IsSafeZone);
	}

	/// <summary>
	/// Calculates distance between two positions.
	/// </summary>
	private static float CalculateDistance(FPOS a, FPOS b)
	{
		float dx = a.x - b.x;
		float dy = a.y - b.y;
		float dz = a.z - b.z;
		return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
	}

	/// <summary>
	/// Converts a string to a fixed-size byte array.
	/// </summary>
	private static byte[] GetNameBytes(string name, int size)
	{
		byte[] result = new byte[size];
		byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(name);
		int copyLen = Math.Min(nameBytes.Length, size - 1);
		Array.Copy(nameBytes, result, copyLen);
		return result;
	}
}
