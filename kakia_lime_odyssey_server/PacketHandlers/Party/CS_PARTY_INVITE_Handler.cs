using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_INVITE)]
class CS_PARTY_INVITE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_PARTY_INVITE>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[PARTY] {playerName} inviting '{packet.name}' to party", LogLevel.Debug);

		var result = LimeServer.PartyService.InvitePlayer(pc, packet.name);

		if (!result.Success)
		{
			Logger.Log($"[PARTY] Failed to invite player: {result.Error} - {result.Message}", LogLevel.Debug);
		}
	}
}
