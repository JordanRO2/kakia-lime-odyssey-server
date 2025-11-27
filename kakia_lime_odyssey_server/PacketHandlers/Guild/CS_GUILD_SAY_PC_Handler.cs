using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Guild;

[PacketHandlerAttr(PacketType.CS_GUILD_SAY_PC)]
class CS_GUILD_SAY_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_GUILD_SAY_PC>();

		// Read variable-length message
		int messageLength = p.Size - 8; // Size minus the fixed fields (maintainTime + type)
		string message = string.Empty;
		if (messageLength > 0)
		{
			byte[] messageBytes = pr.ReadBytes(messageLength);
			message = Encoding.ASCII.GetString(messageBytes).TrimEnd('\0');
		}

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (string.IsNullOrWhiteSpace(message))
		{
			Logger.Log($"[GUILD CHAT] Empty message from {playerName}", LogLevel.Debug);
			return;
		}

		Logger.Log($"[GUILD CHAT] {playerName}: {message}", LogLevel.Debug);

		LimeServer.GuildService.SendGuildChat(pc, message, packet.maintainTime, packet.type);
	}
}
