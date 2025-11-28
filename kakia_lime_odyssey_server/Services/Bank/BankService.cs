/// <summary>
/// Service for managing player bank storage operations.
/// </summary>
/// <remarks>
/// Handles deposit, withdrawal, and bank slot management.
/// Bank storage persists between sessions and provides additional
/// item storage beyond the player's main inventory.
/// </remarks>
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_logging;

namespace kakia_lime_odyssey_server.Services.Bank;

/// <summary>
/// Result of a bank operation.
/// </summary>
public enum BankResult
{
	/// <summary>Operation completed successfully</summary>
	Success,

	/// <summary>Bank is not currently open</summary>
	BankNotOpen,

	/// <summary>Invalid slot number</summary>
	InvalidSlot,

	/// <summary>Slot is already occupied</summary>
	SlotOccupied,

	/// <summary>Slot is empty</summary>
	SlotEmpty,

	/// <summary>Not enough bank slots</summary>
	NotEnoughSlots,

	/// <summary>Item not found</summary>
	ItemNotFound
}

/// <summary>
/// Implements bank storage functionality for a player.
/// </summary>
public class PlayerBankStorage : IPlayerBank
{
	private const int DefaultMaxSlots = 30;
	private const int MaxExpandedSlots = 120;

	private readonly Dictionary<int, IItem> _items = new();
	private int _maxSlots;
	private bool _isOpen;

	/// <summary>
	/// Creates a new player bank storage.
	/// </summary>
	/// <param name="maxSlots">Initial maximum slots (default 30)</param>
	public PlayerBankStorage(int maxSlots = DefaultMaxSlots)
	{
		_maxSlots = Math.Min(maxSlots, MaxExpandedSlots);
		_isOpen = false;
	}

	/// <inheritdoc/>
	public int MaxSlots => _maxSlots;

	/// <inheritdoc/>
	public int ItemCount => _items.Count;

	/// <inheritdoc/>
	public bool IsOpen => _isOpen;

	/// <inheritdoc/>
	public void Open()
	{
		_isOpen = true;
		Logger.Log("[BANK] Bank opened", LogLevel.Debug);
	}

	/// <inheritdoc/>
	public void Close()
	{
		_isOpen = false;
		Logger.Log("[BANK] Bank closed", LogLevel.Debug);
	}

	/// <inheritdoc/>
	public IItem? AtSlot(int slot)
	{
		if (!IsValidSlot(slot))
			return null;

		return _items.TryGetValue(slot, out var item) ? item : null;
	}

	/// <inheritdoc/>
	public bool AddItem(IItem item, int slot)
	{
		if (!IsValidSlot(slot))
		{
			Logger.Log($"[BANK] Invalid slot {slot} (max: {_maxSlots})", LogLevel.Warning);
			return false;
		}

		if (_items.ContainsKey(slot))
		{
			Logger.Log($"[BANK] Slot {slot} is already occupied", LogLevel.Warning);
			return false;
		}

		_items[slot] = item;
		Logger.Log($"[BANK] Added item {item.GetId()} to slot {slot}", LogLevel.Debug);
		return true;
	}

	/// <inheritdoc/>
	public bool RemoveItem(int slot)
	{
		if (!IsValidSlot(slot))
			return false;

		if (_items.Remove(slot))
		{
			Logger.Log($"[BANK] Removed item from slot {slot}", LogLevel.Debug);
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public void UpdateItemAtSlot(int slot, IItem item)
	{
		if (IsValidSlot(slot))
		{
			_items[slot] = item;
			Logger.Log($"[BANK] Updated item at slot {slot}", LogLevel.Debug);
		}
	}

	/// <inheritdoc/>
	public int FindEmptySlot()
	{
		for (int i = 0; i < _maxSlots; i++)
		{
			if (!_items.ContainsKey(i))
				return i;
		}
		return -1;
	}

	/// <inheritdoc/>
	public bool IsValidSlot(int slot)
	{
		return slot >= 0 && slot < _maxSlots;
	}

	/// <inheritdoc/>
	public void ExpandSlots(int additionalSlots)
	{
		int newMax = _maxSlots + additionalSlots;
		_maxSlots = Math.Min(newMax, MaxExpandedSlots);
		Logger.Log($"[BANK] Expanded to {_maxSlots} slots", LogLevel.Debug);
	}

	/// <inheritdoc/>
	public Dictionary<int, IItem> GetAllItems()
	{
		return new Dictionary<int, IItem>(_items);
	}

	/// <summary>
	/// Loads bank items from persistence data.
	/// </summary>
	/// <param name="items">Dictionary of slot -> item from database</param>
	/// <param name="maxSlots">Number of unlocked slots</param>
	public void LoadFromPersistence(Dictionary<int, IItem> items, int maxSlots)
	{
		_items.Clear();
		_maxSlots = Math.Min(maxSlots, MaxExpandedSlots);

		foreach (var kvp in items)
		{
			if (IsValidSlot(kvp.Key))
			{
				_items[kvp.Key] = kvp.Value;
			}
		}

		Logger.Log($"[BANK] Loaded {_items.Count} items, {_maxSlots} slots", LogLevel.Debug);
	}
}
