/// <summary>
/// Handles CS_REQUEST_TRADE packet - open NPC trade window.
/// </summary>
/// <remarks>
/// Triggered by: Player requesting to trade with currently selected NPC
/// Response packets: SC_TRADE_DESC
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Trade;

[PacketHandlerAttr(PacketType.CS_REQUEST_TRADE)]
class CS_REQUEST_TRADE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} requesting NPC trade", LogLevel.Debug);

		// TODO: Get currently selected NPC
		// TODO: Validate NPC is a merchant
		// TODO: Send SC_TRADE_DESC with NPC's shop items
	}
}
