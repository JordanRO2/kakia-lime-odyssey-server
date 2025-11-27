/// <summary>
/// Handles CS_KNIGHTS_SAY_PC packet - player sends a guild (knights) chat message.
/// </summary>
/// <remarks>
/// Triggered by: Player using guild/knights chat channel
/// Response packets: SC_KNIGHTS_SAY (broadcast to all guild members)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Social;

[PacketHandlerAttr(PacketType.CS_KNIGHTS_SAY_PC)]
class CS_KNIGHTS_SAY_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var chat = pr.Read<CS_KNIGHTS_SAY_PC>();
		string message = pr.ReadRemaining();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHAT:KNIGHTS] {playerName}: {message}", LogLevel.Debug);

		LimeServer.GuildService.SendGuildChat(pc, message, chat.maintainTime, chat.type);
	}
}
