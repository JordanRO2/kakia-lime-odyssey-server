/// <summary>
/// Handles CS_CANCEL_INVENTORY_COMPOSE_ITEM packet - cancel item composition.
/// </summary>
/// <remarks>
/// Triggered by: Player closing item composition interface
/// Response packets: None
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_CANCEL_INVENTORY_COMPOSE_ITEM)]
class CS_CANCEL_INVENTORY_COMPOSE_ITEM_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[COMPOSE] {playerName} canceling composition", LogLevel.Debug);

		LimeServer.ItemService.CancelComposition(pc);
	}
}
