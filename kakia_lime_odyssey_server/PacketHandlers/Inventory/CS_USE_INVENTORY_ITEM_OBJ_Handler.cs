/// <summary>
/// Handles CS_USE_INVENTORY_ITEM_OBJ packet - player uses item on a target object.
/// </summary>
/// <remarks>
/// Triggered by: Player using an item on an NPC, pet, or other entity
/// Response packets: SC_USE_ITEM_OBJ_RESULT_LIST
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

[PacketHandlerAttr(PacketType.CS_USE_INVENTORY_ITEM_OBJ)]
class CS_USE_INVENTORY_ITEM_OBJ_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<PACKET_CS_USE_INVENTORY_ITEM_OBJ>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[ITEM] {playerName} using item at slot {packet.slot} on object {packet.targetInstID}", LogLevel.Debug);

		LimeServer.ItemService.UseItemOnObject(pc, packet.slot, packet.targetInstID);
	}
}
