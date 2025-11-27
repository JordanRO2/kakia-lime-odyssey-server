/// <summary>
/// Handles CS_TRADE_SELL packet - player sells item to NPC merchant.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking sell in merchant window
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

[PacketHandlerAttr(PacketType.CS_TRADE_SELL)]
class CS_TRADE_SELL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_TRADE_SELL>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} selling item from slot {packet.slot} x{packet.count}", LogLevel.Debug);

		LimeServer.TradeService.SellItem(pc, packet.slot, packet.count);
	}
}
