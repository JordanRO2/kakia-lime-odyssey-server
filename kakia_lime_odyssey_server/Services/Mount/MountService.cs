/// <summary>
/// Service for managing mount/pet riding operations.
/// </summary>
/// <remarks>
/// Handles mounting, dismounting, mount inventory, speed bonuses,
/// mount stamina, and mount state broadcasting.
/// Uses: SC_RIDE_PC, SC_UNRIDE_PC packets for state updates.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;
using System.Collections.Concurrent;
using ItemModel = kakia_lime_odyssey_server.Models.Item;

namespace kakia_lime_odyssey_server.Services.Mount;

/// <summary>
/// Mount state for a player.
/// </summary>
public class MountState
{
	/// <summary>Whether the player is currently mounted.</summary>
	public bool IsMounted { get; set; }

	/// <summary>The mount item type ID.</summary>
	public int MountItemTypeId { get; set; }

	/// <summary>Mount speed bonus percentage.</summary>
	public float SpeedBonus { get; set; }

	/// <summary>Current mount stamina.</summary>
	public int CurrentStamina { get; set; }

	/// <summary>Maximum mount stamina.</summary>
	public int MaxStamina { get; set; }

	/// <summary>When the player mounted.</summary>
	public DateTime MountedAt { get; set; }

	/// <summary>When stamina was last decreased.</summary>
	public DateTime LastStaminaUpdate { get; set; }
}

/// <summary>
/// Mount definition from game data.
/// </summary>
public class MountDefinition
{
	/// <summary>Item type ID of the mount.</summary>
	public int ItemTypeId { get; set; }

	/// <summary>Display name.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Speed bonus percentage (e.g., 50 = 50% faster).</summary>
	public float SpeedBonus { get; set; } = 30f;

	/// <summary>Maximum stamina.</summary>
	public int MaxStamina { get; set; } = 100;

	/// <summary>Stamina consumption per second while moving.</summary>
	public float StaminaConsumptionRate { get; set; } = 1f;

	/// <summary>Stamina regeneration per second while stationary.</summary>
	public float StaminaRegenRate { get; set; } = 2f;

	/// <summary>Required player level to use this mount.</summary>
	public int RequiredLevel { get; set; } = 1;

	/// <summary>Whether this mount can be used in combat zones.</summary>
	public bool AllowedInCombat { get; set; } = false;
}

/// <summary>
/// Service for managing player mounts.
/// </summary>
public class MountService
{
	/// <summary>Item StuffType values that represent mounts.</summary>
	private static readonly HashSet<int> MountStuffTypes = new() { 70, 71, 72 };

	/// <summary>Stamina update interval in seconds.</summary>
	private const int StaminaUpdateIntervalSeconds = 5;

	/// <summary>Current mount states by player ID.</summary>
	private readonly ConcurrentDictionary<long, MountState> _mountStates = new();

	/// <summary>Mount definitions by item type ID.</summary>
	private readonly Dictionary<int, MountDefinition> _mountDefinitions = new();

	/// <summary>
	/// Initializes the mount service with default mount definitions.
	/// </summary>
	public MountService()
	{
		InitializeDefaultMounts();
	}

	/// <summary>
	/// Initializes default mount definitions.
	/// </summary>
	private void InitializeDefaultMounts()
	{
		// Basic mounts - these would normally come from XML data
		_mountDefinitions[10001] = new MountDefinition
		{
			ItemTypeId = 10001,
			Name = "Basic Horse",
			SpeedBonus = 30f,
			MaxStamina = 100,
			RequiredLevel = 10
		};

		_mountDefinitions[10002] = new MountDefinition
		{
			ItemTypeId = 10002,
			Name = "War Horse",
			SpeedBonus = 50f,
			MaxStamina = 150,
			RequiredLevel = 30,
			AllowedInCombat = true
		};

		_mountDefinitions[10003] = new MountDefinition
		{
			ItemTypeId = 10003,
			Name = "Swift Horse",
			SpeedBonus = 70f,
			MaxStamina = 80,
			RequiredLevel = 50
		};

		Logger.Log($"[MOUNT] Initialized {_mountDefinitions.Count} mount definitions", LogLevel.Information);
	}

	/// <summary>
	/// Registers a mount definition.
	/// </summary>
	public void RegisterMount(MountDefinition mount)
	{
		_mountDefinitions[mount.ItemTypeId] = mount;
	}

	/// <summary>
	/// Gets a mount definition by item type ID.
	/// </summary>
	public MountDefinition? GetMountDefinition(int itemTypeId)
	{
		return _mountDefinitions.TryGetValue(itemTypeId, out var def) ? def : null;
	}

	/// <summary>
	/// Checks if an item is a mount.
	/// </summary>
	public bool IsMount(ItemModel item)
	{
		return MountStuffTypes.Contains(item.StuffType) ||
			   _mountDefinitions.ContainsKey(item.Id);
	}

	/// <summary>
	/// Gets the current mount state for a player.
	/// </summary>
	public MountState? GetMountState(long playerId)
	{
		return _mountStates.TryGetValue(playerId, out var state) ? state : null;
	}

	/// <summary>
	/// Checks if a player is currently mounted.
	/// </summary>
	public bool IsMounted(long playerId)
	{
		return _mountStates.TryGetValue(playerId, out var state) && state.IsMounted;
	}

	/// <summary>
	/// Attempts to mount a player on a mount item.
	/// </summary>
	/// <param name="pc">The player client.</param>
	/// <param name="mountItemTypeId">The mount item type ID.</param>
	/// <returns>True if mounting succeeded.</returns>
	public bool Mount(PlayerClient pc, int mountItemTypeId)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Check if already mounted
		if (IsMounted(playerId))
		{
			Logger.Log($"[MOUNT] {playerName} already mounted", LogLevel.Warning);
			return false;
		}

		// Get mount definition
		var mountDef = GetMountDefinition(mountItemTypeId);
		float speedBonus = mountDef?.SpeedBonus ?? 30f;
		int maxStamina = mountDef?.MaxStamina ?? 100;

		// Check level requirement
		if (mountDef != null)
		{
			var character = pc.GetCurrentCharacter();
			if (character != null && character.status.combatJob.lv < mountDef.RequiredLevel)
			{
				Logger.Log($"[MOUNT] {playerName} level too low for mount (need {mountDef.RequiredLevel})", LogLevel.Warning);
				return false;
			}
		}

		// Create mount state
		var state = new MountState
		{
			IsMounted = true,
			MountItemTypeId = mountItemTypeId,
			SpeedBonus = speedBonus,
			CurrentStamina = maxStamina,
			MaxStamina = maxStamina,
			MountedAt = DateTime.UtcNow,
			LastStaminaUpdate = DateTime.UtcNow
		};

		_mountStates[playerId] = state;

		// Send mount packet to player and broadcast to others
		SendMountPacket(pc, mountItemTypeId);

		string mountName = mountDef?.Name ?? $"Mount {mountItemTypeId}";
		Logger.Log($"[MOUNT] {playerName} mounted {mountName} (+{speedBonus}% speed)", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Dismounts a player.
	/// </summary>
	/// <param name="pc">The player client.</param>
	/// <returns>True if dismounting succeeded.</returns>
	public bool Dismount(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_mountStates.TryRemove(playerId, out var state))
		{
			Logger.Log($"[MOUNT] {playerName} not mounted", LogLevel.Warning);
			return false;
		}

		// Send dismount packet
		SendDismountPacket(pc);

		var mountDef = GetMountDefinition(state.MountItemTypeId);
		string mountName = mountDef?.Name ?? $"Mount {state.MountItemTypeId}";
		var rideDuration = DateTime.UtcNow - state.MountedAt;

		Logger.Log($"[MOUNT] {playerName} dismounted {mountName} (rode for {rideDuration.TotalMinutes:F1} minutes)", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Forces dismount (e.g., when entering combat zone or taking damage).
	/// </summary>
	public void ForceDismount(PlayerClient pc, string reason)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_mountStates.TryRemove(playerId, out _))
		{
			SendDismountPacket(pc);
			Logger.Log($"[MOUNT] {playerName} force dismounted: {reason}", LogLevel.Information);
		}
	}

	/// <summary>
	/// Gets the current speed bonus for a player (from mount).
	/// </summary>
	/// <returns>Speed bonus as a multiplier (e.g., 1.5 = 50% faster).</returns>
	public float GetSpeedMultiplier(long playerId)
	{
		if (_mountStates.TryGetValue(playerId, out var state) && state.IsMounted)
		{
			return 1.0f + (state.SpeedBonus / 100f);
		}
		return 1.0f;
	}

	/// <summary>
	/// Updates mount stamina for moving players.
	/// Called from server tick.
	/// </summary>
	public void UpdateMountStamina(PlayerClient pc, bool isMoving)
	{
		long playerId = pc.GetId();

		if (!_mountStates.TryGetValue(playerId, out var state) || !state.IsMounted)
		{
			return;
		}

		var now = DateTime.UtcNow;
		var elapsed = (now - state.LastStaminaUpdate).TotalSeconds;

		if (elapsed < 1) return;

		var mountDef = GetMountDefinition(state.MountItemTypeId);
		float consumeRate = mountDef?.StaminaConsumptionRate ?? 1f;
		float regenRate = mountDef?.StaminaRegenRate ?? 2f;

		if (isMoving)
		{
			// Consume stamina while moving
			state.CurrentStamina = Math.Max(0, state.CurrentStamina - (int)(elapsed * consumeRate));

			// Auto-dismount when stamina depleted
			if (state.CurrentStamina <= 0)
			{
				ForceDismount(pc, "mount exhausted");
				return;
			}
		}
		else
		{
			// Regenerate stamina while stationary
			state.CurrentStamina = Math.Min(state.MaxStamina, state.CurrentStamina + (int)(elapsed * regenRate));
		}

		state.LastStaminaUpdate = now;
	}

	/// <summary>
	/// Uses a mount item from inventory.
	/// </summary>
	public bool UseMountItem(PlayerClient pc, int inventorySlot)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var inventory = pc.GetInventory();

		var item = inventory.AtSlot(inventorySlot) as ItemModel;
		if (item == null)
		{
			Logger.Log($"[MOUNT] {playerName} use mount failed: No item at slot {inventorySlot}", LogLevel.Warning);
			return false;
		}

		if (!IsMount(item))
		{
			Logger.Log($"[MOUNT] {playerName} use mount failed: Item {item.Name} is not a mount", LogLevel.Warning);
			return false;
		}

		return Mount(pc, item.Id);
	}

	/// <summary>
	/// Sends SC_RIDE_PC packet.
	/// </summary>
	private void SendMountPacket(PlayerClient pc, int mountItemTypeId)
	{
		var packet = new SC_RIDE_PC
		{
			objInstID = pc.GetObjInstID(),
			rideItemTypeID = mountItemTypeId
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		var data = pw.ToPacket();

		// Send to self
		pc.Send(data, default).Wait();

		// Broadcast to others in sight
		pc.SendGlobalPacket(data, default).Wait();
	}

	/// <summary>
	/// Sends SC_UNRIDE_PC packet.
	/// </summary>
	private void SendDismountPacket(PlayerClient pc)
	{
		var packet = new SC_UNRIDE_PC
		{
			objInstID = pc.GetObjInstID()
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		var data = pw.ToPacket();

		// Send to self
		pc.Send(data, default).Wait();

		// Broadcast to others in sight
		pc.SendGlobalPacket(data, default).Wait();
	}

	/// <summary>
	/// Gets mount status information for UI.
	/// </summary>
	public (bool IsMounted, int ItemTypeId, int Stamina, int MaxStamina, float SpeedBonus) GetMountInfo(long playerId)
	{
		if (_mountStates.TryGetValue(playerId, out var state) && state.IsMounted)
		{
			return (true, state.MountItemTypeId, state.CurrentStamina, state.MaxStamina, state.SpeedBonus);
		}
		return (false, 0, 0, 0, 0f);
	}

	/// <summary>
	/// Cleans up mount state for a player (on disconnect).
	/// </summary>
	public void CleanupPlayerMount(long playerId)
	{
		_mountStates.TryRemove(playerId, out _);
	}

	/// <summary>
	/// Checks if player can mount in their current location.
	/// </summary>
	public (bool CanMount, string Reason) CanMountHere(PlayerClient pc, int mountItemTypeId)
	{
		// Check if in combat
		// (This would need integration with combat service)

		var mountDef = GetMountDefinition(mountItemTypeId);
		if (mountDef != null && !mountDef.AllowedInCombat)
		{
			// Would check combat state here
		}

		// Check zone restrictions
		// (This would need integration with zone service)

		return (true, string.Empty);
	}
}
