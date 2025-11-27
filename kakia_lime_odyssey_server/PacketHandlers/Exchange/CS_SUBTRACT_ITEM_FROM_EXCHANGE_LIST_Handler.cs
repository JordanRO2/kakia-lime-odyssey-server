using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Exchange;

[PacketHandlerAttr(PacketType.CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST)]
class CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<PACKET_CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXCHANGE] {playerName} removing item from slot {packet.slot} (count: {packet.count})", LogLevel.Debug);

		LimeServer.ExchangeService.SubtractItem(pc, packet.slot, packet.count);
	}
}
