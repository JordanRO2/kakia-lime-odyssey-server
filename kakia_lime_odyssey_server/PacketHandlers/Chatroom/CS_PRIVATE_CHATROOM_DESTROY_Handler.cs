using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Chatroom;

[PacketHandlerAttr(PacketType.CS_PRIVATE_CHATROOM_DESTROY)]
class CS_PRIVATE_CHATROOM_DESTROY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHATROOM] {playerName} destroying chatroom", LogLevel.Debug);

		LimeServer.ChatroomService.DestroyChatroom(pc);
	}
}
