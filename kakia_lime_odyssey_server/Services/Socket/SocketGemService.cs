/// <summary>
/// Service for managing gem socketing into equipment items.
/// </summary>
/// <remarks>
/// Handles gem validation, socketing operations, inherit calculation,
/// and socket slot management for equipment items.
/// Uses: InheritInfo for inherit definitions, ItemService for item operations.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.InheritXML;
using kakia_lime_odyssey_server.Network;
using ItemModel = kakia_lime_odyssey_server.Models.Item;

namespace kakia_lime_odyssey_server.Services.Socket;

/// <summary>
/// Result of a socket operation.
/// </summary>
public class SocketResult
{
	/// <summary>Whether the operation succeeded.</summary>
	public bool Success { get; set; }

	/// <summary>Error message if operation failed.</summary>
	public string ErrorMessage { get; set; } = string.Empty;

	/// <summary>The inherit type that was added.</summary>
	public int InheritTypeId { get; set; }

	/// <summary>The inherit value that was added.</summary>
	public int InheritValue { get; set; }

	/// <summary>Creates a success result.</summary>
	public static SocketResult Succeeded(int inheritTypeId, int value) => new()
	{
		Success = true,
		InheritTypeId = inheritTypeId,
		InheritValue = value
	};

	/// <summary>Creates a failure result.</summary>
	public static SocketResult Failed(string message) => new()
	{
		Success = false,
		ErrorMessage = message
	};
}

/// <summary>
/// Information about a gem item's socketing properties.
/// </summary>
public class GemInfo
{
	/// <summary>The gem item ID.</summary>
	public int ItemId { get; set; }

	/// <summary>The gem item name.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>The inherit type this gem provides.</summary>
	public int InheritTypeId { get; set; }

	/// <summary>The inherit type name.</summary>
	public string InheritName { get; set; } = string.Empty;

	/// <summary>The inherit value this gem provides.</summary>
	public int InheritValue { get; set; }
}

/// <summary>
/// Service for managing gem socketing operations.
/// </summary>
public class SocketGemService
{
	/// <summary>
	/// Maximum number of socket slots an item can have.
	/// </summary>
	public const int MaxSocketSlots = 5;

	/// <summary>
	/// Item stuffType values that represent socket gems.
	/// </summary>
	private static readonly HashSet<int> GemStuffTypes = new() { 50, 51, 52, 53, 54 };

	private readonly Dictionary<int, GemInfo> _gemCache = new();
	private bool _gemCacheInitialized;

	/// <summary>
	/// Initializes the gem cache from item and inherit data.
	/// </summary>
	public void InitializeGemCache()
	{
		if (_gemCacheInitialized) return;

		try
		{
			var inherits = InheritInfo.GetEntries();
			foreach (var inheritEntry in inherits.Values)
			{
				foreach (var socketItem in inheritEntry.SocketItems)
				{
					if (!_gemCache.ContainsKey(socketItem.ItemTypeId))
					{
						_gemCache[socketItem.ItemTypeId] = new GemInfo
						{
							ItemId = socketItem.ItemTypeId,
							InheritTypeId = inheritEntry.TypeId,
							InheritName = inheritEntry.TypeName,
							InheritValue = socketItem.Value
						};
					}
				}
			}
			_gemCacheInitialized = true;
			Logger.Log($"[SOCKET] Initialized gem cache with {_gemCache.Count} gems", LogLevel.Information);
		}
		catch (Exception ex)
		{
			Logger.Log($"[SOCKET] Failed to initialize gem cache: {ex.Message}", LogLevel.Error);
		}
	}

	/// <summary>
	/// Gets gem information for an item ID.
	/// </summary>
	/// <param name="gemItemId">The gem item ID.</param>
	/// <returns>GemInfo if found, null otherwise.</returns>
	public GemInfo? GetGemInfo(int gemItemId)
	{
		InitializeGemCache();
		return _gemCache.TryGetValue(gemItemId, out var info) ? info : null;
	}

	/// <summary>
	/// Checks if an item is a socketable gem.
	/// </summary>
	/// <param name="item">The item to check.</param>
	/// <returns>True if item is a gem.</returns>
	public bool IsGem(ItemModel item)
	{
		return GetGemInfo(item.Id) != null || GemStuffTypes.Contains(item.StuffType);
	}

	/// <summary>
	/// Checks if an item can have gems socketed into it.
	/// </summary>
	/// <param name="item">The item to check.</param>
	/// <returns>True if item supports socketing.</returns>
	public bool CanBeSocketed(ItemModel item)
	{
		// Equipment items with MaxEnchantCount > 0 can be socketed
		return item.MaxEnchantCount > 0 && item.Type >= 1 && item.Type <= 20;
	}

	/// <summary>
	/// Gets the maximum number of sockets for an item based on its grade/enchant level.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>Maximum socket count.</returns>
	public int GetMaxSockets(ItemModel item)
	{
		if (!CanBeSocketed(item)) return 0;

		// Base sockets from MaxEnchantCount, capped at MaxSocketSlots
		int baseSockets = Math.Min(item.MaxEnchantCount, MaxSocketSlots);

		// Additional sockets can be unlocked by grade
		int gradeBonus = item.Grade / 3; // Every 3 grade levels adds a socket

		return Math.Min(baseSockets + gradeBonus, MaxSocketSlots);
	}

	/// <summary>
	/// Initializes socket slots on an item if not already set.
	/// </summary>
	/// <param name="item">The item to initialize.</param>
	public void InitializeItemSockets(ItemModel item)
	{
		if (item.SocketCount == 0 && CanBeSocketed(item))
		{
			item.SocketCount = GetMaxSockets(item);
		}
	}

	/// <summary>
	/// Validates if a gem can be socketed into a target item.
	/// </summary>
	/// <param name="targetItem">The item to socket into.</param>
	/// <param name="gemItem">The gem to socket.</param>
	/// <returns>Validation result with error message if invalid.</returns>
	public (bool IsValid, string ErrorMessage) ValidateSocket(ItemModel targetItem, ItemModel gemItem)
	{
		// Check if target can be socketed
		if (!CanBeSocketed(targetItem))
		{
			return (false, "This item cannot have gems socketed.");
		}

		// Check if gem is valid
		var gemInfo = GetGemInfo(gemItem.Id);
		if (gemInfo == null)
		{
			return (false, "This item is not a valid socket gem.");
		}

		// Initialize sockets if needed
		InitializeItemSockets(targetItem);

		// Check for available slots
		if (!targetItem.CanAddSocket())
		{
			return (false, $"No available socket slots. ({targetItem.SocketedInherits?.Count ?? 0}/{targetItem.SocketCount})");
		}

		// Check for duplicate inherit type (optional - depends on game design)
		if (targetItem.SocketedInherits?.Any(s => s.InheritTypeId == gemInfo.InheritTypeId) == true)
		{
			return (false, $"This item already has a {gemInfo.InheritName} gem socketed.");
		}

		return (true, string.Empty);
	}

	/// <summary>
	/// Sockets a gem into a target item.
	/// </summary>
	/// <param name="pc">The player client.</param>
	/// <param name="targetSlot">Inventory slot of target item.</param>
	/// <param name="gemSlot">Inventory slot of gem item.</param>
	/// <returns>Socket result.</returns>
	public SocketResult SocketGem(PlayerClient pc, int targetSlot, int gemSlot)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var inventory = pc.GetInventory();

		// Get items
		var targetItem = inventory.AtSlot(targetSlot) as ItemModel;
		var gemItem = inventory.AtSlot(gemSlot) as ItemModel;

		if (targetItem == null)
		{
			return SocketResult.Failed("Target item not found.");
		}

		if (gemItem == null)
		{
			return SocketResult.Failed("Gem item not found.");
		}

		// Validate
		var (isValid, errorMessage) = ValidateSocket(targetItem, gemItem);
		if (!isValid)
		{
			Logger.Log($"[SOCKET] {playerName} failed socket validation: {errorMessage}", LogLevel.Warning);
			return SocketResult.Failed(errorMessage);
		}

		// Get gem info
		var gemInfo = GetGemInfo(gemItem.Id);
		if (gemInfo == null)
		{
			return SocketResult.Failed("Invalid gem.");
		}

		// Add socketed inherit
		if (!targetItem.AddSocketedInherit(gemInfo.InheritTypeId, gemInfo.InheritValue, gemItem.Id))
		{
			return SocketResult.Failed("Failed to add socket.");
		}

		// Consume gem
		if (gemItem.Stackable() && gemItem.Count > 1)
		{
			gemItem.UpdateAmount(gemItem.Count - 1);
			inventory.UpdateItemAtSlot(gemSlot, gemItem);
		}
		else
		{
			inventory.RemoveItem(gemSlot);
		}

		Logger.Log($"[SOCKET] {playerName} socketed {gemInfo.InheritName} gem (+{gemInfo.InheritValue}) into {targetItem.Name}", LogLevel.Information);

		return SocketResult.Succeeded(gemInfo.InheritTypeId, gemInfo.InheritValue);
	}

	/// <summary>
	/// Removes a socketed gem from an item.
	/// </summary>
	/// <param name="pc">The player client.</param>
	/// <param name="targetSlot">Inventory slot of target item.</param>
	/// <param name="socketIndex">Index of socket to remove (0-based).</param>
	/// <param name="returnGem">Whether to return the gem to inventory.</param>
	/// <returns>True if successful.</returns>
	public bool UnsocketGem(PlayerClient pc, int targetSlot, int socketIndex, bool returnGem = false)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var inventory = pc.GetInventory();

		var targetItem = inventory.AtSlot(targetSlot) as ItemModel;
		if (targetItem == null)
		{
			Logger.Log($"[SOCKET] {playerName} unsocket failed: Target item not found", LogLevel.Warning);
			return false;
		}

		var removed = targetItem.RemoveSocketedInherit(socketIndex);
		if (removed == null)
		{
			Logger.Log($"[SOCKET] {playerName} unsocket failed: Invalid socket index {socketIndex}", LogLevel.Warning);
			return false;
		}

		// Optionally return gem to inventory
		if (returnGem)
		{
			var items = ItemInfo.GetItems();
			var gemDef = items.FirstOrDefault(i => i.Id == removed.GemItemId);
			if (gemDef != null)
			{
				var returnedGem = new ItemModel
				{
					Id = gemDef.Id,
					ModelId = gemDef.ModelId,
					Name = gemDef.Name,
					Desc = gemDef.Desc,
					Grade = gemDef.Grade,
					MaxEnchantCount = gemDef.MaxEnchantCount,
					Type = gemDef.Type,
					SecondType = gemDef.SecondType,
					Level = gemDef.Level,
					StuffType = gemDef.StuffType,
					ImageName = gemDef.ImageName,
					SmallImageName = gemDef.SmallImageName,
					Price = gemDef.Price,
					Inherits = gemDef.Inherits,
					Count = 1
				};

				if (inventory.FreeSlotsCount() > 0)
				{
					inventory.AddItem(returnedGem);
					Logger.Log($"[SOCKET] {playerName} unsocketed gem returned to inventory", LogLevel.Debug);
				}
				else
				{
					Logger.Log($"[SOCKET] {playerName} unsocket: No inventory space for returned gem", LogLevel.Warning);
				}
			}
		}

		var gemInfo = GetGemInfo(removed.GemItemId);
		string gemName = gemInfo?.InheritName ?? "Unknown";
		Logger.Log($"[SOCKET] {playerName} unsocketed {gemName} gem from {targetItem.Name}", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Gets the total inherit bonuses from all socketed gems on an item.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>Dictionary of inherit type ID to total value.</returns>
	public Dictionary<int, int> GetSocketedInheritTotals(ItemModel item)
	{
		var totals = new Dictionary<int, int>();

		if (item.SocketedInherits == null) return totals;

		foreach (var socketed in item.SocketedInherits)
		{
			if (totals.ContainsKey(socketed.InheritTypeId))
			{
				totals[socketed.InheritTypeId] += socketed.Value;
			}
			else
			{
				totals[socketed.InheritTypeId] = socketed.Value;
			}
		}

		return totals;
	}

	/// <summary>
	/// Gets a formatted description of all socketed gems on an item.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>List of socket descriptions.</returns>
	public List<string> GetSocketDescriptions(ItemModel item)
	{
		var descriptions = new List<string>();

		if (item.SocketedInherits == null) return descriptions;

		foreach (var socketed in item.SocketedInherits)
		{
			var inherit = InheritInfo.GetInherit(socketed.InheritTypeId);
			string name = inherit?.TypeName ?? $"Type {socketed.InheritTypeId}";
			descriptions.Add($"{name}: +{socketed.Value}");
		}

		return descriptions;
	}

	/// <summary>
	/// Gets socket slot status for an item.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>Tuple of (used slots, total slots).</returns>
	public (int Used, int Total) GetSocketStatus(ItemModel item)
	{
		InitializeItemSockets(item);
		int used = item.SocketedInherits?.Count ?? 0;
		return (used, item.SocketCount);
	}

	/// <summary>
	/// Calculates the cost to add a socket slot to an item.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>Cost in Peder, or -1 if cannot add more sockets.</returns>
	public int CalculateSocketSlotCost(ItemModel item)
	{
		InitializeItemSockets(item);

		if (item.SocketCount >= MaxSocketSlots)
		{
			return -1; // Cannot add more sockets
		}

		// Base cost increases with current socket count and item grade
		int baseCost = 10000;
		int socketMultiplier = (item.SocketCount + 1) * 2;
		int gradeMultiplier = Math.Max(1, item.Grade);

		return baseCost * socketMultiplier * gradeMultiplier;
	}

	/// <summary>
	/// Adds a socket slot to an item.
	/// </summary>
	/// <param name="pc">The player client.</param>
	/// <param name="targetSlot">Inventory slot of target item.</param>
	/// <returns>True if successful.</returns>
	public bool AddSocketSlot(PlayerClient pc, int targetSlot)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var inventory = pc.GetInventory();

		var targetItem = inventory.AtSlot(targetSlot) as ItemModel;
		if (targetItem == null)
		{
			Logger.Log($"[SOCKET] {playerName} add socket failed: Item not found", LogLevel.Warning);
			return false;
		}

		if (!CanBeSocketed(targetItem))
		{
			Logger.Log($"[SOCKET] {playerName} add socket failed: Item cannot be socketed", LogLevel.Warning);
			return false;
		}

		InitializeItemSockets(targetItem);

		if (targetItem.SocketCount >= MaxSocketSlots)
		{
			Logger.Log($"[SOCKET] {playerName} add socket failed: Max sockets reached", LogLevel.Warning);
			return false;
		}

		int cost = CalculateSocketSlotCost(targetItem);
		if (cost < 0)
		{
			return false;
		}

		// Check if player can afford
		if (inventory.WalletPeder < cost)
		{
			Logger.Log($"[SOCKET] {playerName} add socket failed: Insufficient funds (need {cost}, have {inventory.WalletPeder})", LogLevel.Warning);
			return false;
		}

		// Deduct cost and add socket
		inventory.WalletPeder -= cost;
		targetItem.SocketCount++;

		Logger.Log($"[SOCKET] {playerName} added socket slot to {targetItem.Name} for {cost} Peder (now {targetItem.SocketCount} slots)", LogLevel.Information);

		return true;
	}
}
