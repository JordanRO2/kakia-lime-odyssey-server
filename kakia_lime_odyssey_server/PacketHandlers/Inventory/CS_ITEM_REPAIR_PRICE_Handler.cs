/// <summary>
/// Handles CS_ITEM_REPAIR_PRICE packet - player requests repair price for an item.
/// </summary>
/// <remarks>
/// Triggered by: Player inspecting repair cost at NPC blacksmith
/// Response packets: SC_ITEM_REPAIR_PRICE
/// Database: None directly
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Inventory;

[PacketHandlerAttr(PacketType.CS_ITEM_REPAIR_PRICE)]
class CS_ITEM_REPAIR_PRICE_Handler : PacketHandler
{
	// Base repair cost multiplier
	private const float RepairCostMultiplier = 0.1f;

	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var request = PacketConverter.Extract<CS_ITEM_REPAIR_PRICE>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[REPAIR] {playerName} requesting repair price for slot {request.slot} (equipped: {request.isEquiped})", LogLevel.Debug);

		Item? item = null;

		if (request.isEquiped)
		{
			// Get from equipment
			var equipment = pc.GetEquipment(true);
			var slot = (EQUIP_SLOT)request.slot;
			item = equipment.GetEquipped(slot) as Item;
		}
		else
		{
			// Get from inventory
			var inventory = pc.GetInventory();
			item = inventory.AtSlot(request.slot) as Item;
		}

		uint price = 0;
		if (item != null)
		{
			price = CalculateRepairPrice(item);
		}

		// Send repair price response
		SC_ITEM_REPAIR_PRICE response = new()
		{
			isEquiped = request.isEquiped,
			slot = request.slot,
			price = price
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();

		Logger.Log($"[REPAIR] Sent repair price {price} for item to {playerName}", LogLevel.Debug);
	}

	/// <summary>
	/// Calculates the repair price for an item based on its properties.
	/// </summary>
	private static uint CalculateRepairPrice(Item item)
	{
		// Base price on item value and durability loss
		// For now, use a simple formula: base_price * grade * (1 - durability_ratio)
		int basePrice = item.Price > 0 ? item.Price : 100;
		int grade = item.Grade > 0 ? item.Grade : 1;

		// TODO: Calculate actual durability loss when durability tracking is implemented
		// For now, assume 50% durability loss
		float durabilityLoss = 0.5f;

		uint repairCost = (uint)(basePrice * grade * durabilityLoss * RepairCostMultiplier);
		return Math.Max(1, repairCost); // Minimum 1 currency
	}
}
