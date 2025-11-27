using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing crafting recipe details and success rates.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ITEM_MAKE_UPDATE_REPORT
/// Size: 652 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// - 0x06: int successPercent (4 bytes)
/// - 0x0A: int criticalSuccessPercent (4 bytes)
/// - 0x0E: unsigned int makeTime (4 bytes)
/// - 0x12: unsigned __int16 requestLP (2 bytes)
/// - 0x14: int itemTypeID (4 bytes)
/// - 0x18: unsigned int count (4 bytes)
/// - 0x1C: int durability (4 bytes)
/// - 0x20: int mdurability (4 bytes)
/// - 0x24: ITEM_INHERITS inherits (300 bytes)
/// - 0x150: int criticalItemTypeID (4 bytes)
/// - 0x154: unsigned int criticalCount (4 bytes)
/// - 0x158: int criticalDurability (4 bytes)
/// - 0x15C: int criticalMdurability (4 bytes)
/// - 0x160: ITEM_INHERITS criticalInherits (300 bytes)
/// Triggered by: CS_ITEM_MAKE_READY, CS_ITEM_MAKE_REQUEST_REPORT
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ITEM_MAKE_UPDATE_REPORT : IPacketFixed
{
	/// <summary>Recipe type ID (offset 0x02)</summary>
	public uint typeID;

	/// <summary>Success percentage 0-100 (offset 0x06)</summary>
	public int successPercent;

	/// <summary>Critical success percentage 0-100 (offset 0x0A)</summary>
	public int criticalSuccessPercent;

	/// <summary>Time required to craft in milliseconds (offset 0x0E)</summary>
	public uint makeTime;

	/// <summary>Life Points (LP) cost for crafting (offset 0x12)</summary>
	public ushort requestLP;

	/// <summary>Item type ID of the normal result (offset 0x14)</summary>
	public int itemTypeID;

	/// <summary>Number of items produced on normal success (offset 0x18)</summary>
	public uint count;

	/// <summary>Durability of the normal result item (offset 0x1C)</summary>
	public int durability;

	/// <summary>Maximum durability of the normal result item (offset 0x20)</summary>
	public int mdurability;

	/// <summary>Inherit properties for normal result (offset 0x24)</summary>
	public ITEM_INHERITS inherits;

	/// <summary>Item type ID of the critical success result (offset 0x150)</summary>
	public int criticalItemTypeID;

	/// <summary>Number of items produced on critical success (offset 0x154)</summary>
	public uint criticalCount;

	/// <summary>Durability of the critical result item (offset 0x158)</summary>
	public int criticalDurability;

	/// <summary>Maximum durability of the critical result item (offset 0x15C)</summary>
	public int criticalMdurability;

	/// <summary>Inherit properties for critical result (offset 0x160)</summary>
	public ITEM_INHERITS criticalInherits;
}
