/// <summary>
/// Handles CS_SELECT_TARGET_REQUEST_TRADE packet - select NPC and open trade.
/// </summary>
/// <remarks>
/// Triggered by: Player selecting NPC and requesting trade
/// Response packets: SC_TRADE_DESC
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Trade;

[PacketHandlerAttr(PacketType.CS_SELECT_TARGET_REQUEST_TRADE)]
class CS_SELECT_TARGET_REQUEST_TRADE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_SELECT_TARGET_REQUEST_TRADE>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} requesting trade with NPC {packet.objInstID}", LogLevel.Debug);

		LimeServer.TradeService.OpenTradeWithNpc(pc, packet.objInstID);
	}
}
