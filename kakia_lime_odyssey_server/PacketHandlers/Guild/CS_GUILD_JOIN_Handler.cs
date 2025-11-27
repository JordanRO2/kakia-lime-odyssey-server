using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Guild;

[PacketHandlerAttr(PacketType.CS_GUILD_JOIN)]
class CS_GUILD_JOIN_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[GUILD] {playerName} accepting guild invitation", LogLevel.Debug);

		var result = LimeServer.GuildService.AcceptInvitation(pc);

		if (!result.Success)
		{
			Logger.Log($"[GUILD] {playerName} failed to join guild: {result.Error} - {result.Message}", LogLevel.Debug);
		}
	}
}
