/// <summary>
/// Handles CS_USE_INVENTORY_ITEM_SLOT packet - player uses item on another inventory slot.
/// </summary>
/// <remarks>
/// Triggered by: Player using an item on another item (combining, upgrading, etc.)
/// Response packets: SC_USE_ITEM_SLOT_RESULT
/// Database: inventory (update)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_USE_INVENTORY_ITEM_SLOT)]
class CS_USE_INVENTORY_ITEM_SLOT_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<PACKET_CS_USE_INVENTORY_ITEM_SLOT>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[ITEM] {playerName} using item at slot {packet.slot} on slot {packet.targetSlot}", LogLevel.Debug);

		LimeServer.ItemService.UseItemOnSlot(pc, packet.slot, packet.targetSlot);
	}
}
