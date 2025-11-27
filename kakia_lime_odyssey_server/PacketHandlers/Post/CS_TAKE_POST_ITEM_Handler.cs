using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Post;

[PacketHandlerAttr(PacketType.CS_TAKE_POST_ITEM)]
class CS_TAKE_POST_ITEM_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<PACKET_CS_TAKE_POST_ITEM>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[POST] {playerName} taking items from mail index {packet.indexNumber}", LogLevel.Debug);

		if (LimeServer.PostService.TakePostItems(pc, packet.indexNumber))
		{
			LimeServer.PostService.SendPostItem(pc, packet.indexNumber);
			Logger.Log($"[POST] {playerName} took items from mail index {packet.indexNumber}", LogLevel.Debug);
		}
		else
		{
			Logger.Log($"[POST] {playerName} failed to take items from mail index {packet.indexNumber}", LogLevel.Debug);
		}
	}
}
