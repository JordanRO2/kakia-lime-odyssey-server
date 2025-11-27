/// <summary>
/// Handles CS_REQUEST_EXCHANGE packet - request player-to-player exchange.
/// </summary>
/// <remarks>
/// Triggered by: Player requesting to trade with another player
/// Response packets: SC_REQUEST_EXCHANGE sent to target
/// Note: Different from NPC trade - this is P2P
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Exchange;

[PacketHandlerAttr(PacketType.CS_REQUEST_EXCHANGE)]
class CS_REQUEST_EXCHANGE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXCHANGE] {playerName} requesting P2P exchange", LogLevel.Debug);

		// TODO: Get current target player
		// TODO: Send exchange request to target
		// TODO: Wait for accept/reject
	}
}
