/// <summary>
/// Handles CS_NOTICE packet - server notice/announcement.
/// </summary>
/// <remarks>
/// Triggered by: GM/Admin sending notice
/// Response packets: SC_NOTICE (broadcast to all)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Social;

[PacketHandlerAttr(PacketType.CS_NOTICE)]
class CS_NOTICE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		string message = pr.ReadRemaining();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[NOTICE] {playerName} sent notice: {message}", LogLevel.Information);

		LimeServer.ChatroomService.SendServerNotice(pc, message);
	}
}
