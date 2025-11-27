/// <summary>
/// Handles CS_EQUIPPED_ITEM_REPAIR_PRICE packet - request repair price for equipped items.
/// </summary>
/// <remarks>
/// Triggered by: Player requesting repair cost at NPC
/// Response packets: SC_EQUIPPED_ITEM_REPAIR_PRICE
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_EQUIPPED_ITEM_REPAIR_PRICE)]
class CS_EQUIPPED_ITEM_REPAIR_PRICE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[REPAIR] {playerName} requesting equipped item repair price", LogLevel.Debug);

		uint price = LimeServer.ItemService.GetEquippedItemsRepairPrice(pc);

		// Send SC_EQUIPPED_ITEM_REPAIR_PRICE with total cost
		using PacketWriter pw = new();
		pw.Writer.Write((ushort)PacketType.SC_EQUIPPED_ITEM_REPAIR_PRICE);
		pw.Writer.Write(price);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}
}
