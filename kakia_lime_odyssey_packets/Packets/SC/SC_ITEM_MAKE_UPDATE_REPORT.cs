/// <summary>
/// Server->Client packet containing crafting recipe details and success rates.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ITEM_MAKE_UPDATE_REPORT
/// Size: 650 bytes (652 with PACKET_FIX header)
/// Triggered by: CS_ITEM_MAKE_READY, CS_ITEM_MAKE_REQUEST_REPORT
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_ITEM_MAKE_UPDATE_REPORT
{
	/// <summary>Recipe type ID</summary>
	public uint typeID;

	/// <summary>Success percentage (0-100)</summary>
	public int successPercent;

	/// <summary>Critical success percentage (0-100)</summary>
	public int criticalSuccessPercent;

	/// <summary>Time required to craft in milliseconds</summary>
	public uint makeTime;

	/// <summary>Life Points (LP) cost for crafting</summary>
	public ushort requestLP;

	/// <summary>Item type ID of the normal result</summary>
	public int itemTypeID;

	/// <summary>Number of items produced on normal success</summary>
	public uint count;

	/// <summary>Durability of the normal result item</summary>
	public int durability;

	/// <summary>Maximum durability of the normal result item</summary>
	public int mdurability;

	/// <summary>Inherit properties for normal result</summary>
	public ITEM_INHERITS inherits;

	/// <summary>Item type ID of the critical success result</summary>
	public int criticalItemTypeID;

	/// <summary>Number of items produced on critical success</summary>
	public uint criticalCount;

	/// <summary>Durability of the critical result item</summary>
	public int criticalDurability;

	/// <summary>Maximum durability of the critical result item</summary>
	public int criticalMdurability;

	/// <summary>Inherit properties for critical result</summary>
	public ITEM_INHERITS criticalInherits;
}
