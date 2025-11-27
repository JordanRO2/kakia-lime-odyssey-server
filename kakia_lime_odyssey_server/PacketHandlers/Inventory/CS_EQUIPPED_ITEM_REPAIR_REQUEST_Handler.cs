/// <summary>
/// Handles CS_EQUIPPED_ITEM_REPAIR_REQUEST packet - repair all equipped items.
/// </summary>
/// <remarks>
/// Triggered by: Player confirming repair at NPC
/// Response packets: SC_EQUIPPED_ITEM_REPAIR_RESULT
/// Database: inventory (update durability)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_EQUIPPED_ITEM_REPAIR_REQUEST)]
class CS_EQUIPPED_ITEM_REPAIR_REQUEST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[REPAIR] {playerName} requesting equipped item repair", LogLevel.Debug);

		// TODO: Calculate repair cost
		// TODO: Check player has enough gold
		// TODO: Deduct gold and repair all equipped items
		// TODO: Send SC_EQUIPPED_ITEM_REPAIR_RESULT
	}
}
