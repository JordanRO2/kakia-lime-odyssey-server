/// <summary>
/// Handles CS_TRADE_BUY packet - player buys item from NPC merchant.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking buy in merchant window
/// Response packets: SC_TRADE_BOUGHT_SOLD_ITEMS
/// Database: inventory (update)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Trade;

[PacketHandlerAttr(PacketType.CS_TRADE_BUY)]
class CS_TRADE_BUY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_TRADE_BUY>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} buying item {packet.itemTypeID} x{packet.count} to slot {packet.slot}", LogLevel.Debug);

		LimeServer.TradeService.BuyItem(pc, packet.itemTypeID, packet.count, packet.slot);
	}
}
