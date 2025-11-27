using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Guild;

namespace kakia_lime_odyssey_server.PacketHandlers.Guild;

[PacketHandlerAttr(PacketType.CS_GUILD_CHANGE_OPTION)]
class CS_GUILD_CHANGE_OPTION_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_GUILD_CHANGE_OPTION>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[GUILD] {playerName} changing guild option to {packet.type}", LogLevel.Debug);

		bool success = LimeServer.GuildService.ChangeOption(pc, (GuildOption)packet.type);

		if (!success)
		{
			Logger.Log($"[GUILD] {playerName} failed to change option (not enough permission)", LogLevel.Debug);
		}
	}
}
