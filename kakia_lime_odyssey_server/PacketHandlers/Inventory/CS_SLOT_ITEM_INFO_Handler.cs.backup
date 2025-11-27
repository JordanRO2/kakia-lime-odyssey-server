/// <summary>
/// Handles CS_SLOT_ITEM_INFO packet - request detailed item info for a slot.
/// </summary>
/// <remarks>
/// Triggered by: Player hovering over or inspecting an item
/// Response packets: SC_SLOT_ITEM_INFO
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_SLOT_ITEM_INFO)]
class CS_SLOT_ITEM_INFO_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_SLOT_ITEM_INFO>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[ITEM] {playerName} requesting info for slot type {packet.slot.slotType} index {packet.slot.slotIdx}", LogLevel.Debug);

		LimeServer.ItemService.GetSlotItemInfo(pc, packet.slot.slotType, packet.slot.slotIdx);
	}
}
