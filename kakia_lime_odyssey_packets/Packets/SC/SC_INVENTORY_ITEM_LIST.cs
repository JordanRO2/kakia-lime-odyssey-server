using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing player inventory items.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_INVENTORY_ITEM_LIST
/// Size: 9 bytes total (header + fixed fields)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned __int8 bagNum (1 byte)
/// - 0x05: int maxCount (4 bytes)
/// Note: Variable-length array of INVENTORY_ITEM follows
/// Triggered by: CS_START_GAME, bag open
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_INVENTORY_ITEM_LIST : IPacketVar
{
	/// <summary>Bag number (offset 0x04)</summary>
	public byte bagNum;

	/// <summary>Maximum inventory slot count (offset 0x05)</summary>
	public int maxCount;

	/// <summary>Inventory grade/quality level (not in IDA struct, added for functionality)</summary>
	[field: NonSerialized]
	public byte inventoryGrade { get; set; }

	/// <summary>
	/// Variable-length array of inventory items (not part of struct layout, handled separately)
	/// This field is not marshaled - it's for C# convenience only
	/// </summary>
	[field: NonSerialized]
	public INVENTORY_ITEM[] inventory { get; set; }
}
