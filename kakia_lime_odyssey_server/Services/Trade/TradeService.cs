/// <summary>
/// Service for managing NPC merchant buy/sell transactions.
/// </summary>
/// <remarks>
/// Handles buying items from and selling items to NPC merchants.
/// Uses: PlayerInventory for item access, ItemDB for item definitions and prices
/// All buy/sell transactions are tracked via audit services.
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Entities.Npcs;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Audit;

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
/// Represents an NPC shop configuration.
/// </summary>
public class ShopConfig
{
	/// <summary>List of item type IDs sold by this shop</summary>
	public List<int> ItemTypeIds { get; set; } = new();

	/// <summary>Discount rate as percentage (0-100)</summary>
	public int DiscountRate { get; set; } = 0;

	/// <summary>Whether this shop can repair items</summary>
	public bool CanRepair { get; set; } = false;
}

/// <summary>
/// Service for managing NPC merchant buy/sell transactions.
/// </summary>
public class TradeService
{
	/// <summary>Tracks recently sold items per player for buyback</summary>
	private readonly ConcurrentDictionary<long, List<SoldItem>> _soldItems = new();

	/// <summary>Shop configurations by NPC type ID</summary>
	private readonly Dictionary<int, ShopConfig> _shopConfigs = new();

	/// <summary>Maximum items to keep in sold list per player</summary>
	private const int MaxSoldItems = 20;

	/// <summary>
	/// Initializes the trade service with default shop configurations.
	/// </summary>
	public TradeService()
	{
		InitializeDefaultShops();
	}

	/// <summary>
	/// Sets up default shop inventories for common NPC types.
	/// </summary>
	private void InitializeDefaultShops()
	{
		// General Goods Merchant (potions, basic supplies)
		RegisterShop(1001, new ShopConfig
		{
			ItemTypeIds = new List<int> { 1, 2, 3, 4, 5, 10, 11, 12, 13, 14 },
			DiscountRate = 0,
			CanRepair = false
		});

		// Blacksmith (weapons, repairs)
		RegisterShop(1002, new ShopConfig
		{
			ItemTypeIds = new List<int> { 100, 101, 102, 103, 104, 105, 106, 107, 108, 109 },
			DiscountRate = 0,
			CanRepair = true
		});

		// Armor Vendor
		RegisterShop(1003, new ShopConfig
		{
			ItemTypeIds = new List<int> { 200, 201, 202, 203, 204, 205, 206, 207, 208, 209 },
			DiscountRate = 0,
			CanRepair = true
		});

		// Magic Shop
		RegisterShop(1004, new ShopConfig
		{
			ItemTypeIds = new List<int> { 300, 301, 302, 303, 304 },
			DiscountRate = 0,
			CanRepair = false
		});

		Logger.Log($"[TRADE] Initialized {_shopConfigs.Count} shop configurations", LogLevel.Information);
	}

	/// <summary>
	/// Registers a shop configuration for an NPC type.
	/// </summary>
	/// <param name="npcTypeId">NPC type identifier</param>
	/// <param name="config">Shop configuration</param>
	public void RegisterShop(int npcTypeId, ShopConfig config)
	{
		_shopConfigs[npcTypeId] = config;
	}

	/// <summary>
	/// Gets the shop configuration for an NPC type.
	/// </summary>
	/// <param name="npcTypeId">NPC type identifier</param>
	/// <returns>Shop configuration or null if not a merchant</returns>
	public ShopConfig? GetShopConfig(int npcTypeId)
	{
		return _shopConfigs.TryGetValue(npcTypeId, out var config) ? config : null;
	}
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

		// Check inventory space first (before taking money)
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

		// Track item creation for audit
		var instanceId = LimeServer.ItemAuditService.CreateTrackedItem(newItem, ItemCreationSource.Shop, pc, $"BoughtFromNPC");

		// Process purchase with audited currency method
		if (!LimeServer.CurrencyService.ProcessShopBuyAudited(pc, itemTypeId, itemDef.Name, itemDef.Price, count, instanceId))
		{
			long totalPrice = LimeServer.CurrencyService.CalculateBuyPrice(itemDef.Price, count);
			Logger.Log($"[TRADE] {playerName} cannot afford {totalPrice} Peder for {itemDef.Name} x{count}", LogLevel.Debug);
			return;
		}

		// Add item to inventory
		inventory.AddItem(newItem, targetSlot);

		// Log item bought from NPC
		LimeServer.ItemAuditService.LogBoughtFromNpc(instanceId, pc, (ulong)count, itemDef.Price * (int)count);

		Logger.Log($"[TRADE] {playerName} bought {itemDef.Name} x{count}", LogLevel.Information);

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

		// Create tracked item instance for audit trail before selling
		var instanceId = LimeServer.ItemAuditService.CreateTrackedItem(item, ItemCreationSource.Unknown, pc, "SoldToNPC");

		// Remove/reduce item first, then add currency
		if (count >= (long)item.GetAmount())
		{
			inventory.RemoveItem(slot);
		}
		else
		{
			item.UpdateAmount(item.GetAmount() - (ulong)count);
			inventory.UpdateItemAtSlot(slot, item);
		}

		// Process sell with audited currency method
		long sellPrice = LimeServer.CurrencyService.ProcessShopSellAudited(pc, item.Id, item.Name, item.Price, count, instanceId);

		// Log item sold to NPC
		LimeServer.ItemAuditService.LogSoldToNpc(instanceId, pc, (ulong)count, (int)sellPrice);

		// Track sold items for buyback
		AddToSoldItems(pc, item, count, sellPrice);

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
		pw.Write((ushort)PacketType.SC_TRADE_END);
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

		Logger.Log($"[TRADE] {playerName} opened trade with NPC {npc.Appearance.appearance.typeID}", LogLevel.Debug);

		// Send trade description packet with NPC shop items
		// Shop items are loaded from registered shop configurations (see InitializeDefaultShops)
		SendTradeDesc(pc, npc);
	}

	/// <summary>
	/// Sends SC_TRADE_DESC packet with NPC shop information.
	/// </summary>
	private void SendTradeDesc(PlayerClient pc, Npc npc)
	{
		int npcTypeId = (int)npc.Appearance.appearance.typeID;
		var shopConfig = GetShopConfig(npcTypeId);

		// Get shop items
		var tradeItems = BuildTradeItemList(shopConfig);

		// Build packet: SC_TRADE_DESC header + TRADE_ITEM array
		using PacketWriter pw = new();
		pw.Write((ushort)PacketType.SC_TRADE_DESC);
		pw.Write((long)npc.Id);                          // objInstID
		pw.Write((uint)tradeItems.Count);                // itemCount
		pw.Write(shopConfig?.DiscountRate ?? 0);         // discountRate
		pw.Write(shopConfig?.CanRepair ?? false);        // isRepairable

		// Write each TRADE_ITEM as raw bytes
		foreach (var tradeItem in tradeItems)
		{
			WriteTradeItem(pw, tradeItem);
		}

		pc.Send(pw.ToSizedPacket(), default).Wait();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[TRADE] Sent shop inventory to {playerName}: {tradeItems.Count} items from NPC type {npcTypeId}", LogLevel.Debug);
	}

	/// <summary>
	/// Writes a TRADE_ITEM structure to the packet writer.
	/// </summary>
	private static void WriteTradeItem(PacketWriter pw, TRADE_ITEM item)
	{
		pw.Write(item.typeID);
		pw.Write(item.count);
		pw.Write(item.price);
		pw.Write(item.durability);
		pw.Write(item.mdurability);
		pw.Write(item.grade);

		// Write ITEM_INHERITS (25 x ITEM_INHERIT)
		for (int i = 0; i < 25; i++)
		{
			var inherit = item.inherits.inherits[i];
			pw.Write(inherit.typeID);
			pw.Write(inherit.value);
			pw.Write(inherit.type);
			pw.Write(inherit.padding1);
			pw.Write(inherit.padding2);
			pw.Write(inherit.padding3);
		}
	}

	/// <summary>
	/// Builds a list of TRADE_ITEM structures from shop configuration.
	/// </summary>
	/// <param name="shopConfig">Shop configuration or null for no items</param>
	/// <returns>List of TRADE_ITEM structures</returns>
	private List<TRADE_ITEM> BuildTradeItemList(ShopConfig? shopConfig)
	{
		var result = new List<TRADE_ITEM>();

		if (shopConfig == null)
		{
			return result;
		}

		foreach (int itemTypeId in shopConfig.ItemTypeIds)
		{
			var itemDef = LimeServer.ItemDB.FirstOrDefault(i => i.Id == itemTypeId);
			if (itemDef == null)
			{
				Logger.Log($"[TRADE] Warning: Shop item {itemTypeId} not found in ItemDB", LogLevel.Warning);
				continue;
			}

			// Use Durable property for durability (default to 100 if not set)
			int durability = itemDef.Durable > 0 ? itemDef.Durable : 100;

			// Create TRADE_ITEM from item definition
			var tradeItem = new TRADE_ITEM
			{
				typeID = itemDef.Id,
				count = 99,                              // Unlimited stock
				price = (uint)itemDef.Price,
				durability = durability,
				mdurability = durability,
				grade = 0,                               // No enchant on shop items
				inherits = CreateItemInherits(itemDef)
			};

			result.Add(tradeItem);
		}

		return result;
	}

	/// <summary>
	/// Creates ITEM_INHERITS from item definition.
	/// </summary>
	private static ITEM_INHERITS CreateItemInherits(Models.Item itemDef)
	{
		var inherits = new ITEM_INHERITS
		{
			inherits = new ITEM_INHERIT[25]
		};

		// Initialize all 25 slots to empty
		for (int i = 0; i < 25; i++)
		{
			inherits.inherits[i] = new ITEM_INHERIT();
		}

		// Copy inherits from item definition if any
		if (itemDef.Inherits != null)
		{
			int index = 0;
			foreach (var inherit in itemDef.Inherits)
			{
				if (index >= 25) break;
				inherits.inherits[index] = new ITEM_INHERIT
				{
					typeID = (uint)inherit.typeID,
					value = inherit.val,
					type = 0
				};
				index++;
			}
		}

		return inherits;
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
		pw.Write((ushort)PacketType.SC_SOLD_ITEM_LIST);
		pw.Write(soldItems.Count);

		foreach (var item in soldItems)
		{
			pw.Write(item.ItemTypeId);
			pw.Write(item.Count);
			pw.Write(item.SellPrice);
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

		// Check if player has enough Peder for buyback
		if (!LimeServer.CurrencyService.HasEnoughPeder(pc, totalCost))
		{
			Logger.Log($"[TRADE] {playerName} cannot afford {totalCost} Peder for buyback", LogLevel.Debug);
			return;
		}

		// Deduct the buyback cost
		LimeServer.CurrencyService.RemovePeder(pc, totalCost);

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
		pw.Write((ushort)PacketType.SC_TRADE_BOUGHT_SOLD_ITEMS);
		pw.Write(itemsReturned);
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
