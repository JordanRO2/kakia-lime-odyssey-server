/// <summary>
/// Service for managing NPC merchant buy/sell transactions.
/// </summary>
/// <remarks>
/// Handles buying items from and selling items to NPC merchants.
/// Uses: PlayerInventory for item access, ItemDB for item definitions and prices
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Entities.Npcs;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Trade;

/// <summary>
/// Represents an item that was sold and can be bought back.
/// </summary>
public class SoldItem
{
	public int ItemTypeId { get; set; }
	public string Name { get; set; } = "";
	public long Count { get; set; }
	public long SellPrice { get; set; }
	public DateTime SoldAt { get; set; }
}

/// <summary>
/// Service for managing NPC merchant buy/sell transactions.
/// </summary>
public class TradeService
{
	/// <summary>Tracks recently sold items per player for buyback</summary>
	private readonly ConcurrentDictionary<long, List<SoldItem>> _soldItems = new();

	/// <summary>Maximum items to keep in sold list per player</summary>
	private const int MaxSoldItems = 20;
	/// <summary>
	/// Processes a buy request from NPC merchant.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="itemTypeId">Item type ID to purchase</param>
	/// <param name="count">Quantity to purchase</param>
	/// <param name="targetSlot">Target inventory slot</param>
	public void BuyItem(PlayerClient pc, int itemTypeId, long count, int targetSlot)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[TRADE] BuyItem failed: No character loaded", LogLevel.Warning);
			return;
		}

		string playerName = character.appearance.name;

		// Get item definition
		var itemDef = LimeServer.ItemDB.FirstOrDefault(i => i.Id == itemTypeId);
		if (itemDef == null)
		{
			Logger.Log($"[TRADE] BuyItem failed: Invalid item ID {itemTypeId}", LogLevel.Warning);
			return;
		}

		// Calculate total price
		long totalPrice = itemDef.Price * count;

		// TODO: Check if player has enough currency when currency system is implemented
		// For now, allow purchase (client-side validates currency)
		long totalPriceValue = totalPrice; // For logging

		// Check inventory space
		var inventory = pc.GetInventory();
		if (targetSlot <= 0)
		{
			// Find first empty slot
			for (int i = 1; i <= 96; i++)
			{
				if (inventory.AtSlot(i) == null)
				{
					targetSlot = i;
					break;
				}
			}
		}

		if (targetSlot <= 0 || inventory.AtSlot(targetSlot) != null)
		{
			Logger.Log($"[TRADE] {playerName} has no inventory space for {itemDef.Name}", LogLevel.Debug);
			return;
		}

		// Create item copy for inventory
		var newItem = new Models.Item
		{
			Id = itemDef.Id,
			ModelId = itemDef.ModelId,
			Name = itemDef.Name,
			Desc = itemDef.Desc,
			Grade = itemDef.Grade,
			Type = itemDef.Type,
			SecondType = itemDef.SecondType,
			Level = itemDef.Level,
			Price = itemDef.Price,
			Inherits = itemDef.Inherits ?? new List<Models.Inherit>()
		};
		newItem.UpdateAmount((ulong)count);

		// Add item to inventory
		inventory.AddItem(newItem, targetSlot);

		Logger.Log($"[TRADE] {playerName} bought {itemDef.Name} x{count} for {totalPriceValue} Peder", LogLevel.Information);

		// Send confirmation
		SendBuySellConfirmation(pc, targetSlot);

		// Update inventory display
		pc.SendInventory();
	}

	/// <summary>
	/// Processes a sell request to NPC merchant.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot of item to sell</param>
	/// <param name="count">Quantity to sell</param>
	public void SellItem(PlayerClient pc, int slot, long count)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[TRADE] SellItem failed: No character loaded", LogLevel.Warning);
			return;
		}

		string playerName = character.appearance.name;

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		if (item == null)
		{
			Logger.Log($"[TRADE] SellItem failed: No item at slot {slot}", LogLevel.Warning);
			return;
		}

		// Check if item is sellable
		if (item.IsSell == 0)
		{
			Logger.Log($"[TRADE] {playerName} cannot sell {item.Name} (not sellable)", LogLevel.Debug);
			return;
		}

		// Check count
		if (count > (long)item.GetAmount())
		{
			count = (long)item.GetAmount();
		}

		// Calculate sell price (typically 50% of buy price)
		long sellPrice = (item.Price / 2) * count;
		if (sellPrice <= 0) sellPrice = 1;

		// TODO: Add currency when currency system is implemented
		// Client-side handles currency addition based on SC_TRADE_BOUGHT_SOLD_ITEMS response

		// Remove/reduce item
		if (count >= (long)item.GetAmount())
		{
			inventory.RemoveItem(slot);
		}
		else
		{
			item.UpdateAmount(item.GetAmount() - (ulong)count);
			inventory.UpdateItemAtSlot(slot, item);
		}

		Logger.Log($"[TRADE] {playerName} sold {item.Name} x{count} for {sellPrice} Peder", LogLevel.Information);

		// Send confirmation
		SendBuySellConfirmation(pc, slot);

		// Update inventory display
		pc.SendInventory();
	}

	/// <summary>
	/// Gets the sell price for an item.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="slot">Inventory slot of item to price</param>
	public void GetItemSellPrice(PlayerClient pc, int slot)
	{
		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(slot) as Models.Item;

		long price = 0;
		if (item != null && item.IsSell != 0)
		{
			price = item.Price / 2;
			if (price <= 0) price = 1;
		}

		var packet = new SC_TRADE_PRICE
		{
			slot = slot,
			price = price
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Ends the current trade session.
	/// </summary>
	/// <param name="pc">The player client</param>
	public void EndTrade(PlayerClient pc)
	{
		var character = pc.GetCurrentCharacter();
		string playerName = character?.appearance.name ?? "Unknown";

		Logger.Log($"[TRADE] {playerName} ended trade session", LogLevel.Debug);

		// Send trade end packet
		using PacketWriter pw = new();
		pw.Writer.Write((ushort)PacketType.SC_TRADE_END);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends buy/sell confirmation packet.
	/// </summary>
	private void SendBuySellConfirmation(PlayerClient pc, int slot)
	{
		var packet = new SC_TRADE_BOUGHT_SOLD_ITEMS
		{
			slot = slot
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	// ============ NPC TRADE WINDOW ============

	/// <summary>
	/// Opens trade window with currently selected NPC.
	/// </summary>
	public void OpenTradeWithCurrentTarget(PlayerClient pc)
	{
		long targetId = pc.GetCurrentTarget();
		if (targetId == 0)
		{
			Logger.Log("[TRADE] OpenTrade failed: No target selected", LogLevel.Debug);
			return;
		}

		OpenTradeWithNpc(pc, targetId);
	}

	/// <summary>
	/// Opens trade window with specific NPC.
	/// </summary>
	public void OpenTradeWithNpc(PlayerClient pc, long npcInstId)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Find NPC in zone
		Npc? npc = null;
		uint zone = pc.GetZone();
		if (LimeServer.Npcs.TryGetValue(zone, out var npcsInZone))
		{
			npc = npcsInZone.FirstOrDefault(n => n.Id == npcInstId);
		}

		if (npc == null)
		{
			Logger.Log($"[TRADE] {playerName} failed to open trade: NPC {npcInstId} not found", LogLevel.Debug);
			return;
		}

		// Set as current target
		pc.SetCurrentTarget(npcInstId);

		Logger.Log($"[TRADE] {playerName} opened trade with NPC {npc.Appearance.typeID}", LogLevel.Debug);

		// Send trade description packet with NPC shop items
		// TODO: Load shop items from NPC definition when shop system is implemented
		SendTradeDesc(pc, npc);
	}

	/// <summary>
	/// Sends SC_TRADE_DESC packet with NPC shop information.
	/// </summary>
	private void SendTradeDesc(PlayerClient pc, Npc npc)
	{
		// For now, send a basic trade description
		// TODO: Implement actual shop inventory lookup based on NPC type
		using PacketWriter pw = new();
		pw.Writer.Write((ushort)PacketType.SC_TRADE_DESC);
		pw.Writer.Write((long)npc.Id); // NPC instance ID
		pw.Writer.Write((int)npc.Appearance.typeID); // NPC type ID
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	// ============ SOLD ITEMS BUYBACK ============

	/// <summary>
	/// Adds an item to the sold items list for potential buyback.
	/// </summary>
	private void AddToSoldItems(PlayerClient pc, Models.Item item, long count, long sellPrice)
	{
		long playerId = pc.GetId();

		var soldItem = new SoldItem
		{
			ItemTypeId = item.Id,
			Name = item.Name,
			Count = count,
			SellPrice = sellPrice,
			SoldAt = DateTime.Now
		};

		if (!_soldItems.ContainsKey(playerId))
		{
			_soldItems[playerId] = new List<SoldItem>();
		}

		var list = _soldItems[playerId];
		list.Add(soldItem);

		// Keep only the most recent items
		while (list.Count > MaxSoldItems)
		{
			list.RemoveAt(0);
		}
	}

	/// <summary>
	/// Gets the list of recently sold items for buyback.
	/// </summary>
	public void GetSoldItems(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		var soldItems = _soldItems.TryGetValue(playerId, out var list) ? list : new List<SoldItem>();

		Logger.Log($"[TRADE] {playerName} requesting sold items list ({soldItems.Count} items)", LogLevel.Debug);

		// Send SC_SOLD_ITEM_LIST packet
		using PacketWriter pw = new();
		pw.Writer.Write((ushort)PacketType.SC_SOLD_ITEM_LIST);
		pw.Writer.Write(soldItems.Count);

		foreach (var item in soldItems)
		{
			pw.Writer.Write(item.ItemTypeId);
			pw.Writer.Write(item.Count);
			pw.Writer.Write(item.SellPrice);
		}

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Buys back all recently sold items.
	/// </summary>
	public void BuyBackSoldItems(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_soldItems.TryGetValue(playerId, out var soldItems) || soldItems.Count == 0)
		{
			Logger.Log($"[TRADE] {playerName} has no items to buy back", LogLevel.Debug);
			return;
		}

		// Calculate total cost
		long totalCost = soldItems.Sum(i => i.SellPrice);

		// TODO: Check if player has enough gold when currency system is implemented

		var inventory = pc.GetInventory();
		int itemsReturned = 0;

		foreach (var soldItem in soldItems.ToList())
		{
			// Find empty slot
			int targetSlot = -1;
			for (int i = 1; i <= 96; i++)
			{
				if (inventory.AtSlot(i) == null)
				{
					targetSlot = i;
					break;
				}
			}

			if (targetSlot < 0)
			{
				Logger.Log($"[TRADE] {playerName} ran out of inventory space during buyback", LogLevel.Debug);
				break;
			}

			// Get item definition
			var itemDef = LimeServer.ItemDB.FirstOrDefault(i => i.Id == soldItem.ItemTypeId);
			if (itemDef == null) continue;

			// Create item
			var newItem = new Models.Item
			{
				Id = itemDef.Id,
				ModelId = itemDef.ModelId,
				Name = itemDef.Name,
				Desc = itemDef.Desc,
				Grade = itemDef.Grade,
				Type = itemDef.Type,
				SecondType = itemDef.SecondType,
				Level = itemDef.Level,
				Price = itemDef.Price,
				Inherits = itemDef.Inherits ?? new List<Models.Inherit>()
			};
			newItem.UpdateAmount((ulong)soldItem.Count);

			inventory.AddItem(newItem, targetSlot);
			itemsReturned++;
		}

		// Clear sold items
		soldItems.Clear();

		Logger.Log($"[TRADE] {playerName} bought back {itemsReturned} items for {totalCost} Peder", LogLevel.Information);

		// Send confirmation
		using PacketWriter pw = new();
		pw.Writer.Write((ushort)PacketType.SC_TRADE_BOUGHT_SOLD_ITEMS);
		pw.Writer.Write(itemsReturned);
		pc.Send(pw.ToSizedPacket(), default).Wait();

		// Update inventory
		pc.SendInventory();
	}

	/// <summary>
	/// Cleans up sold items for a player (on disconnect or after time expires).
	/// </summary>
	public void CleanupPlayer(long playerId)
	{
		_soldItems.TryRemove(playerId, out _);
	}
}
