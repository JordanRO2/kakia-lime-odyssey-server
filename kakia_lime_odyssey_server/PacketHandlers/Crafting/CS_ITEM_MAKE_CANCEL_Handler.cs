/// <summary>
/// Handles CS_ITEM_MAKE_CANCEL packet - player cancels crafting.
/// </summary>
/// <remarks>
/// Triggered by: Player canceling crafting process
/// Response packets: SC_ITEM_MAKE_FINISH
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_ITEM_MAKE_CANCEL)]
class CS_ITEM_MAKE_CANCEL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} canceling crafting", LogLevel.Debug);

		// TODO: Cancel crafting timer
		// TODO: Return materials (if applicable)
		// TODO: Send SC_ITEM_MAKE_FINISH

		Logger.Log($"[CRAFT] {playerName} crafting canceled", LogLevel.Debug);
	}
}
