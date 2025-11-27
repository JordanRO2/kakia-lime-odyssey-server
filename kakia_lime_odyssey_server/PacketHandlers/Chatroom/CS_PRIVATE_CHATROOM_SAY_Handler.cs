using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Chatroom;

[PacketHandlerAttr(PacketType.CS_PRIVATE_CHATROOM_SAY)]
class CS_PRIVATE_CHATROOM_SAY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string message = Encoding.ASCII.GetString(p.Payload).TrimEnd('\0');

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHATROOM] {playerName}: {message}", LogLevel.Debug);

		LimeServer.ChatroomService.SendMessage(pc, message);
	}
}
