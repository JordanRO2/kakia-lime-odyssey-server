using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Post;

[PacketHandlerAttr(PacketType.CS_REQUEST_DELETE_POST)]
class CS_REQUEST_DELETE_POST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<PACKET_CS_REQUEST_DELETE_POST>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[POST] {playerName} requesting to delete mail index {packet.indexNumber}", LogLevel.Debug);

		if (LimeServer.PostService.DeletePost(pc, packet.indexNumber))
		{
			LimeServer.PostService.SendDeletedPost(pc, packet.indexNumber);
			Logger.Log($"[POST] {playerName} deleted mail index {packet.indexNumber}", LogLevel.Debug);
		}
		else
		{
			Logger.Log($"[POST] {playerName} failed to delete mail index {packet.indexNumber}", LogLevel.Debug);
		}
	}
}
