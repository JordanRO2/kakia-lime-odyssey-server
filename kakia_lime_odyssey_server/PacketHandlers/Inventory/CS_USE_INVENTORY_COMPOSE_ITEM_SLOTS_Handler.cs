/// <summary>
/// Handles CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS packet - execute item composition.
/// </summary>
/// <remarks>
/// Triggered by: Player confirming item composition with materials
/// Response packets: SC_INVENTORY_COMPOSE_ITEM_FINISH
/// Database: inventory (update item, consume materials)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS)]
class CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[COMPOSE] {playerName} executing composition on slot {packet.slot}", LogLevel.Debug);

		LimeServer.ItemService.ExecuteComposition(pc, packet.slot, packet.targetSlots.slots);
	}
}
