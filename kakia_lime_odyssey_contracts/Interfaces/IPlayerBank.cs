namespace kakia_lime_odyssey_contracts.Interfaces;

/// <summary>
/// Interface for player bank storage operations.
/// </summary>
public interface IPlayerBank
{
	/// <summary>Gets the maximum number of bank slots the player has unlocked.</summary>
	int MaxSlots { get; }

	/// <summary>Gets the number of items currently in the bank.</summary>
	int ItemCount { get; }

	/// <summary>Checks if the bank is currently open (player at banker NPC).</summary>
	bool IsOpen { get; }

	/// <summary>Opens the bank for the player.</summary>
	void Open();

	/// <summary>Closes the bank for the player.</summary>
	void Close();

	/// <summary>Gets an item at the specified bank slot.</summary>
	/// <param name="slot">The bank slot index</param>
	/// <returns>The item at the slot, or null if empty</returns>
	IItem? AtSlot(int slot);

	/// <summary>Adds an item to the bank at a specific slot.</summary>
	/// <param name="item">The item to add</param>
	/// <param name="slot">The bank slot index</param>
	/// <returns>True if successful, false if slot is occupied or invalid</returns>
	bool AddItem(IItem item, int slot);

	/// <summary>Removes an item from a bank slot.</summary>
	/// <param name="slot">The bank slot index</param>
	/// <returns>True if an item was removed, false if slot was empty</returns>
	bool RemoveItem(int slot);

	/// <summary>Updates an item at a bank slot.</summary>
	/// <param name="slot">The bank slot index</param>
	/// <param name="item">The updated item</param>
	void UpdateItemAtSlot(int slot, IItem item);

	/// <summary>Finds the first empty bank slot.</summary>
	/// <returns>The slot index, or -1 if no empty slots</returns>
	int FindEmptySlot();

	/// <summary>Checks if a slot is valid and within the player's unlocked slots.</summary>
	/// <param name="slot">The bank slot index</param>
	/// <returns>True if valid, false otherwise</returns>
	bool IsValidSlot(int slot);

	/// <summary>Expands the bank by a number of slots.</summary>
	/// <param name="additionalSlots">Number of slots to add</param>
	void ExpandSlots(int additionalSlots);

	/// <summary>Gets all items in the bank.</summary>
	/// <returns>Dictionary of slot -> item</returns>
	Dictionary<int, IItem> GetAllItems();
}
