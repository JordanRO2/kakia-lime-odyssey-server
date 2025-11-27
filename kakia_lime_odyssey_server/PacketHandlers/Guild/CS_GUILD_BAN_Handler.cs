using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Guild;

[PacketHandlerAttr(PacketType.CS_GUILD_BAN)]
class CS_GUILD_BAN_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_GUILD_BAN>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[GUILD] {playerName} kicking member index {packet.idx}", LogLevel.Debug);

		bool success = LimeServer.GuildService.KickMember(pc, packet.idx);

		if (!success)
		{
			Logger.Log($"[GUILD] {playerName} failed to kick member (not enough permission or invalid index)", LogLevel.Debug);
		}
	}
}
