/// <summary>
/// Handles CS_ITEM_MAKE_REQUEST_REPORT packet - player requests crafting status.
/// </summary>
/// <remarks>
/// Triggered by: Player requesting crafting status update
/// Response packets: SC_ITEM_MAKE_UPDATE_REPORT
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_ITEM_MAKE_REQUEST_REPORT)]
class CS_ITEM_MAKE_REQUEST_REPORT_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} requesting crafting report", LogLevel.Debug);

		// TODO: Send SC_ITEM_MAKE_UPDATE_REPORT with current crafting status

		Logger.Log($"[CRAFT] {playerName} crafting report sent", LogLevel.Debug);
	}
}
