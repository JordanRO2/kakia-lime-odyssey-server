using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing detailed item slot information.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SLOT_ITEM_INFO
/// Size: 335 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: ITEM_SLOT slot (5 bytes)
/// - 0x07: int typeID (4 bytes)
/// - 0x0B: __int64 count (8 bytes)
/// - 0x13: int durability (4 bytes)
/// - 0x17: int mdurability (4 bytes)
/// - 0x1B: int grade (4 bytes)
/// - 0x1F: int remainExpiryTime (4 bytes)
/// - 0x23: ITEM_INHERITS inherits (300 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SLOT_ITEM_INFO : IPacketFixed
{
	/// <summary>Item slot info (offset 0x02)</summary>
	public ITEM_SLOT slot;

	/// <summary>Item type ID (offset 0x07)</summary>
	public int typeID;

	/// <summary>Item count (offset 0x0B)</summary>
	public long count;

	/// <summary>Current durability (offset 0x13)</summary>
	public int durability;

	/// <summary>Max durability (offset 0x17)</summary>
	public int mdurability;

	/// <summary>Item grade (offset 0x1B)</summary>
	public int grade;

	/// <summary>Remaining expiry time (offset 0x1F)</summary>
	public int remainExpiryTime;

	/// <summary>Item inheritance/enchant data (offset 0x23)</summary>
	public ITEM_INHERITS inherits;
}
