using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Chatroom;

[PacketHandlerAttr(PacketType.CS_PRIVATE_CHATROOM_ENTER)]
class CS_PRIVATE_CHATROOM_ENTER_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_PRIVATE_CHATROOM_ENTER>();

		string password = Encoding.ASCII.GetString(packet.password).TrimEnd('\0');

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHATROOM] {playerName} entering chatroom {packet.instID}", LogLevel.Debug);

		LimeServer.ChatroomService.EnterChatroom(pc, packet.instID, password);
	}
}
