/// <summary>
/// Handles CS_TRADE_END packet - player closes merchant window.
/// </summary>
/// <remarks>
/// Triggered by: Player closing the merchant trading window
/// Response packets: SC_TRADE_END
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Trade;

[PacketHandlerAttr(PacketType.CS_TRADE_END)]
class CS_TRADE_END_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} ending trade session", LogLevel.Debug);

		LimeServer.TradeService.EndTrade(pc);
	}
}
