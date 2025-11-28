/// <summary>
/// Handles CS_PARTY_JOIN packet - player accepting party invitation or join request.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking accept on party invite/join request dialog
/// Response packets: SC_PARTY_JOINED, SC_PARTY_MEMBER_ADDED
/// This handler supports two scenarios:
/// 1. Player accepting an invitation to join someone else's party
/// 2. Party leader accepting a join request from another player
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Party;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_JOIN)]
class CS_PARTY_JOIN_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// First try accepting a pending invitation (player was invited to join a party)
		var result = LimeServer.PartyService.AcceptInvitation(pc);

		if (result.Success)
		{
			Logger.Log($"[PARTY] {playerName} accepted party invitation", LogLevel.Debug);
			return;
		}

		// If no pending invitation, try accepting a pending join request (leader approving a player)
		if (result.Error == PartyError.NoPendingInvitation)
		{
			result = LimeServer.PartyService.AcceptJoinRequest(pc);

			if (result.Success)
			{
				Logger.Log($"[PARTY] {playerName} approved join request", LogLevel.Debug);
				return;
			}
		}

		Logger.Log($"[PARTY] Failed to process join/accept: {result.Error} - {result.Message}", LogLevel.Debug);
	}
}
