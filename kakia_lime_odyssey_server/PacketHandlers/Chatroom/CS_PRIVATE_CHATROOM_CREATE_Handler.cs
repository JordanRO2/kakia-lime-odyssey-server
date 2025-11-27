using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Chatroom;

[PacketHandlerAttr(PacketType.CS_PRIVATE_CHATROOM_CREATE)]
class CS_PRIVATE_CHATROOM_CREATE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_PRIVATE_CHATROOM_CREATE>(p.Payload);

		string name = Encoding.ASCII.GetString(packet.name).TrimEnd('\0');
		string password = Encoding.ASCII.GetString(packet.password).TrimEnd('\0');

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHATROOM] {playerName} creating chatroom '{name}'", LogLevel.Debug);

		LimeServer.ChatroomService.CreateChatroom(pc, name, password, packet.maxPersons, packet.type);
	}
}
