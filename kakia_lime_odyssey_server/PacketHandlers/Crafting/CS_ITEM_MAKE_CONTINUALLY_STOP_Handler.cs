/// <summary>
/// Handles CS_ITEM_MAKE_CONTINUALLY_STOP packet - player stops continuous crafting.
/// </summary>
/// <remarks>
/// Triggered by: Player stopping continuous crafting
/// Response packets: SC_ITEM_MAKE_FINISH
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_ITEM_MAKE_CONTINUALLY_STOP)]
class CS_ITEM_MAKE_CONTINUALLY_STOP_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} stopping continuous crafting", LogLevel.Debug);

		// TODO: Stop crafting loop
		// TODO: Send SC_ITEM_MAKE_FINISH with current progress

		Logger.Log($"[CRAFT] {playerName} continuous crafting stopped", LogLevel.Debug);
	}
}
