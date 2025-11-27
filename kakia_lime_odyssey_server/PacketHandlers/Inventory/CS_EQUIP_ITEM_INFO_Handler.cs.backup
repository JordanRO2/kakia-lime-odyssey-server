/// <summary>
/// Handles CS_EQUIP_ITEM_INFO packet - player requests detailed info about equipped item.
/// </summary>
/// <remarks>
/// Triggered by: Player inspecting an equipped item
/// Response packets: SC_EQUIP_ITEM_INFO
/// Database: None directly
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
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_EQUIP_ITEM_INFO)]
class CS_EQUIP_ITEM_INFO_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var request = PacketConverter.Extract<CS_EQUIP_ITEM_INFO>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[INVENTORY] {playerName} requesting info for equipment slot {request.equipSlot}", LogLevel.Debug);

		// Get the equipment for current job
		var equipment = pc.GetEquipment(true);
		var slot = (EQUIP_SLOT)request.equipSlot;

		// Get the item at the specified slot
		var item = equipment.GetEquipped(slot) as Item;
		if (item is null)
		{
			Logger.Log($"[INVENTORY] No item equipped at slot {request.equipSlot} for {playerName}", LogLevel.Debug);
			// Send empty response
			SC_EQUIP_ITEM_INFO emptyResponse = new()
			{
				equipSlot = request.equipSlot,
				typeID = 0,
				durability = 0,
				mdurability = 0,
				grade = 0,
				inherits = new ITEM_INHERITS()
			};

			using PacketWriter pwEmpty = new();
			pwEmpty.Write(emptyResponse);
			pc.Send(pwEmpty.ToPacket(), default).Wait();
			return;
		}

		// Build response with item details
		SC_EQUIP_ITEM_INFO response = new()
		{
			equipSlot = request.equipSlot,
			typeID = item.Id,
			durability = item.Durable,  // Current durability
			mdurability = item.Durable, // Max durability (same as current for now)
			grade = item.Grade,
			inherits = item.GetInherits()
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();

		Logger.Log($"[INVENTORY] Sent item info for {item.Id} at slot {request.equipSlot} to {playerName}", LogLevel.Debug);
	}
}
