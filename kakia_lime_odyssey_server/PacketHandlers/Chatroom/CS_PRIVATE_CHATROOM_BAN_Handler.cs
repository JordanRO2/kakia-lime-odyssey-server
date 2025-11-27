using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Chatroom;

[PacketHandlerAttr(PacketType.CS_PRIVATE_CHATROOM_BAN)]
class CS_PRIVATE_CHATROOM_BAN_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_PRIVATE_CHATROOM_BAN>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHATROOM] {playerName} banning member {packet.instID}", LogLevel.Debug);

		LimeServer.ChatroomService.BanMember(pc, packet.instID);
	}
}
