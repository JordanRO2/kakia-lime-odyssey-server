using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Guild;

[PacketHandlerAttr(PacketType.CS_GUILD_SECEDE)]
class CS_GUILD_SECEDE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[GUILD] {playerName} leaving guild", LogLevel.Debug);

		bool success = LimeServer.GuildService.LeaveGuild(pc);

		if (!success)
		{
			Logger.Log($"[GUILD] {playerName} failed to leave guild (is leader)", LogLevel.Debug);
		}
	}
}
