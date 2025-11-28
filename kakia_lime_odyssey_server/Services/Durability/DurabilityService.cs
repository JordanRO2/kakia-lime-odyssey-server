/// <summary>
/// Service for managing item durability operations.
/// </summary>
/// <remarks>
/// Handles durability decrease on combat, repair, and durability update notifications.
/// Uses: PlayerEquipment for equipped items, sends SC_UPDATE_DURABILITY packets.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Item;
using ItemModel = kakia_lime_odyssey_server.Models.Item;

namespace kakia_lime_odyssey_server.Services.Durability;

/// <summary>
/// Service for managing item durability operations.
/// </summary>
public class DurabilityService
{
	/// <summary>
	/// Durability loss per weapon attack.
	/// </summary>
	private const int WeaponDurabilityLossPerAttack = 1;

	/// <summary>
	/// Durability loss per armor hit received.
	/// </summary>
	private const int ArmorDurabilityLossPerHit = 1;

	/// <summary>
	/// Durability threshold for equipment warning (percentage).
	/// </summary>
	private const int DurabilityWarningThreshold = 20;

	/// <summary>
	/// Decreases weapon durability after an attack.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="isCombatJob">True if using combat job equipment</param>
	public void OnPlayerAttack(PlayerClient pc, bool isCombatJob = true)
	{
		var equipment = pc.GetEquipment(isCombatJob);
		var mainWeapon = equipment.GetItemInSlot(EQUIP_SLOT.MAIN_EQUIP) as ItemModel;

		if (mainWeapon == null)
			return;

		if (mainWeapon.DecreaseDurability(WeaponDurabilityLossPerAttack))
		{
			// Item still usable, send update
			SendEquippedDurabilityUpdate(pc, mainWeapon, EQUIP_SLOT.MAIN_EQUIP, isCombatJob);
		}
		else
		{
			// Item broke - notify player
			SendEquippedDurabilityUpdate(pc, mainWeapon, EQUIP_SLOT.MAIN_EQUIP, isCombatJob);
			NotifyItemBroken(pc, mainWeapon);
		}
	}

	/// <summary>
	/// Decreases armor durability when player takes damage.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="isCombatJob">True if using combat job equipment</param>
	public void OnPlayerTakeDamage(PlayerClient pc, bool isCombatJob = true)
	{
		var equipment = pc.GetEquipment(isCombatJob);

		// Randomly select armor piece to degrade
		var armorSlots = new[]
		{
			EQUIP_SLOT.HEAD,
			EQUIP_SLOT.UPPER_BODY,
			EQUIP_SLOT.LOWER_BODY,
			EQUIP_SLOT.HAND,
			EQUIP_SLOT.FOOT
		};

		var slot = armorSlots[Random.Shared.Next(armorSlots.Length)];
		var armor = equipment.GetItemInSlot(slot) as ItemModel;

		if (armor == null)
			return;

		if (armor.DecreaseDurability(ArmorDurabilityLossPerHit))
		{
			// Item still usable
			SendEquippedDurabilityUpdate(pc, armor, slot, isCombatJob);

			// Check for low durability warning
			float durabilityPercent = (float)armor.GetDurability() / armor.GetMaxDurability() * 100;
			if (durabilityPercent <= DurabilityWarningThreshold)
			{
				NotifyLowDurability(pc, armor);
			}
		}
		else
		{
			// Item broke
			SendEquippedDurabilityUpdate(pc, armor, slot, isCombatJob);
			NotifyItemBroken(pc, armor);
		}
	}

	/// <summary>
	/// Decreases durability for a specific inventory item.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot</param>
	/// <param name="amount">Amount to decrease</param>
	public void DecreaseInventoryItemDurability(PlayerClient pc, int slot, int amount = 1)
	{
		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as ItemModel;

		if (item == null)
			return;

		item.DecreaseDurability(amount);
		SendInventoryDurabilityUpdate(pc, item, slot);

		if (item.IsBroken())
		{
			NotifyItemBroken(pc, item);
		}
	}

	/// <summary>
	/// Repairs a specific equipped item.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Equipment slot</param>
	/// <param name="isCombatJob">True if combat job equipment</param>
	/// <returns>Repair cost in Peder</returns>
	public uint RepairEquippedItem(PlayerClient pc, EQUIP_SLOT slot, bool isCombatJob = true)
	{
		var equipment = pc.GetEquipment(isCombatJob);
		var item = equipment.GetItemInSlot(slot) as ItemModel;

		if (item == null)
			return 0;

		uint cost = ItemService.CalculateRepairPrice(item);
		item.RepairFully();

		SendEquippedDurabilityUpdate(pc, item, slot, isCombatJob);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[DURABILITY] {playerName} repaired {item.Name} for {cost} Peder", LogLevel.Debug);

		return cost;
	}

	/// <summary>
	/// Repairs all equipped items for a player.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="isCombatJob">True if combat job equipment</param>
	/// <returns>Total repair cost in Peder</returns>
	public uint RepairAllEquippedItems(PlayerClient pc, bool isCombatJob = true)
	{
		var equipment = pc.GetEquipment(isCombatJob);
		uint totalCost = 0;

		for (int i = 1; i <= 20; i++)
		{
			var slot = (EQUIP_SLOT)i;
			var item = equipment.GetItemInSlot(slot) as ItemModel;

			if (item == null)
				continue;

			uint cost = ItemService.CalculateRepairPrice(item);
			if (cost > 0)
			{
				item.RepairFully();
				SendEquippedDurabilityUpdate(pc, item, slot, isCombatJob);
				totalCost += cost;
			}
		}

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[DURABILITY] {playerName} repaired all equipment for {totalCost} Peder", LogLevel.Information);

		return totalCost;
	}

	/// <summary>
	/// Sends durability update for an inventory item.
	/// </summary>
	private void SendInventoryDurabilityUpdate(PlayerClient pc, ItemModel item, int slot)
	{
		var packet = new SC_UPDATE_DURABILITY_INVENTORY_ITEM
		{
			slot = slot,
			durability = item.GetDurability(),
			mdurability = item.GetMaxDurability()
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends durability update for an equipped item.
	/// </summary>
	private void SendEquippedDurabilityUpdate(PlayerClient pc, ItemModel item, EQUIP_SLOT slot, bool isCombatJob)
	{
		using PacketWriter pw = new();

		if (isCombatJob)
		{
			var packet = new SC_COMBAT_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM
			{
				equipSlot = (byte)slot,
				durability = item.GetDurability(),
				mdurability = item.GetMaxDurability()
			};
			pw.Write(packet);
		}
		else
		{
			var packet = new SC_LIFE_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM
			{
				equipSlot = (byte)slot,
				durability = item.GetDurability(),
				mdurability = item.GetMaxDurability()
			};
			pw.Write(packet);
		}

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Notifies player their item has broken.
	/// </summary>
	private void NotifyItemBroken(PlayerClient pc, ItemModel item)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[DURABILITY] {playerName}'s {item.Name} has broken!", LogLevel.Warning);

		// Could send a system message to the client here
		// For now, just log it - the 0 durability update will show in UI
	}

	/// <summary>
	/// Notifies player of low durability warning.
	/// </summary>
	private void NotifyLowDurability(PlayerClient pc, ItemModel item)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[DURABILITY] {playerName}'s {item.Name} is at low durability ({item.GetDurability()}/{item.GetMaxDurability()})", LogLevel.Debug);
	}
}
