using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Exchange;

[PacketHandlerAttr(PacketType.CS_SELECT_TARGET_REQUEST_EXCHANGE)]
class CS_SELECT_TARGET_REQUEST_EXCHANGE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<PACKET_CS_SELECT_TARGET_REQUEST_EXCHANGE>(p.Payload);

		var target = LimeServer.PlayerClients.FirstOrDefault(x => x.GetId() == packet.objInstID);
		if (target == null)
		{
			Logger.Log($"[EXCHANGE] Target player not found: {packet.objInstID}", LogLevel.Debug);
			return;
		}

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		string targetName = target.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXCHANGE] {playerName} requesting exchange with {targetName}", LogLevel.Debug);

		LimeServer.ExchangeService.RequestExchange(pc, target);
	}
}
