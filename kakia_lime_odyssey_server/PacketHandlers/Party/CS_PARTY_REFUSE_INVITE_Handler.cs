using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_REFUSE_INVITE)]
class CS_PARTY_REFUSE_INVITE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[PARTY] {playerName} declining party invitation", LogLevel.Debug);

		LimeServer.PartyService.DeclineInvitation(pc);
	}
}
