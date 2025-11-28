using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Represents an item available for purchase from an NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: TRADE_ITEM
/// Size: 324 bytes
/// Memory Layout (IDA):
/// - 0x00: int typeID (4 bytes) - Item type identifier
/// - 0x04: unsigned int count (4 bytes) - Quantity available
/// - 0x08: unsigned int price (4 bytes) - Item price in Peder
/// - 0x0C: int durability (4 bytes) - Current durability
/// - 0x10: int mdurability (4 bytes) - Max durability
/// - 0x14: int grade (4 bytes) - Item grade/enchant level
/// - 0x18: ITEM_INHERITS inherits (300 bytes) - Item inherit effects
/// Used in: SC_TRADE_DESC item list
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TRADE_ITEM
{
	/// <summary>Item type identifier from ItemInfo (offset 0x00)</summary>
	public int typeID;

	/// <summary>Quantity available for purchase (offset 0x04)</summary>
	public uint count;

	/// <summary>Item price in Peder currency (offset 0x08)</summary>
	public uint price;

	/// <summary>Current durability of the item (offset 0x0C)</summary>
	public int durability;

	/// <summary>Maximum durability of the item (offset 0x10)</summary>
	public int mdurability;

	/// <summary>Item grade/enchant level (offset 0x14)</summary>
	public int grade;

	/// <summary>Item inherit effects and bonuses (offset 0x18, 300 bytes)</summary>
	public ITEM_INHERITS inherits;
}
