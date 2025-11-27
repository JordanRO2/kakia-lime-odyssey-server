/// <summary>
/// Handles CS_READY_INVENTORY_COMPOSE_ITEM packet - prepare item composition.
/// </summary>
/// <remarks>
/// Triggered by: Player opening item composition interface
/// Response packets: SC_READY_INVENTORY_COMPOSE_ITEM
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_READY_INVENTORY_COMPOSE_ITEM)]
class CS_READY_INVENTORY_COMPOSE_ITEM_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_READY_INVENTORY_COMPOSE_ITEM>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[COMPOSE] {playerName} preparing composition for slot {packet.slot}", LogLevel.Debug);

		// TODO: Validate item in slot can be composed
		// TODO: Send SC_READY_INVENTORY_COMPOSE_ITEM with composition info
	}
}
