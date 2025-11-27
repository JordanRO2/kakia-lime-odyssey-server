/// <summary>
/// Handles CS_STUFF_MAKE_DELETE_LIST packet - remove item from material processing queue.
/// </summary>
/// <remarks>
/// Triggered by: Player removing material from processing queue
/// Response packets: SC_STUFF_MAKE_DELETE_LIST_SUCCESS
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_STUFF_MAKE_DELETE_LIST)]
class CS_STUFF_MAKE_DELETE_LIST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_STUFF_MAKE_DELETE_LIST>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} removing item from stuff make list", LogLevel.Debug);

		LimeServer.CraftingService.RemoveFromStuffMakeList(pc, packet.deleteItem.slot);
	}
}
