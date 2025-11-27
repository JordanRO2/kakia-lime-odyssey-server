/// <summary>
/// Handles CS_REQUEST_START_LOOTING packet - player requests to start looting current target.
/// </summary>
/// <remarks>
/// Triggered by: Player attempting to loot with target already selected
/// Response packets: SC_LOOTABLE_ITEM_LIST
/// Note: Similar to CS_SELECT_TARGET_REQUEST_START_LOOTING but target is already selected
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_REQUEST_START_LOOTING)]
class CS_REQUEST_START_LOOTING_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var monsterId = pc.GetCurrentTarget();
		if (monsterId == 0)
		{
			Logger.Log("[LOOT] No target selected for looting", LogLevel.Debug);
			return;
		}

		if (!LimeServer.TryGetEntity(monsterId, out var mob) || mob == null)
		{
			Logger.Log($"[LOOT] Target entity {monsterId} not found", LogLevel.Debug);
			return;
		}

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[LOOT] {playerName} starting to loot entity {monsterId}", LogLevel.Debug);

		List<INVENTORY_ITEM> items = new();

		var loot = mob.GetLoot();
		for (int i = 0; i < loot.Count; i++)
			items.Add(loot[i].AsInventoryItem(loot[i].Id));

		SC_LOOTABLE_ITEM_LIST sc_lootable_item_list = new()
		{
			count = (ushort)items.Count,
			lootTable = items.ToArray()
		};

		using PacketWriter pw = new();
		pw.Write(sc_lootable_item_list);
		pc.Send(pw.ToSizedPacket(), default).Wait();

		Logger.Log($"[LOOT] Sent {items.Count} lootable items to {playerName}", LogLevel.Debug);
	}
}
