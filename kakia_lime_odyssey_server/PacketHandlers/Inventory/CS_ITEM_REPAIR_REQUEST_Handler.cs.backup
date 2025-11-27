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
			item = equipment.GetEquipped(slot) as Item;
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
			// Deduct currency and restore durability when item durability system is implemented
			// For now, repair is always successful (no currency cost)
			success = true;
			Logger.Log($"[REPAIR] {playerName} repaired item {item.Name} successfully", LogLevel.Information);
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
}
