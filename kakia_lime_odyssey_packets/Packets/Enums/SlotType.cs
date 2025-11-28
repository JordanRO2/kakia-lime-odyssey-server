namespace kakia_lime_odyssey_packets.Packets.Enums;

/// <summary>
/// Defines the type of item slot container.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Used in ITEM_SLOT.type field to differentiate between inventory, bank, equipment etc.
/// </remarks>
public enum SlotType : byte
{
	/// <summary>Player inventory (main bag)</summary>
	Inventory = 0,

	/// <summary>Equipment slots (combat/life job)</summary>
	Equipment = 1,

	/// <summary>Bank storage</summary>
	Bank = 2,

	/// <summary>Pet inventory</summary>
	Pet = 3,

	/// <summary>Trade window slots</summary>
	Trade = 4,

	/// <summary>Exchange window slots</summary>
	Exchange = 5
}
