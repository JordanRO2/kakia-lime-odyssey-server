using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that a new item was inserted into a slot.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_INSERT_SLOT_ITEM
/// Size: 335 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: ITEM_SLOT slot (5 bytes) - Target slot location
/// - 0x07: int typeID (4 bytes) - Item type identifier
/// - 0x0B: __int64 count (8 bytes) - Stack count
/// - 0x13: int durability (4 bytes) - Current durability
/// - 0x17: int mdurability (4 bytes) - Max durability
/// - 0x1B: int remainExpiryTime (4 bytes) - Time until expiration
/// - 0x1F: int grade (4 bytes) - Item grade/quality
/// - 0x23: ITEM_INHERITS inherits (300 bytes) - Item bonuses/enchants
/// Triggered by: Item pickup, loot, trade receive, quest reward
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_INSERT_SLOT_ITEM : IPacketFixed
{
	/// <summary>Target slot location (offset 0x02)</summary>
	public ITEM_SLOT slot;

	/// <summary>Item type identifier (offset 0x07)</summary>
	public int typeID;

	/// <summary>Stack count (offset 0x0B)</summary>
	public long count;

	/// <summary>Current durability (offset 0x13)</summary>
	public int durability;

	/// <summary>Max durability (offset 0x17)</summary>
	public int mdurability;

	/// <summary>Time until expiration in seconds (offset 0x1B)</summary>
	public int remainExpiryTime;

	/// <summary>Item grade/quality level (offset 0x1F)</summary>
	public int grade;

	/// <summary>Item bonuses and enchantments (offset 0x23)</summary>
	public ITEM_INHERITS inherits;
}
