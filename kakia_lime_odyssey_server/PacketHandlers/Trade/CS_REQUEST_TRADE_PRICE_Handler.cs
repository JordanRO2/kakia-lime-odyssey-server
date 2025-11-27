/// <summary>
/// Handles CS_REQUEST_TRADE_PRICE packet - request item sell price.
/// </summary>
/// <remarks>
/// Triggered by: Player checking sell price at NPC merchant
/// Response packets: SC_TRADE_PRICE
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Trade;

[PacketHandlerAttr(PacketType.CS_REQUEST_TRADE_PRICE)]
class CS_REQUEST_TRADE_PRICE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_REQUEST_TRADE_PRICE>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] {playerName} requesting price for slot {packet.slot}", LogLevel.Debug);

		// TODO: Get item from inventory slot
		// TODO: Calculate sell price
		// TODO: Send SC_TRADE_PRICE
	}
}
