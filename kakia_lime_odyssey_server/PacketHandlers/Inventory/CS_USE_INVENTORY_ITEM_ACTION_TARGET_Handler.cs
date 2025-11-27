/// <summary>
/// Handles CS_USE_INVENTORY_ITEM_ACTION_TARGET packet - use item on target.
/// </summary>
/// <remarks>
/// Triggered by: Player using an item that requires a target
/// Response packets: Item-specific response
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_USE_INVENTORY_ITEM_ACTION_TARGET)]
class CS_USE_INVENTORY_ITEM_ACTION_TARGET_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_USE_INVENTORY_ITEM_ACTION_TARGET>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[ITEM] {playerName} using item at slot {packet.slot} on target", LogLevel.Debug);

		LimeServer.ItemService.UseItemOnCurrentTarget(pc, packet.slot);
	}
}
