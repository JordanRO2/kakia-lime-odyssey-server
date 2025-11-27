/// <summary>
/// Handles CS_PARTY_REQUEST_JOIN packet - player requests to join another player's party.
/// </summary>
/// <remarks>
/// Triggered by: Player requesting to join a party
/// Response packets: Request sent to party leader
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_REQUEST_JOIN)]
class CS_PARTY_REQUEST_JOIN_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_PARTY_REQUEST_JOIN>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[PARTY] {playerName} requesting to join party of '{packet.name}'", LogLevel.Debug);

		var result = LimeServer.PartyService.RequestJoin(pc, packet.name);

		if (!result.Success)
		{
			Logger.Log($"[PARTY] Failed to request join: {result.Error} - {result.Message}", LogLevel.Debug);
		}
	}
}
