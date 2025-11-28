/// <summary>
/// Handler for CS_MOVE_SLOT_ITEM packet - moves items between slots.
/// </summary>
/// <remarks>
/// Handles moving items within inventory, and between inventory and bank.
/// Uses ITEM_SLOT.type to determine source/destination container type.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_utils.Extensions;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

/// <summary>
/// Handles item slot movement between inventory/bank/equipment.
/// </summary>
[PacketHandlerAttr(PacketType.CS_MOVE_SLOT_ITEM)]
class CS_MOVE_SLOT_ITEM_Handler : PacketHandler
{
	/// <summary>
	/// Checks if two items should stack together.
	/// </summary>
	/// <param name="item1">First item</param>
	/// <param name="item2">Second item</param>
	/// <returns>True if items can stack</returns>
	private bool ShouldStack(IItem item1, IItem item2)
	{
		return item1.GetId() == item2.GetId();
	}

	/// <summary>
	/// Creates stacks by combining item amounts.
	/// </summary>
	/// <param name="item1">Source item</param>
	/// <param name="item2">Destination item</param>
	private void CreateStacks(IItem item1, IItem item2)
	{
		var totalAmount = item1.GetAmount() + item2.GetAmount();
		if (totalAmount <= Models.Item.MaxStackSize)
		{
			item1.UpdateAmount(0);
			item2.UpdateAmount(totalAmount);
			return;
		}
		else
		{
			item1.UpdateAmount(totalAmount - Models.Item.MaxStackSize);
			item2.UpdateAmount(Models.Item.MaxStackSize);
		}
	}

	/// <summary>
	/// Processes item movement between slots.
	/// </summary>
	/// <param name="client">The client connection</param>
	/// <param name="p">The raw packet data</param>
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var cs_move = PacketConverter.Extract<CS_MOVE_SLOT_ITEM>(p.Payload);
		var inventory = client.GetInventory();
		var bank = client.GetBank();

		var fromType = (SlotType)cs_move.from.type;
		var toType = (SlotType)cs_move.to.type;

		// Route to appropriate handler based on slot types
		SC_MOVE_SLOT_ITEM sc_move;

		if (fromType == SlotType.Inventory && toType == SlotType.Bank)
		{
			// Inventory -> Bank
			sc_move = HandleInventoryToBank(inventory, bank, cs_move);
		}
		else if (fromType == SlotType.Bank && toType == SlotType.Inventory)
		{
			// Bank -> Inventory
			sc_move = HandleBankToInventory(bank, inventory, cs_move);
		}
		else if (fromType == SlotType.Bank && toType == SlotType.Bank)
		{
			// Bank -> Bank (move within bank)
			sc_move = HandleBankToBank(bank, cs_move);
		}
		else
		{
			// Inventory -> Inventory (default case)
			sc_move = HandleInventoryToInventory(inventory, cs_move);
		}

		using PacketWriter pw = new();
		pw.Write(sc_move);

		Logger.Log(pw.ToSizedPacket().ToFormatedHexString());
		client.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Handles item movement within inventory.
	/// </summary>
	private SC_MOVE_SLOT_ITEM HandleInventoryToInventory(IPlayerInventory inventory, CS_MOVE_SLOT_ITEM cs_move)
	{
		var item_slot1 = inventory.AtSlot(cs_move.from.slot);
		var item_slot2 = inventory.AtSlot(cs_move.to.slot);

		SC_MOVE_SLOT_ITEM sc_move = new()
		{
			move_list = new()
		};

		// Moving item to empty slot
		if (item_slot2 is null && item_slot1 is not null)
		{
			inventory.AddItem(item_slot1, cs_move.to.slot);
			inventory.RemoveItem(cs_move.from.slot);

			sc_move.move_list.Add(new()
			{
				fromCount = 0,
				toCount = item_slot1.GetAmount(),
				fromSlot = cs_move.from,
				toSlot = cs_move.to
			});
		}
		// Moving item to slot where there already is an item
		else if (item_slot1 is not null && item_slot2 is not null)
		{
			if (ShouldStack(item_slot1, item_slot2))
			{
				CreateStacks(item_slot1, item_slot2);
				if (item_slot1.GetAmount() == 0)
					inventory.RemoveItem(cs_move.from.slot);
				else
					inventory.UpdateItemAtSlot(cs_move.from.slot, item_slot1);

				inventory.UpdateItemAtSlot(cs_move.to.slot, item_slot2);

				sc_move.move_list.Add(new()
				{
					fromCount = item_slot1.GetAmount(),
					toCount = item_slot2.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});
			}
			else
			{
				// Regular switch-a-roo of items
				inventory.RemoveItem(cs_move.to.slot);
				inventory.RemoveItem(cs_move.from.slot);

				inventory.AddItem(item_slot1, cs_move.to.slot);
				inventory.AddItem(item_slot2, cs_move.from.slot);

				sc_move.move_list.Add(new()
				{
					fromCount = item_slot1.GetAmount(),
					toCount = item_slot1.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});

				sc_move.move_list.Add(new()
				{
					fromCount = item_slot2.GetAmount(),
					toCount = item_slot2.GetAmount(),
					fromSlot = cs_move.to,
					toSlot = cs_move.from
				});
			}
		}

		return sc_move;
	}

	/// <summary>
	/// Handles item movement from inventory to bank.
	/// </summary>
	private SC_MOVE_SLOT_ITEM HandleInventoryToBank(IPlayerInventory inventory, IPlayerBank bank, CS_MOVE_SLOT_ITEM cs_move)
	{
		SC_MOVE_SLOT_ITEM sc_move = new()
		{
			move_list = new()
		};

		if (!bank.IsOpen)
		{
			Logger.Log("[BANK] Attempted to move item to bank but bank is not open", LogLevel.Warning);
			return sc_move;
		}

		var invItem = inventory.AtSlot(cs_move.from.slot);
		var bankItem = bank.AtSlot(cs_move.to.slot);

		if (invItem is null)
		{
			Logger.Log($"[BANK] No item at inventory slot {cs_move.from.slot}", LogLevel.Warning);
			return sc_move;
		}

		// Transfer to empty bank slot (move entire stack)
		if (bankItem is null)
		{
			inventory.RemoveItem(cs_move.from.slot);
			bank.AddItem(invItem, cs_move.to.slot);

			sc_move.move_list.Add(new()
			{
				fromCount = 0,
				toCount = invItem.GetAmount(),
				fromSlot = cs_move.from,
				toSlot = cs_move.to
			});
		}
		// Stack with existing bank item or swap
		else
		{
			if (ShouldStack(invItem, bankItem))
			{
				CreateStacks(invItem, bankItem);
				if (invItem.GetAmount() == 0)
					inventory.RemoveItem(cs_move.from.slot);
				else
					inventory.UpdateItemAtSlot(cs_move.from.slot, invItem);

				bank.UpdateItemAtSlot(cs_move.to.slot, bankItem);

				sc_move.move_list.Add(new()
				{
					fromCount = invItem.GetAmount(),
					toCount = bankItem.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});
			}
			else
			{
				// Swap items
				inventory.RemoveItem(cs_move.from.slot);
				bank.RemoveItem(cs_move.to.slot);
				inventory.AddItem(bankItem, cs_move.from.slot);
				bank.AddItem(invItem, cs_move.to.slot);

				sc_move.move_list.Add(new()
				{
					fromCount = bankItem.GetAmount(),
					toCount = invItem.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});

				sc_move.move_list.Add(new()
				{
					fromCount = invItem.GetAmount(),
					toCount = bankItem.GetAmount(),
					fromSlot = cs_move.to,
					toSlot = cs_move.from
				});
			}
		}

		Logger.Log($"[BANK] Moved item from inventory slot {cs_move.from.slot} to bank slot {cs_move.to.slot}", LogLevel.Debug);
		return sc_move;
	}

	/// <summary>
	/// Handles item movement from bank to inventory.
	/// </summary>
	private SC_MOVE_SLOT_ITEM HandleBankToInventory(IPlayerBank bank, IPlayerInventory inventory, CS_MOVE_SLOT_ITEM cs_move)
	{
		SC_MOVE_SLOT_ITEM sc_move = new()
		{
			move_list = new()
		};

		if (!bank.IsOpen)
		{
			Logger.Log("[BANK] Attempted to move item from bank but bank is not open", LogLevel.Warning);
			return sc_move;
		}

		var bankItem = bank.AtSlot(cs_move.from.slot);
		var invItem = inventory.AtSlot(cs_move.to.slot);

		if (bankItem is null)
		{
			Logger.Log($"[BANK] No item at bank slot {cs_move.from.slot}", LogLevel.Warning);
			return sc_move;
		}

		// Transfer to empty inventory slot (move entire stack)
		if (invItem is null)
		{
			bank.RemoveItem(cs_move.from.slot);
			inventory.AddItem(bankItem, cs_move.to.slot);

			sc_move.move_list.Add(new()
			{
				fromCount = 0,
				toCount = bankItem.GetAmount(),
				fromSlot = cs_move.from,
				toSlot = cs_move.to
			});
		}
		// Stack with existing inventory item or swap
		else
		{
			if (ShouldStack(bankItem, invItem))
			{
				CreateStacks(bankItem, invItem);
				if (bankItem.GetAmount() == 0)
					bank.RemoveItem(cs_move.from.slot);
				else
					bank.UpdateItemAtSlot(cs_move.from.slot, bankItem);

				inventory.UpdateItemAtSlot(cs_move.to.slot, invItem);

				sc_move.move_list.Add(new()
				{
					fromCount = bankItem.GetAmount(),
					toCount = invItem.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});
			}
			else
			{
				// Swap items
				bank.RemoveItem(cs_move.from.slot);
				inventory.RemoveItem(cs_move.to.slot);
				bank.AddItem(invItem, cs_move.from.slot);
				inventory.AddItem(bankItem, cs_move.to.slot);

				sc_move.move_list.Add(new()
				{
					fromCount = invItem.GetAmount(),
					toCount = bankItem.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});

				sc_move.move_list.Add(new()
				{
					fromCount = bankItem.GetAmount(),
					toCount = invItem.GetAmount(),
					fromSlot = cs_move.to,
					toSlot = cs_move.from
				});
			}
		}

		Logger.Log($"[BANK] Moved item from bank slot {cs_move.from.slot} to inventory slot {cs_move.to.slot}", LogLevel.Debug);
		return sc_move;
	}

	/// <summary>
	/// Handles item movement within bank.
	/// </summary>
	private SC_MOVE_SLOT_ITEM HandleBankToBank(IPlayerBank bank, CS_MOVE_SLOT_ITEM cs_move)
	{
		SC_MOVE_SLOT_ITEM sc_move = new()
		{
			move_list = new()
		};

		if (!bank.IsOpen)
		{
			Logger.Log("[BANK] Attempted to move item within bank but bank is not open", LogLevel.Warning);
			return sc_move;
		}

		var item_slot1 = bank.AtSlot(cs_move.from.slot);
		var item_slot2 = bank.AtSlot(cs_move.to.slot);

		// Moving item to empty slot
		if (item_slot2 is null && item_slot1 is not null)
		{
			bank.RemoveItem(cs_move.from.slot);
			bank.AddItem(item_slot1, cs_move.to.slot);

			sc_move.move_list.Add(new()
			{
				fromCount = 0,
				toCount = item_slot1.GetAmount(),
				fromSlot = cs_move.from,
				toSlot = cs_move.to
			});
		}
		// Moving item to slot where there already is an item
		else if (item_slot1 is not null && item_slot2 is not null)
		{
			if (ShouldStack(item_slot1, item_slot2))
			{
				CreateStacks(item_slot1, item_slot2);
				if (item_slot1.GetAmount() == 0)
					bank.RemoveItem(cs_move.from.slot);
				else
					bank.UpdateItemAtSlot(cs_move.from.slot, item_slot1);

				bank.UpdateItemAtSlot(cs_move.to.slot, item_slot2);

				sc_move.move_list.Add(new()
				{
					fromCount = item_slot1.GetAmount(),
					toCount = item_slot2.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});
			}
			else
			{
				// Swap items
				bank.RemoveItem(cs_move.to.slot);
				bank.RemoveItem(cs_move.from.slot);
				bank.AddItem(item_slot1, cs_move.to.slot);
				bank.AddItem(item_slot2, cs_move.from.slot);

				sc_move.move_list.Add(new()
				{
					fromCount = item_slot1.GetAmount(),
					toCount = item_slot1.GetAmount(),
					fromSlot = cs_move.from,
					toSlot = cs_move.to
				});

				sc_move.move_list.Add(new()
				{
					fromCount = item_slot2.GetAmount(),
					toCount = item_slot2.GetAmount(),
					fromSlot = cs_move.to,
					toSlot = cs_move.from
				});
			}
		}

		Logger.Log($"[BANK] Moved item within bank from slot {cs_move.from.slot} to slot {cs_move.to.slot}", LogLevel.Debug);
		return sc_move;
	}
}
