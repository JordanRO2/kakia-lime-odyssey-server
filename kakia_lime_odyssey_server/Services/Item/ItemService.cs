/// <summary>
/// Service for managing item usage and consumption.
/// </summary>
/// <remarks>
/// Handles using consumable items on self, objects, positions, and other inventory slots.
/// Uses: PlayerInventory for item access, ItemDB for item definitions
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.SC;
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
	/// Uses an item on a target object (NPC, pet, etc).
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
			return;
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[ITEM] UseItemOnObject failed: No item at slot {slot}", LogLevel.Warning);
			return;
		}

		string playerName = character.appearance.name;
		Logger.Log($"[ITEM] {playerName} using item {item.Name} on object {targetInstId}", LogLevel.Debug);

		// For now, just consume the item
		// TODO: Implement specific target interactions
		ConsumeItem(pc, slot, item);

		// Send result packet
		SendUseItemObjResult(pc, (long)item.GetId(), targetInstId, true);
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
			return;
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[ITEM] UseItemAtPosition failed: No item at slot {slot}", LogLevel.Warning);
			return;
		}

		string playerName = character.appearance.name;
		Logger.Log($"[ITEM] {playerName} using item {item.Name} at position ({pos.x}, {pos.y}, {pos.z})", LogLevel.Debug);

		// For now, just consume the item
		// TODO: Implement position-based item effects
		ConsumeItem(pc, slot, item);

		// Send result packet
		SendUseItemPosResult(pc, (long)item.GetId(), pos, true);
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

		// TODO: Implement item combining/upgrading logic
		// For now, just report success without consuming
		SendUseItemResult(pc, (long)item.GetId(), true);
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

		// TODO: Check player has enough gold
		// TODO: Deduct gold from player

		// For now, assume repair is always successful
		string playerName = character.appearance.name;
		Logger.Log($"[REPAIR] {playerName} repaired all equipped items for {totalCost} gold", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Calculates the repair price for an item based on its properties.
	/// </summary>
	public static uint CalculateRepairPrice(Models.Item item)
	{
		// Base price on item value and durability loss
		// For now, use a simple formula: base_price * grade * (1 - durability_ratio)
		int basePrice = item.Price > 0 ? item.Price : 100;
		int grade = item.Grade > 0 ? item.Grade : 1;

		// TODO: Calculate actual durability loss when durability tracking is implemented
		// For now, assume 50% durability loss
		float durabilityLoss = 0.5f;

		uint repairCost = (uint)(basePrice * grade * durabilityLoss * RepairCostMultiplier);
		return Math.Max(1, repairCost); // Minimum 1 currency
	}

	// ============ ITEM COMPOSITION (Upgrading/Enchanting) ============

	private readonly ConcurrentDictionary<long, int> _activeCompositions = new();

	/// <summary>
	/// Prepares an item for composition (upgrading/enchanting).
	/// </summary>
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

		// TODO: Validate item can be composed (equipment only, max upgrade level, etc.)

		_activeCompositions[playerId] = slot;

		Logger.Log($"[COMPOSE] {playerName} ready to compose item {item.Name}", LogLevel.Debug);
		return true;
	}

	/// <summary>
	/// Cancels the current composition.
	/// </summary>
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
	public bool ExecuteComposition(PlayerClient pc, int baseSlot, int[] materialSlots)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		var inventory = pc.GetInventory();
		var baseItem = inventory.AtSlot(baseSlot) as Models.Item;

		if (baseItem == null)
		{
			Logger.Log($"[COMPOSE] {playerName} failed: No base item at slot {baseSlot}", LogLevel.Warning);
			return false;
		}

		// TODO: Validate materials exist
		// TODO: Calculate success rate based on materials
		// TODO: Consume materials
		// TODO: Apply composition result (success: upgrade item, fail: potentially destroy)

		Logger.Log($"[COMPOSE] {playerName} executed composition on {baseItem.Name} with {materialSlots.Length} materials", LogLevel.Debug);

		// Clean up active composition
		_activeCompositions.TryRemove(playerId, out _);

		return true;
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
