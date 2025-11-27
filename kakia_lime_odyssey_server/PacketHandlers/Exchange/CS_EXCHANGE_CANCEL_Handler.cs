using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Exchange;

[PacketHandlerAttr(PacketType.CS_EXCHANGE_CANCEL)]
class CS_EXCHANGE_CANCEL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXCHANGE] {playerName} cancelled exchange", LogLevel.Debug);

		LimeServer.ExchangeService.CancelExchange(pc);
	}
}
