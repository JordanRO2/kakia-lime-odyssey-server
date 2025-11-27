/// <summary>
/// Handles CS_TRADE_BUY_SOLD_ITEMS packet - buyback previously sold items.
/// </summary>
/// <remarks>
/// Triggered by: Player buying back items from NPC merchant
/// Response packets: SC_TRADE_BOUGHT_SOLD_ITEMS
/// Database: inventory (add item back)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Trade;

[PacketHandlerAttr(PacketType.CS_TRADE_BUY_SOLD_ITEMS)]
class CS_TRADE_BUY_SOLD_ITEMS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} buying back sold items", LogLevel.Debug);

		LimeServer.TradeService.BuyBackSoldItems(pc);
	}
}
