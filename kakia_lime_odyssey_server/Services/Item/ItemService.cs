/// <summary>
/// Service for managing item usage and consumption.
/// </summary>
/// <remarks>
/// Handles using consumable items on self, objects, positions, and other inventory slots.
/// Uses: PlayerInventory for item access, ItemDB for item definitions
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Common;
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
		pw.Writer.Write((ushort)PacketType.SC_USE_ITEM_SLOT_RESULT);
		pw.Writer.Write(itemInstId);
		pw.Writer.Write(success);

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_USE_ITEM_OBJ_RESULT_LIST packet.
	/// </summary>
	private void SendUseItemObjResult(PlayerClient pc, long itemInstId, long targetInstId, bool success)
	{
		using PacketWriter pw = new();
		pw.Writer.Write((ushort)PacketType.SC_USE_ITEM_OBJ_RESULT_LIST);
		// Variable length packet - write basic result
		pw.Writer.Write((int)1); // result count
		pw.Writer.Write(itemInstId);
		pw.Writer.Write(targetInstId);
		pw.Writer.Write(success);

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_USE_ITEM_POS_RESULT_LIST packet.
	/// </summary>
	private void SendUseItemPosResult(PlayerClient pc, long itemInstId, FPOS pos, bool success)
	{
		using PacketWriter pw = new();
		pw.Writer.Write((ushort)PacketType.SC_USE_ITEM_POS_RESULT_LIST);
		// Variable length packet - write basic result
		pw.Writer.Write((int)1); // result count
		pw.Writer.Write(itemInstId);
		pw.Writer.Write(pos.x);
		pw.Writer.Write(pos.y);
		pw.Writer.Write(pos.z);
		pw.Writer.Write(success);

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
}
