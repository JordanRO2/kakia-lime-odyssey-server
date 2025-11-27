using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Guild;

[PacketHandlerAttr(PacketType.CS_GUILD_REQUEST_JOIN)]
class CS_GUILD_REQUEST_JOIN_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_GUILD_REQUEST_JOIN>();

		string guildName = Encoding.ASCII.GetString(packet.name).TrimEnd('\0');
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (string.IsNullOrWhiteSpace(guildName))
		{
			Logger.Log($"[GUILD] {playerName} tried to request join with empty guild name", LogLevel.Debug);
			return;
		}

		Logger.Log($"[GUILD] {playerName} requesting to join guild '{guildName}'", LogLevel.Debug);

		// Note: This requires guild request system implementation
		// For now, log and ignore
		Logger.Log($"[GUILD] Guild join requests not yet implemented", LogLevel.Warning);
	}
}
