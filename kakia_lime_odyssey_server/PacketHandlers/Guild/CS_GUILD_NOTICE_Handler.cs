using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Guild;

[PacketHandlerAttr(PacketType.CS_GUILD_NOTICE)]
class CS_GUILD_NOTICE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_GUILD_NOTICE>(p.Payload);

		string notice = Encoding.ASCII.GetString(packet.notice).TrimEnd('\0');
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		Logger.Log($"[GUILD] {playerName} setting guild notice", LogLevel.Debug);

		bool success = LimeServer.GuildService.SetNotice(pc, notice);

		if (!success)
		{
			Logger.Log($"[GUILD] {playerName} failed to set notice (not enough permission)", LogLevel.Debug);
		}
	}
}
