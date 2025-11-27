/// <summary>
/// Handles CS_STUFF_MAKE_CANCEL packet - cancel material gathering/processing.
/// </summary>
/// <remarks>
/// Triggered by: Player canceling gathering/processing action
/// Response packets: SC_STUFF_MAKE_FINISH
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_STUFF_MAKE_CANCEL)]
class CS_STUFF_MAKE_CANCEL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} canceling stuff make", LogLevel.Debug);

		// TODO: Cancel gathering/processing timer
		// TODO: Send SC_STUFF_MAKE_FINISH
	}
}
