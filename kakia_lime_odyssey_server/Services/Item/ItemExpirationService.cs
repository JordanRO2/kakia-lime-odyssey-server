/// <summary>
/// Service for managing item expiration and time-limited items.
/// </summary>
/// <remarks>
/// Handles tracking expiration times, checking for expired items,
/// and removing expired items from player inventories.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;
using ItemModel = kakia_lime_odyssey_server.Models.Item;

namespace kakia_lime_odyssey_server.Services.Item;

/// <summary>
/// Service for managing item expiration.
/// </summary>
public class ItemExpirationService
{
	/// <summary>
	/// Check interval for expired items (in seconds).
	/// </summary>
	private const int ExpirationCheckIntervalSeconds = 60;

	/// <summary>
	/// Maximum inventory slots (fixed game value).
	/// </summary>
	private const int MaxInventorySlots = 96;

	private DateTime _lastExpirationCheck = DateTime.UtcNow;

	/// <summary>
	/// Checks if enough time has passed to run an expiration check.
	/// </summary>
	public bool ShouldRunExpirationCheck()
	{
		return (DateTime.UtcNow - _lastExpirationCheck).TotalSeconds >= ExpirationCheckIntervalSeconds;
	}

	/// <summary>
	/// Performs expiration check on all connected players.
	/// Called from server tick.
	/// </summary>
	public void PerformExpirationCheck()
	{
		_lastExpirationCheck = DateTime.UtcNow;

		foreach (var player in LimeServer.PlayerClients)
		{
			if (!player.IsLoaded())
				continue;

			CheckPlayerInventory(player);
			CheckPlayerEquipment(player, isCombat: true);
			CheckPlayerEquipment(player, isCombat: false);
			CheckPlayerBank(player);
		}
	}

	/// <summary>
	/// Checks a player's inventory for expired items.
	/// </summary>
	public void CheckPlayerInventory(PlayerClient pc)
	{
		var inventory = pc.GetInventory();
		var expiredSlots = new List<int>();

		// Check all inventory slots
		for (int slot = 1; slot <= MaxInventorySlots; slot++)
		{
			var item = inventory.AtSlot(slot) as ItemModel;
			if (item == null)
				continue;

			if (item.IsExpired())
			{
				expiredSlots.Add(slot);
			}
		}

		// Remove expired items
		if (expiredSlots.Count > 0)
		{
			foreach (int slot in expiredSlots)
			{
				var item = inventory.AtSlot(slot) as ItemModel;
				if (item != null)
				{
					inventory.RemoveItem(slot);
					NotifyItemExpired(pc, item, "inventory", slot);
				}
			}

			// Send updated inventory
			pc.SendInventory();
		}
	}

	/// <summary>
	/// Checks a player's equipment for expired items.
	/// </summary>
	public void CheckPlayerEquipment(PlayerClient pc, bool isCombat)
	{
		var equipment = pc.GetEquipment(isCombat);
		var expiredSlots = new List<kakia_lime_odyssey_packets.Packets.Enums.EQUIP_SLOT>();

		// Check all equipment slots
		for (int i = 1; i <= 20; i++)
		{
			var slot = (kakia_lime_odyssey_packets.Packets.Enums.EQUIP_SLOT)i;
			var item = equipment.GetItemInSlot(slot) as ItemModel;
			if (item == null)
				continue;

			if (item.IsExpired())
			{
				expiredSlots.Add(slot);
			}
		}

		// Remove expired items
		if (expiredSlots.Count > 0)
		{
			foreach (var slot in expiredSlots)
			{
				var item = equipment.GetItemInSlot(slot) as ItemModel;
				if (item != null)
				{
					equipment.UnEquip(slot);
					string equipType = isCombat ? "combat equipment" : "life equipment";
					NotifyItemExpired(pc, item, equipType, (int)slot);
				}
			}

			// Send updated equipment
			pc.SendEquipment();
		}
	}

	/// <summary>
	/// Checks a player's bank for expired items.
	/// </summary>
	public void CheckPlayerBank(PlayerClient pc)
	{
		var bank = pc.GetBank();
		if (!bank.IsOpen)
			return; // Only check bank when open

		var expiredSlots = new List<int>();
		var allItems = bank.GetAllItems();

		foreach (var kvp in allItems)
		{
			var item = kvp.Value as ItemModel;
			if (item == null)
				continue;

			if (item.IsExpired())
			{
				expiredSlots.Add(kvp.Key);
			}
		}

		// Remove expired items
		foreach (int slot in expiredSlots)
		{
			var item = bank.AtSlot(slot) as ItemModel;
			if (item != null)
			{
				bank.RemoveItem(slot);
				NotifyItemExpired(pc, item, "bank", slot);
			}
		}
	}

	/// <summary>
	/// Notifies player that an item has expired.
	/// </summary>
	private void NotifyItemExpired(PlayerClient pc, ItemModel item, string location, int slot)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXPIRATION] {playerName}'s {item.Name} (ID: {item.Id}) in {location} slot {slot} has expired and was removed", LogLevel.Information);

		// Send SC_SYSTEM_MSG or similar notification to client
		// For now just log it
	}

	/// <summary>
	/// Creates a time-limited item that expires after the specified duration.
	/// </summary>
	/// <param name="item">Item to make time-limited</param>
	/// <param name="duration">Duration until expiration</param>
	public static void MakeTimeLimited(ItemModel item, TimeSpan duration)
	{
		item.SetExpiration(duration);
		Logger.Log($"[EXPIRATION] Item {item.Name} set to expire in {duration.TotalMinutes:F0} minutes", LogLevel.Debug);
	}

	/// <summary>
	/// Creates a time-limited item that expires at the specified time.
	/// </summary>
	/// <param name="item">Item to make time-limited</param>
	/// <param name="expirationTime">Absolute expiration time (UTC)</param>
	public static void MakeTimeLimitedUntil(ItemModel item, DateTime expirationTime)
	{
		item.SetExpiration(expirationTime);
		Logger.Log($"[EXPIRATION] Item {item.Name} set to expire at {expirationTime:yyyy-MM-dd HH:mm:ss} UTC", LogLevel.Debug);
	}

	/// <summary>
	/// Creates a time-limited item from a duration in seconds.
	/// </summary>
	/// <param name="item">Item to make time-limited</param>
	/// <param name="seconds">Seconds until expiration</param>
	public static void MakeTimeLimitedSeconds(ItemModel item, int seconds)
	{
		MakeTimeLimited(item, TimeSpan.FromSeconds(seconds));
	}

	/// <summary>
	/// Creates a time-limited item from a duration in minutes.
	/// </summary>
	/// <param name="item">Item to make time-limited</param>
	/// <param name="minutes">Minutes until expiration</param>
	public static void MakeTimeLimitedMinutes(ItemModel item, int minutes)
	{
		MakeTimeLimited(item, TimeSpan.FromMinutes(minutes));
	}

	/// <summary>
	/// Creates a time-limited item from a duration in hours.
	/// </summary>
	/// <param name="item">Item to make time-limited</param>
	/// <param name="hours">Hours until expiration</param>
	public static void MakeTimeLimitedHours(ItemModel item, int hours)
	{
		MakeTimeLimited(item, TimeSpan.FromHours(hours));
	}

	/// <summary>
	/// Creates a time-limited item from a duration in days.
	/// </summary>
	/// <param name="item">Item to make time-limited</param>
	/// <param name="days">Days until expiration</param>
	public static void MakeTimeLimitedDays(ItemModel item, int days)
	{
		MakeTimeLimited(item, TimeSpan.FromDays(days));
	}

	/// <summary>
	/// Checks if a specific item will expire soon (within threshold).
	/// </summary>
	/// <param name="item">Item to check</param>
	/// <param name="thresholdSeconds">Warning threshold in seconds</param>
	/// <returns>True if item expires within threshold</returns>
	public static bool WillExpireSoon(ItemModel item, int thresholdSeconds = 300)
	{
		if (!item.HasExpiration())
			return false;

		int remainingSeconds = item.GetRemainingExpirySeconds();
		return remainingSeconds >= 0 && remainingSeconds <= thresholdSeconds;
	}
}
