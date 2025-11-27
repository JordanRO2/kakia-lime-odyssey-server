using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Post;

[PacketHandlerAttr(PacketType.CS_REQUEST_POST)]
class CS_REQUEST_POST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<PACKET_CS_REQUEST_POST>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[POST] {playerName} requesting mail details for index {packet.indexNumber}", LogLevel.Debug);

		LimeServer.PostService.SendPostDetails(pc, packet.indexNumber);
	}
}
