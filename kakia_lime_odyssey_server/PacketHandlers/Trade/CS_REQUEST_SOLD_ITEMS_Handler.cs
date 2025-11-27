/// <summary>
/// Handles CS_REQUEST_SOLD_ITEMS packet - request list of sold items.
/// </summary>
/// <remarks>
/// Triggered by: Player opening buyback tab at NPC merchant
/// Response packets: SC_SOLD_ITEM_LIST
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Trade;

[PacketHandlerAttr(PacketType.CS_REQUEST_SOLD_ITEMS)]
class CS_REQUEST_SOLD_ITEMS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} requesting sold items list", LogLevel.Debug);

		// TODO: Get list of recently sold items for this player
		// TODO: Send SC_SOLD_ITEM_LIST
	}
}
