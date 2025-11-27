using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_SAY_PC)]
class CS_PARTY_SAY_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_PARTY_SAY_PC>(p.Payload);

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
			Logger.Log($"[PARTY CHAT] Empty message from {playerName}", LogLevel.Debug);
			return;
		}

		Logger.Log($"[PARTY CHAT] {playerName}: {message}", LogLevel.Debug);

		LimeServer.PartyService.SendPartyChat(pc, message, packet.maintainTime, packet.type);
	}
}
