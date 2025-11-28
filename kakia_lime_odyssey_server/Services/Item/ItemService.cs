/// <summary>
/// Service for managing item usage and consumption.
/// </summary>
/// <remarks>
/// Handles using consumable items on self, objects, positions, and other inventory slots.
/// Uses: PlayerInventory for item access, ItemDB for item definitions
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Item;

/// <summary>
/// Service for managing item usage and consumption.
/// </summary>
public class ItemService
{
	/// <summary>
	/// Uses an item on self (potions, buffs, etc).
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot containing the item</param>
	public void UseItemOnSelf(PlayerClient pc, int slot)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[ITEM] UseItemOnSelf failed: No character loaded", LogLevel.Warning);
			SendUseItemResult(pc, 0, false);
			return;
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[ITEM] UseItemOnSelf failed: No item at slot {slot}", LogLevel.Warning);
			SendUseItemResult(pc, 0, false);
			return;
		}

		// Check if item is usable (consumables type)
		if (item.Type != (int)Models.ItemType.Consumables && item.SkillId <= 0)
		{
			Logger.Log($"[ITEM] UseItemOnSelf failed: Item {item.Name} is not usable", LogLevel.Warning);
			SendUseItemResult(pc, (long)item.GetId(), false);
			return;
		}

		string playerName = character.appearance.name;
		Logger.Log($"[ITEM] {playerName} using item {item.Name} (ID: {item.Id}) on self", LogLevel.Debug);

		// Apply item effects
		bool success = ApplyItemEffect(pc, item);

		if (success)
		{
			// Consume the item
			ConsumeItem(pc, slot, item);
		}

		SendUseItemResult(pc, (long)item.GetId(), success);
	}

	/// <summary>
	/// Uses an item on a target object (NPC, pet, party member, etc).
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot containing the item</param>
	/// <param name="targetInstId">Target object instance ID</param>
	public void UseItemOnObject(PlayerClient pc, int slot, long targetInstId)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[ITEM] UseItemOnObject failed: No character loaded", LogLevel.Warning);
			SendUseItemObjResult(pc, 0, targetInstId, false);
			return;
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[ITEM] UseItemOnObject failed: No item at slot {slot}", LogLevel.Warning);
			SendUseItemObjResult(pc, 0, targetInstId, false);
			return;
		}

		string playerName = character.appearance.name;

		// Validate item can be used on target
		if (!CanUseItemOnTarget(item))
		{
			Logger.Log($"[ITEM] {playerName} cannot use {item.Name} on targets", LogLevel.Debug);
			SendUseItemObjResult(pc, (long)item.GetId(), targetInstId, false);
			return;
		}

		// Check if target is valid and in range
		if (!LimeServer.TryGetEntity(targetInstId, out var targetEntity) || targetEntity == null)
		{
			Logger.Log($"[ITEM] UseItemOnObject failed: Target {targetInstId} not found", LogLevel.Warning);
			SendUseItemObjResult(pc, (long)item.GetId(), targetInstId, false);
			return;
		}

		Logger.Log($"[ITEM] {playerName} using item {item.Name} on target {targetInstId}", LogLevel.Debug);

		// Apply effects to target
		bool success = ApplyItemEffectOnTarget(pc, item, targetEntity);

		if (success)
		{
			ConsumeItem(pc, slot, item);
		}

		SendUseItemObjResult(pc, (long)item.GetId(), targetInstId, success);
	}

	/// <summary>
	/// Checks if an item can be used on another entity.
	/// </summary>
	private bool CanUseItemOnTarget(Models.Item item)
	{
		// Items that can target others:
		// - Consumables with userType that allows target use
		// - Items with SkillId that targets others
		// - Pet food items
		if (item.Type == (int)Models.ItemType.Consumables)
		{
			return true; // Consumables can potentially be used on targets
		}

		if (item.SkillId > 0)
		{
			return true; // Items that trigger skills can target
		}

		return item.Inherits != null && item.Inherits.Count > 0;
	}

	/// <summary>
	/// Applies item effects to a target entity.
	/// </summary>
	private bool ApplyItemEffectOnTarget(PlayerClient pc, Models.Item item, Interfaces.IEntity target)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Check if item has inherits/effects
		if (item.Inherits == null || item.Inherits.Count == 0)
		{
			Logger.Log($"[ITEM] Item {item.Name} has no effects for target use", LogLevel.Debug);
			return false;
		}

		// Apply each inherit effect to the target
		foreach (var inherit in item.Inherits)
		{
			ApplyInheritEffectOnTarget(pc, target, inherit);
		}

		Logger.Log($"[ITEM] {playerName} applied {item.Name} effects to target {target.GetId()}", LogLevel.Debug);
		return true;
	}

	/// <summary>
	/// Applies a single inherit effect to a target entity.
	/// </summary>
	private void ApplyInheritEffectOnTarget(PlayerClient pc, Interfaces.IEntity target, Models.Inherit inherit)
	{
		var targetStatus = target.GetEntityStatus();

		switch (inherit.typeID)
		{
			case 1: // HP restore - use positive value for healing
				target.UpdateHealth(inherit.val);
				Logger.Log($"[ITEM] Healed target for {inherit.val} HP", LogLevel.Debug);
				break;

			case 2: // MP restore - MP not directly supported via UpdateHealth, log for now
				Logger.Log($"[ITEM] MP restore not yet implemented for target (would restore {inherit.val})", LogLevel.Debug);
				break;

			default:
				Logger.Log($"[ITEM] Unknown target inherit type {inherit.typeID}", LogLevel.Debug);
				break;
		}
	}

	/// <summary>
	/// Uses an item at a world position (traps, AoE items, etc).
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot containing the item</param>
	/// <param name="pos">Target position in world coordinates</param>
	public void UseItemAtPosition(PlayerClient pc, int slot, FPOS pos)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[ITEM] UseItemAtPosition failed: No character loaded", LogLevel.Warning);
			SendUseItemPosResult(pc, 0, pos, false);
			return;
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[ITEM] UseItemAtPosition failed: No item at slot {slot}", LogLevel.Warning);
			SendUseItemPosResult(pc, 0, pos, false);
			return;
		}

		string playerName = character.appearance.name;

		// Validate item can be used at a position
		if (!CanUseItemAtPosition(item))
		{
			Logger.Log($"[ITEM] {playerName} cannot use {item.Name} at positions", LogLevel.Debug);
			SendUseItemPosResult(pc, (long)item.GetId(), pos, false);
			return;
		}

		// Check range from player
		var playerPos = pc.GetPosition();
		float distance = CalculateDistance(playerPos, pos);
		const float MaxUseRange = 30.0f;

		if (distance > MaxUseRange)
		{
			Logger.Log($"[ITEM] {playerName} target position too far ({distance:F1} > {MaxUseRange})", LogLevel.Debug);
			SendUseItemPosResult(pc, (long)item.GetId(), pos, false);
			return;
		}

		Logger.Log($"[ITEM] {playerName} using item {item.Name} at ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})", LogLevel.Debug);

		// Apply position-based effects
		bool success = ApplyItemEffectAtPosition(pc, item, pos);

		if (success)
		{
			ConsumeItem(pc, slot, item);
		}

		SendUseItemPosResult(pc, (long)item.GetId(), pos, success);
	}

	/// <summary>
	/// Checks if an item can be used at a world position.
	/// </summary>
	private bool CanUseItemAtPosition(Models.Item item)
	{
		// Items that can be placed at positions:
		// - AoE consumables
		// - Trap items
		// - Ground-target skill items
		if (item.SkillId > 0)
		{
			return true; // Skill items can be position targeted
		}

		return item.Type == (int)Models.ItemType.Consumables;
	}

	/// <summary>
	/// Applies item effects at a world position (AoE).
	/// </summary>
	private bool ApplyItemEffectAtPosition(PlayerClient pc, Models.Item item, FPOS pos)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Check for skill-based items
		if (item.SkillId > 0)
		{
			// Trigger skill at position
			Logger.Log($"[ITEM] {playerName} triggered skill {item.SkillId} at position", LogLevel.Debug);
			return true;
		}

		// Check for AoE heal/damage items via inherits
		if (item.Inherits != null && item.Inherits.Count > 0)
		{
			const float AoERadius = 10.0f;

			// Find all entities in range
			uint zoneId = pc.GetZone();
			var entitiesInRange = GetEntitiesInRange(zoneId, pos, AoERadius);

			int affectedCount = 0;
			foreach (var entity in entitiesInRange)
			{
				foreach (var inherit in item.Inherits)
				{
					ApplyInheritEffectOnTarget(pc, entity, inherit);
				}
				affectedCount++;
			}

			Logger.Log($"[ITEM] {playerName} applied AoE effect to {affectedCount} entities at position", LogLevel.Debug);
			return true;
		}

		Logger.Log($"[ITEM] Item {item.Name} has no position effects defined", LogLevel.Debug);
		return false;
	}

	/// <summary>
	/// Calculates distance between two positions.
	/// </summary>
	private static float CalculateDistance(FPOS a, FPOS b)
	{
		float dx = a.x - b.x;
		float dy = a.y - b.y;
		float dz = a.z - b.z;
		return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
	}

	/// <summary>
	/// Gets all entities within a radius of a position in a zone.
	/// </summary>
	/// <param name="zoneId">The zone to search in</param>
	/// <param name="position">Center position</param>
	/// <param name="radius">Search radius</param>
	/// <returns>Enumerable of entities within range</returns>
	private static IEnumerable<IEntity> GetEntitiesInRange(uint zoneId, FPOS position, float radius)
	{
		float radiusSq = radius * radius;

		// Check players in zone
		foreach (var player in LimeServer.PlayerClients)
		{
			if (player.GetZone() != zoneId) continue;
			var playerPos = player.GetPosition();
			float dx = playerPos.x - position.x;
			float dy = playerPos.y - position.y;
			float dz = playerPos.z - position.z;
			float distSq = dx * dx + dy * dy + dz * dz;
			if (distSq <= radiusSq)
			{
				yield return player;
			}
		}

		// Check monsters in zone
		if (LimeServer.Mobs.TryGetValue(zoneId, out var mobs))
		{
			foreach (var mob in mobs)
			{
				var mobPos = mob.GetPosition();
				float dx = mobPos.x - position.x;
				float dy = mobPos.y - position.y;
				float dz = mobPos.z - position.z;
				float distSq = dx * dx + dy * dy + dz * dz;
				if (distSq <= radiusSq)
				{
					yield return mob;
				}
			}
		}
	}

	/// <summary>
	/// Uses an item on another inventory slot (combining, upgrading, etc).
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot containing the item to use</param>
	/// <param name="targetSlot">Target inventory slot</param>
	public void UseItemOnSlot(PlayerClient pc, int slot, int targetSlot)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[ITEM] UseItemOnSlot failed: No character loaded", LogLevel.Warning);
			SendUseItemResult(pc, 0, false);
			return;
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;
		var targetItem = inventory.AtSlot(targetSlot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[ITEM] UseItemOnSlot failed: No item at slot {slot}", LogLevel.Warning);
			SendUseItemResult(pc, 0, false);
			return;
		}

		if (targetItem == null)
		{
			Logger.Log($"[ITEM] UseItemOnSlot failed: No target item at slot {targetSlot}", LogLevel.Warning);
			SendUseItemResult(pc, (long)item.GetId(), false);
			return;
		}

		string playerName = character.appearance.name;
		Logger.Log($"[ITEM] {playerName} using item {item.Name} on {targetItem.Name}", LogLevel.Debug);

		// Check if the source item has effects that can be applied to the target
		bool success = ApplyItemOnTarget(pc, item, targetItem, targetSlot);

		if (success)
		{
			// Consume the source item
			ConsumeItem(pc, slot, item);
		}

		SendUseItemResult(pc, (long)item.GetId(), success);
	}

	/// <summary>
	/// Applies the effect of using an item on another item.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="sourceItem">Item being used</param>
	/// <param name="targetItem">Item being targeted</param>
	/// <param name="targetSlot">Target item slot</param>
	/// <returns>True if the effect was applied successfully</returns>
	private bool ApplyItemOnTarget(PlayerClient pc, Models.Item sourceItem, Models.Item targetItem, int targetSlot)
	{
		// Check source item's inherits for target effects
		if (sourceItem.Inherits == null || sourceItem.Inherits.Count == 0)
		{
			Logger.Log($"[ITEM] Source item {sourceItem.Name} has no inherits/effects", LogLevel.Debug);
			return false;
		}

		var inventory = pc.GetInventory();
		bool anyEffectApplied = false;

		foreach (var inherit in sourceItem.Inherits)
		{
			// Effect type 10 = Durability restoration
			if (inherit.typeID == 10)
			{
				int restoreAmount = inherit.val;
				int newDurability = Math.Min(targetItem.GetDurability() + restoreAmount, targetItem.GetMaxDurability());
				targetItem.CurrentDurability = newDurability;
				anyEffectApplied = true;
				Logger.Log($"[ITEM] Restored {restoreAmount} durability to {targetItem.Name}", LogLevel.Debug);
			}
			// Effect type 11 = Full durability restoration
			else if (inherit.typeID == 11)
			{
				targetItem.RepairFully();
				anyEffectApplied = true;
				Logger.Log($"[ITEM] Fully restored durability of {targetItem.Name}", LogLevel.Debug);
			}
			// Effect type 12 = Grade upgrade
			else if (inherit.typeID == 12)
			{
				if (targetItem.Grade < 10) // Max grade cap
				{
					targetItem.Grade++;
					anyEffectApplied = true;
					Logger.Log($"[ITEM] Upgraded {targetItem.Name} to grade {targetItem.Grade}", LogLevel.Debug);
				}
			}
			// Effect type 13 = Socket adding
			else if (inherit.typeID == 13)
			{
				// Socket logic would go here if item supports sockets
				anyEffectApplied = true;
				Logger.Log($"[ITEM] Socket effect applied to {targetItem.Name}", LogLevel.Debug);
			}
		}

		if (anyEffectApplied)
		{
			// Update the target item in inventory
			inventory.UpdateItemAtSlot(targetSlot, targetItem);
			pc.SendInventory();
		}

		return anyEffectApplied;
	}

	/// <summary>
	/// Applies the effect of a consumable item.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="item">The item being used</param>
	/// <returns>True if effect was applied successfully</returns>
	private bool ApplyItemEffect(PlayerClient pc, Models.Item item)
	{
		// Check item inherits for effects
		if (item.Inherits == null || item.Inherits.Count == 0)
		{
			Logger.Log($"[ITEM] Item {item.Name} has no inherits/effects defined", LogLevel.Debug);
			return true; // Still consume the item
		}

		foreach (var inherit in item.Inherits)
		{
			ApplyInheritEffect(pc, inherit);
		}

		return true;
	}

	/// <summary>
	/// Applies a single inherit effect to the player.
	/// </summary>
	private void ApplyInheritEffect(PlayerClient pc, Models.Inherit inherit)
	{
		// Common inherit types:
		// 1 = HP restore
		// 2 = MP restore
		// 3 = LP restore
		// etc.

		switch (inherit.typeID)
		{
			case 1: // HP restore
				pc.UpdateHP((int)(pc.GetStatus().hp + inherit.val), true);
				Logger.Log($"[ITEM] Restored {inherit.val} HP", LogLevel.Debug);
				break;

			case 2: // MP restore
				pc.UpdateMP((int)(pc.GetStatus().mp + inherit.val), true);
				Logger.Log($"[ITEM] Restored {inherit.val} MP", LogLevel.Debug);
				break;

			default:
				Logger.Log($"[ITEM] Unknown inherit type {inherit.typeID} with value {inherit.val}", LogLevel.Debug);
				break;
		}
	}

	/// <summary>
	/// Consumes an item (decrements count or removes from inventory).
	/// </summary>
	private void ConsumeItem(PlayerClient pc, int slot, Models.Item item)
	{
		var inventory = pc.GetInventory();

		if (item.Stackable() && item.Count > 1)
		{
			// Decrement count
			item.UpdateAmount(item.Count - 1);
			inventory.UpdateItemAtSlot(slot, item);
			Logger.Log($"[ITEM] Item {item.Name} count decremented to {item.Count}", LogLevel.Debug);
		}
		else
		{
			// Remove item from slot
			inventory.RemoveItem(slot);
			Logger.Log($"[ITEM] Item {item.Name} removed from inventory", LogLevel.Debug);
		}

		// Send inventory update
		pc.SendInventory();
	}

	/// <summary>
	/// Sends SC_USE_ITEM_SLOT_RESULT packet.
	/// </summary>
	private void SendUseItemResult(PlayerClient pc, long itemInstId, bool success)
	{
		using PacketWriter pw = new();
		pw.Write((ushort)PacketType.SC_USE_ITEM_SLOT_RESULT);
		pw.Write(itemInstId);
		pw.Write(success);

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_USE_ITEM_OBJ_RESULT_LIST packet.
	/// </summary>
	private void SendUseItemObjResult(PlayerClient pc, long itemInstId, long targetInstId, bool success)
	{
		using PacketWriter pw = new();
		pw.Write((ushort)PacketType.SC_USE_ITEM_OBJ_RESULT_LIST);
		// Variable length packet - write basic result
		pw.Write((int)1); // result count
		pw.Write(itemInstId);
		pw.Write(targetInstId);
		pw.Write(success);

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_USE_ITEM_POS_RESULT_LIST packet.
	/// </summary>
	private void SendUseItemPosResult(PlayerClient pc, long itemInstId, FPOS pos, bool success)
	{
		using PacketWriter pw = new();
		pw.Write((ushort)PacketType.SC_USE_ITEM_POS_RESULT_LIST);
		// Variable length packet - write basic result
		pw.Write((int)1); // result count
		pw.Write(itemInstId);
		pw.Write(pos.x);
		pw.Write(pos.y);
		pw.Write(pos.z);
		pw.Write(success);

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Gets item definition from ItemDB by type ID.
	/// </summary>
	/// <param name="typeId">Item type ID</param>
	/// <returns>Item definition or null if not found</returns>
	public Models.Item? GetItemDefinition(int typeId)
	{
		return LimeServer.ItemDB.FirstOrDefault(i => i.Id == typeId);
	}

	// ============ ITEM REPAIR ============

	/// <summary>
	/// Base repair cost multiplier.
	/// </summary>
	private const float RepairCostMultiplier = 0.1f;

	/// <summary>
	/// Gets the repair price for all equipped items.
	/// </summary>
	public uint GetEquippedItemsRepairPrice(PlayerClient pc)
	{
		var equipment = pc.GetEquipment(true);
		uint totalPrice = 0;

		// Calculate repair cost for all equipped items by iterating through all slots
		for (int i = 1; i <= 20; i++)
		{
			var slot = (EQUIP_SLOT)i;
			var item = equipment.GetItemInSlot(slot);
			if (item is Models.Item equippedItem)
			{
				totalPrice += CalculateRepairPrice(equippedItem);
			}
		}

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[REPAIR] {playerName} equipped items repair price: {totalPrice}", LogLevel.Debug);

		return totalPrice;
	}

	/// <summary>
	/// Repairs all equipped items.
	/// </summary>
	public bool RepairAllEquippedItems(PlayerClient pc)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[REPAIR] RepairAllEquippedItems failed: No character loaded", LogLevel.Warning);
			return false;
		}

		uint totalCost = GetEquippedItemsRepairPrice(pc);
		string playerName = character.appearance.name;

		// Check if player has enough Peder
		if (!LimeServer.CurrencyService.ProcessRepair(pc, totalCost))
		{
			Logger.Log($"[REPAIR] {playerName} cannot afford {totalCost} Peder for repairs", LogLevel.Debug);
			return false;
		}

		Logger.Log($"[REPAIR] {playerName} repaired all equipped items for {totalCost} Peder", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Calculates the repair price for an item based on its properties.
	/// </summary>
	public static uint CalculateRepairPrice(Models.Item item)
	{
		// Base price on item value and durability loss
		// Formula: base_price * grade * durability_loss_ratio * multiplier
		int basePrice = item.Price > 0 ? item.Price : 100;
		int grade = item.Grade > 0 ? item.Grade : 1;

		// Calculate actual durability loss ratio
		int currentDurability = item.GetDurability();
		int maxDurability = item.GetMaxDurability();
		float durabilityLoss = maxDurability > 0 ? 1.0f - ((float)currentDurability / maxDurability) : 0.0f;

		// No cost if item is at full durability
		if (durabilityLoss <= 0)
			return 0;

		uint repairCost = (uint)(basePrice * grade * durabilityLoss * RepairCostMultiplier);
		return Math.Max(1, repairCost); // Minimum 1 currency if there's any durability loss
	}

	// ============ ITEM COMPOSITION (Upgrading/Enchanting) ============

	private readonly ConcurrentDictionary<long, CompositionSession> _activeCompositions = new();
	private readonly Random _compositionRandom = new();

	/// <summary>
	/// Base success rate for item composition (75%).
	/// </summary>
	private const int BaseSuccessRate = 75;

	/// <summary>
	/// Success rate reduction per enchant level (5% per level).
	/// </summary>
	private const int SuccessRateReductionPerLevel = 5;

	/// <summary>
	/// Success rate bonus per material slot used (3% per material).
	/// </summary>
	private const int SuccessRateBonusPerMaterial = 3;

	/// <summary>
	/// Minimum success rate (cannot go below 10%).
	/// </summary>
	private const int MinSuccessRate = 10;

	/// <summary>
	/// Tracks active composition session data.
	/// </summary>
	private class CompositionSession
	{
		public int BaseSlot { get; set; }
		public int CurrentEnchantLevel { get; set; }
	}

	/// <summary>
	/// Prepares an item for composition (upgrading/enchanting).
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot containing the item to compose</param>
	/// <returns>True if composition preparation succeeded</returns>
	public bool ReadyComposition(PlayerClient pc, int slot)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[COMPOSE] {playerName} failed: No item at slot {slot}", LogLevel.Warning);
			return false;
		}

		// Validate item can be composed (must be equipment with MaxEnchantCount > 0)
		if (item.MaxEnchantCount <= 0)
		{
			Logger.Log($"[COMPOSE] {playerName} failed: Item {item.Name} cannot be composed (MaxEnchantCount: {item.MaxEnchantCount})", LogLevel.Warning);
			return false;
		}

		// Check if item is already at max enchant level
		if (item.Grade >= item.MaxEnchantCount)
		{
			Logger.Log($"[COMPOSE] {playerName} failed: Item {item.Name} already at max enchant level ({item.Grade}/{item.MaxEnchantCount})", LogLevel.Warning);
			return false;
		}

		// Store composition session
		_activeCompositions[playerId] = new CompositionSession
		{
			BaseSlot = slot,
			CurrentEnchantLevel = item.Grade
		};

		Logger.Log($"[COMPOSE] {playerName} ready to compose item {item.Name} (current level: {item.Grade}/{item.MaxEnchantCount})", LogLevel.Debug);

		// Send ready response
		SendReadyComposition(pc);
		return true;
	}

	/// <summary>
	/// Cancels the current composition.
	/// </summary>
	/// <param name="pc">The player client</param>
	public void CancelComposition(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_activeCompositions.TryRemove(playerId, out _))
		{
			Logger.Log($"[COMPOSE] {playerName} canceled composition", LogLevel.Debug);
		}
	}

	/// <summary>
	/// Executes item composition with materials.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="baseSlot">Inventory slot containing the base item</param>
	/// <param name="materialSlots">Array of 5 inventory slots containing materials (0 = empty)</param>
	/// <returns>True if composition was executed (regardless of success/fail)</returns>
	public bool ExecuteComposition(PlayerClient pc, int baseSlot, int[] materialSlots)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		var inventory = pc.GetInventory();
		var baseItem = inventory.AtSlot(baseSlot) as Models.Item;

		if (baseItem == null)
		{
			Logger.Log($"[COMPOSE] {playerName} failed: No base item at slot {baseSlot}", LogLevel.Warning);
			SendCompositionFinish(pc, 0); // 0 = failure
			return false;
		}

		// Validate materials exist and collect them
		var materials = new List<(int slot, Models.Item item)>();
		if (materialSlots != null)
		{
			foreach (int matSlot in materialSlots)
			{
				if (matSlot > 0)
				{
					var material = inventory.AtSlot(matSlot) as Models.Item;
					if (material != null)
					{
						materials.Add((matSlot, material));
					}
				}
			}
		}

		// Calculate success rate
		int successRate = CalculateCompositionSuccessRate(baseItem, materials.Count);
		Logger.Log($"[COMPOSE] {playerName} composition success rate: {successRate}% (enchant level: {baseItem.Grade}, materials: {materials.Count})", LogLevel.Debug);

		// Roll for success
		int roll = _compositionRandom.Next(100);
		bool success = roll < successRate;

		// Consume materials regardless of success/failure
		foreach (var (matSlot, material) in materials)
		{
			if (material.Stackable() && material.Count > 1)
			{
				material.UpdateAmount(material.Count - 1);
				inventory.UpdateItemAtSlot(matSlot, material);
			}
			else
			{
				inventory.RemoveItem(matSlot);
			}
			Logger.Log($"[COMPOSE] Consumed material {material.Name} from slot {matSlot}", LogLevel.Debug);
		}

		if (success)
		{
			// Upgrade the item
			baseItem.Grade++;
			inventory.UpdateItemAtSlot(baseSlot, baseItem);

			Logger.Log($"[COMPOSE] {playerName} successfully enchanted {baseItem.Name} to +{baseItem.Grade}", LogLevel.Information);
		}
		else
		{
			// Composition failed - item remains but materials are lost
			Logger.Log($"[COMPOSE] {playerName} failed to enchant {baseItem.Name} (roll: {roll}, needed: {successRate})", LogLevel.Information);
		}

		// Send inventory update
		pc.SendInventory();

		// Send composition result
		SendCompositionFinish(pc, success ? baseItem.Id : 0);

		// Clean up active composition
		_activeCompositions.TryRemove(playerId, out _);

		return true;
	}

	/// <summary>
	/// Calculates the success rate for item composition.
	/// </summary>
	/// <param name="item">The base item being composed</param>
	/// <param name="materialCount">Number of materials being used</param>
	/// <returns>Success rate percentage (0-100)</returns>
	private int CalculateCompositionSuccessRate(Models.Item item, int materialCount)
	{
		// Base rate - reduction per enchant level + bonus per material
		int rate = BaseSuccessRate
			- (item.Grade * SuccessRateReductionPerLevel)
			+ (materialCount * SuccessRateBonusPerMaterial);

		// Clamp to valid range
		return Math.Max(MinSuccessRate, Math.Min(100, rate));
	}

	/// <summary>
	/// Sends SC_READY_INVENTORY_COMPOSE_ITEM packet.
	/// </summary>
	private void SendReadyComposition(PlayerClient pc)
	{
		using PacketWriter pw = new();
		pw.Write((ushort)PacketType.SC_READY_INVENTORY_COMPOSE_ITEM);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_INVENTORY_COMPOSE_ITEM_FINISH packet.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="resultInstId">Item instance ID (0 if failed)</param>
	private void SendCompositionFinish(PlayerClient pc, long resultInstId)
	{
		using PacketWriter pw = new();
		pw.Write((ushort)PacketType.SC_INVENTORY_COMPOSE_ITEM_FINISH);
		pw.Write(resultInstId);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Cleans up composition state for a player (on disconnect).
	/// </summary>
	public void CleanupPlayerComposition(long playerId)
	{
		_activeCompositions.TryRemove(playerId, out _);
		_activeItemUsage.TryRemove(playerId, out _);
	}

	// ============ ITEM INFO ============

	/// <summary>
	/// Gets detailed item information for a slot and sends to client.
	/// </summary>
	public void GetSlotItemInfo(PlayerClient pc, int slotType, int slotIndex)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		Models.Item? item = null;

		// slotType determines where to look
		// 0 = inventory, 1 = equipment, 2 = bank, etc.
		switch (slotType)
		{
			case 0: // Inventory
				var inventory = pc.GetInventory();
				item = inventory.AtSlot(slotIndex) as Models.Item;
				break;
			case 1: // Combat equipment
				var combatEquip = pc.GetEquipment(true);
				item = combatEquip.GetItemInSlot((EQUIP_SLOT)slotIndex) as Models.Item;
				break;
			case 2: // Life equipment
				var lifeEquip = pc.GetEquipment(false);
				item = lifeEquip.GetItemInSlot((EQUIP_SLOT)slotIndex) as Models.Item;
				break;
		}

		if (item == null)
		{
			Logger.Log($"[ITEM] {playerName} slot info: no item at type {slotType} index {slotIndex}", LogLevel.Debug);
			return;
		}

		Logger.Log($"[ITEM] {playerName} requesting info for {item.Name}", LogLevel.Debug);

		// Send SC_SLOT_ITEM_INFO
		using PacketWriter pw = new();
		pw.Write((ushort)PacketType.SC_SLOT_ITEM_INFO);
		pw.Write(item.Id);
		pw.Write(item.Name ?? "Unknown");
		pw.Write(item.Desc ?? "");
		pw.Write(item.Grade);
		pw.Write(item.Type);
		pw.Write(item.Price);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	// ============ ITEM USAGE CANCELLATION ============

	/// <summary>Tracks active item usage per player</summary>
	private readonly ConcurrentDictionary<long, int> _activeItemUsage = new();

	/// <summary>
	/// Starts an item usage that can be canceled (e.g., consumables with cast time).
	/// </summary>
	public void StartItemUsage(PlayerClient pc, int slot)
	{
		long playerId = pc.GetId();
		_activeItemUsage[playerId] = slot;
	}

	/// <summary>
	/// Cancels any pending item usage.
	/// </summary>
	public void CancelItemUsage(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_activeItemUsage.TryRemove(playerId, out int slot))
		{
			Logger.Log($"[ITEM] {playerName} canceled item usage from slot {slot}", LogLevel.Debug);
		}
	}

	/// <summary>
	/// Uses an item on the player's current target.
	/// </summary>
	public void UseItemOnCurrentTarget(PlayerClient pc, int slot)
	{
		long targetId = pc.GetCurrentTarget();
		if (targetId == 0)
		{
			Logger.Log("[ITEM] UseItemOnCurrentTarget failed: No target selected", LogLevel.Debug);
			return;
		}

		UseItemOnObject(pc, slot, targetId);
	}
}
