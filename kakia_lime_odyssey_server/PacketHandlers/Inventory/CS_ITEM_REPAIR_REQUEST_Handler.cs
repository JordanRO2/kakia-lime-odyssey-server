/// <summary>
/// Handles CS_ITEM_REPAIR_REQUEST packet - player requests to repair an item.
/// </summary>
/// <remarks>
/// Triggered by: Player confirming repair at NPC blacksmith
/// Response packets: SC_ITEM_REPAIR_RESULT
/// Database: inventory/equipment (update durability)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_ITEM_REPAIR_REQUEST)]
class CS_ITEM_REPAIR_REQUEST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var request = PacketConverter.Extract<CS_ITEM_REPAIR_REQUEST>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[REPAIR] {playerName} requesting repair for slot {request.index} (equipped: {request.isEquiped})", LogLevel.Debug);

		Item? item = null;

		if (request.isEquiped)
		{
			// Get from equipment
			var equipment = pc.GetEquipment(true);
			var slot = (EQUIP_SLOT)request.index;
			item = equipment.GetItemInSlot(slot) as Item;
		}
		else
		{
			// Get from inventory
			var inventory = pc.GetInventory();
			item = inventory.AtSlot(request.index) as Item;
		}

		bool success = false;
		if (item != null)
		{
			// Calculate repair cost based on missing durability
			int missingDurability = item.GetMaxDurability() - item.GetDurability();
			if (missingDurability <= 0)
			{
				// Item doesn't need repair
				Logger.Log($"[REPAIR] {playerName} item {item.Name} doesn't need repair", LogLevel.Debug);
				success = true;
			}
			else
			{
				// Repair cost: 1 Peder per durability point (can be adjusted)
				long repairCost = missingDurability;
				if (LimeServer.CurrencyService.ProcessRepair(pc, repairCost))
				{
					// Repair successful - restore durability
					item.RepairFully();
					success = true;

					Logger.Log($"[REPAIR] {playerName} repaired item {item.Name} for {repairCost} Peder", LogLevel.Information);

					// Send durability update
					if (request.isEquiped)
					{
						SendEquipDurabilityUpdate(pc, (byte)request.index, item);
					}
					else
					{
						SendInventoryDurabilityUpdate(pc, request.index, item);
					}
				}
				else
				{
					Logger.Log($"[REPAIR] {playerName} cannot afford repair cost for {item.Name}", LogLevel.Debug);
				}
			}
		}
		else
		{
			Logger.Log($"[REPAIR] {playerName} failed to repair - item not found at slot {request.index}", LogLevel.Warning);
		}

		// Send repair result response
		SC_ITEM_REPAIR_RESULT response = new()
		{
			isSuccess = success
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Sends durability update for an inventory item.
	/// </summary>
	private static void SendInventoryDurabilityUpdate(PlayerClient pc, int slot, Item item)
	{
		var packet = new SC_UPDATE_DURABILITY_INVENTORY_ITEM
		{
			slot = slot,
			durability = item.GetDurability(),
			mdurability = item.GetMaxDurability()
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Sends durability update for an equipped item.
	/// </summary>
	private static void SendEquipDurabilityUpdate(PlayerClient pc, byte equipSlot, Item item)
	{
		var packet = new SC_UPDATE_DURABILITY_EQUIPPED_ITEM
		{
			equipSlot = equipSlot,
			durability = item.GetDurability(),
			mdurability = item.GetMaxDurability()
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToPacket(), default).Wait();
	}
}
