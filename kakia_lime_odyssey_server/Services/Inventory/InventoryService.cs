using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Models;

namespace kakia_lime_odyssey_server.Services.Inventory;

/// <summary>
/// Service for handling inventory operations including item stacking, moving, and slot management.
/// </summary>
public static class InventoryService
{
	/// <summary>
	/// Determines if two items can be stacked together.
	/// </summary>
	public static bool CanStack(IItem item1, IItem item2)
	{
		if (item1 == null || item2 == null)
			return false;

		// Items must have the same ID and both must be stackable
		return item1.GetId() == item2.GetId() && item1.Stackable() && item2.Stackable();
	}

	/// <summary>
	/// Stacks two items together, updating their amounts.
	/// </summary>
	/// <returns>A result indicating the new amounts for both items.</returns>
	public static StackResult StackItems(IItem source, IItem target)
	{
		var totalAmount = source.GetAmount() + target.GetAmount();

		if (totalAmount <= Item.MaxStackSize)
		{
			source.UpdateAmount(0);
			target.UpdateAmount(totalAmount);

			return new StackResult
			{
				SourceAmount = 0,
				TargetAmount = totalAmount,
				SourceDepleted = true
			};
		}
		else
		{
			source.UpdateAmount(totalAmount - Item.MaxStackSize);
			target.UpdateAmount(Item.MaxStackSize);

			return new StackResult
			{
				SourceAmount = totalAmount - Item.MaxStackSize,
				TargetAmount = Item.MaxStackSize,
				SourceDepleted = false
			};
		}
	}

	/// <summary>
	/// Moves an item from one slot to another in the inventory.
	/// </summary>
	public static MoveItemResult MoveItem(IPlayerInventory inventory, int fromSlot, int toSlot, long count)
	{
		var sourceItem = inventory.AtSlot(fromSlot);
		var targetItem = inventory.AtSlot(toSlot);

		if (sourceItem == null)
		{
			return new MoveItemResult
			{
				Success = false,
				Error = "No item in source slot"
			};
		}

		var moves = new List<SLOT_MOVE>();

		// Moving to empty slot
		if (targetItem == null)
		{
			inventory.AddItem(sourceItem, toSlot);
			inventory.RemoveItem(fromSlot);

			moves.Add(new SLOT_MOVE
			{
				fromCount = 0,
				toCount = (ulong)count,
				fromSlot = new SLOT { slot = fromSlot },
				toSlot = new SLOT { slot = toSlot }
			});

			return new MoveItemResult
			{
				Success = true,
				Moves = moves
			};
		}

		// Moving to occupied slot - check for stacking
		if (CanStack(sourceItem, targetItem))
		{
			var stackResult = StackItems(sourceItem, targetItem);

			if (stackResult.SourceDepleted)
				inventory.RemoveItem(fromSlot);
			else
				inventory.UpdateItemAtSlot(fromSlot, sourceItem);

			inventory.UpdateItemAtSlot(toSlot, targetItem);

			moves.Add(new SLOT_MOVE
			{
				fromCount = stackResult.SourceAmount,
				toCount = stackResult.TargetAmount,
				fromSlot = new SLOT { slot = fromSlot },
				toSlot = new SLOT { slot = toSlot }
			});

			return new MoveItemResult
			{
				Success = true,
				Moves = moves
			};
		}

		// Swap items
		inventory.RemoveItem(toSlot);
		inventory.RemoveItem(fromSlot);
		inventory.AddItem(sourceItem, toSlot);
		inventory.AddItem(targetItem, fromSlot);

		moves.Add(new SLOT_MOVE
		{
			fromCount = (ulong)count,
			toCount = (ulong)count,
			fromSlot = new SLOT { slot = fromSlot },
			toSlot = new SLOT { slot = toSlot }
		});

		moves.Add(new SLOT_MOVE
		{
			fromCount = (ulong)count,
			toCount = (ulong)count,
			fromSlot = new SLOT { slot = toSlot },
			toSlot = new SLOT { slot = fromSlot }
		});

		return new MoveItemResult
		{
			Success = true,
			Moves = moves
		};
	}

	/// <summary>
	/// Equips an item from inventory to equipment slot.
	/// </summary>
	public static EquipItemResult EquipItem(
		IPlayerInventory inventory,
		IPlayerEquipment equipment,
		int inventorySlot,
		EQUIP_SLOT equipSlot)
	{
		var item = inventory.AtSlot(inventorySlot);
		if (item == null)
		{
			return new EquipItemResult
			{
				Success = false,
				Error = "No item in inventory slot"
			};
		}

		var equipActions = new List<EQUIPMENT>();
		IItem? previousItem = null;
		int previousItemNewSlot = -1;

		// Unequip existing item if slot is occupied
		if (equipment.IsEquipped(equipSlot))
		{
			previousItem = equipment.UnEquip(equipSlot);
		}

		// Remove item from inventory
		if (!inventory.RemoveItem(inventorySlot))
		{
			return new EquipItemResult
			{
				Success = false,
				Error = "Failed to remove item from inventory"
			};
		}

		// Add previous item back to inventory
		if (previousItem != null)
		{
			previousItemNewSlot = inventory.AddItem(previousItem, inventorySlot);
			var prevItemCast = previousItem as Item;
			if (prevItemCast != null)
			{
				equipActions.Add(prevItemCast.EquipChangePart(EQUIPMENT_TYPE.TYPE_UNEQUIP, previousItemNewSlot));
			}
		}

		// Equip new item
		equipment.Equip(equipSlot, item);
		var itemCast = item as Item;
		if (itemCast != null)
		{
			equipActions.Add(itemCast.EquipChangePart(EQUIPMENT_TYPE.TYPE_EQUIP, inventorySlot));
		}

		return new EquipItemResult
		{
			Success = true,
			EquipActions = equipActions,
			PreviousItem = previousItem,
			PreviousItemSlot = previousItemNewSlot
		};
	}

	/// <summary>
	/// Unequips an item from equipment to inventory.
	/// </summary>
	public static UnequipItemResult UnequipItem(
		IPlayerInventory inventory,
		IPlayerEquipment equipment,
		EQUIP_SLOT equipSlot)
	{
		if (!equipment.IsEquipped(equipSlot))
		{
			return new UnequipItemResult
			{
				Success = false,
				Error = "No item equipped in slot"
			};
		}

		var item = equipment.UnEquip(equipSlot);
		if (item == null)
		{
			return new UnequipItemResult
			{
				Success = false,
				Error = "Failed to unequip item"
			};
		}

		int inventorySlot = inventory.AddItem(item);
		if (inventorySlot == -1)
		{
			// Put item back if inventory is full
			equipment.Equip(equipSlot, item);
			return new UnequipItemResult
			{
				Success = false,
				Error = "Inventory is full"
			};
		}

		var equipActions = new List<EQUIPMENT>();
		var itemCast = item as Item;
		if (itemCast != null)
		{
			equipActions.Add(itemCast.EquipChangePart(EQUIPMENT_TYPE.TYPE_UNEQUIP, inventorySlot));
		}

		return new UnequipItemResult
		{
			Success = true,
			EquipActions = equipActions,
			InventorySlot = inventorySlot
		};
	}
}

#region Result Classes

public class StackResult
{
	public ulong SourceAmount { get; set; }
	public ulong TargetAmount { get; set; }
	public bool SourceDepleted { get; set; }
}

public class MoveItemResult
{
	public bool Success { get; set; }
	public string? Error { get; set; }
	public List<MOVE_SLOT> Moves { get; set; } = new();
}

public class EquipItemResult
{
	public bool Success { get; set; }
	public string? Error { get; set; }
	public List<EQUIPMENT> EquipActions { get; set; } = new();
	public IItem? PreviousItem { get; set; }
	public int PreviousItemSlot { get; set; }
}

public class UnequipItemResult
{
	public bool Success { get; set; }
	public string? Error { get; set; }
	public List<EQUIPMENT> EquipActions { get; set; } = new();
	public int InventorySlot { get; set; }
}

#endregion
