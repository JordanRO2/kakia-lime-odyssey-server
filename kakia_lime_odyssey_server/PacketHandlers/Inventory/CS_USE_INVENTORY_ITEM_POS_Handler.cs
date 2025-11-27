/// <summary>
/// Handles CS_USE_INVENTORY_ITEM_POS packet - player uses item at a world position.
/// </summary>
/// <remarks>
/// Triggered by: Player using a position-targeted item (trap, AoE, etc.)
/// Response packets: SC_USE_ITEM_POS_RESULT_LIST
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

[PacketHandlerAttr(PacketType.CS_USE_INVENTORY_ITEM_POS)]
class CS_USE_INVENTORY_ITEM_POS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<PACKET_CS_USE_INVENTORY_ITEM_POS>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[ITEM] {playerName} using item at slot {packet.slot} at position ({packet.pos.x}, {packet.pos.y}, {packet.pos.z})", LogLevel.Debug);

		LimeServer.ItemService.UseItemAtPosition(pc, packet.slot, packet.pos);
	}
}
