using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_CREATE)]
class CS_PARTY_CREATE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_PARTY_CREATE>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[PARTY] {playerName} creating party '{packet.name}'", LogLevel.Debug);

		var result = LimeServer.PartyService.CreateParty(pc, packet.name);

		if (!result.Success)
		{
			Logger.Log($"[PARTY] Failed to create party: {result.Error} - {result.Message}", LogLevel.Debug);
		}
	}
}
