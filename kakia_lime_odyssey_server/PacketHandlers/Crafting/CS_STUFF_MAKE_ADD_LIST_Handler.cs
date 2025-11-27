/// <summary>
/// Handles CS_STUFF_MAKE_ADD_LIST packet - add item to material processing queue.
/// </summary>
/// <remarks>
/// Triggered by: Player adding material to processing queue
/// Response packets: SC_STUFF_MAKE_ADD_LIST_SUCCESS
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_STUFF_MAKE_ADD_LIST)]
class CS_STUFF_MAKE_ADD_LIST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_STUFF_MAKE_ADD_LIST>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} adding item to stuff make list", LogLevel.Debug);

		// TODO: Validate item exists in inventory
		// TODO: Add to processing queue
		// TODO: Send SC_STUFF_MAKE_ADD_LIST_SUCCESS
	}
}
