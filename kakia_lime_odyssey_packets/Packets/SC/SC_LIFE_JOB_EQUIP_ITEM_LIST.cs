using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing life job equipped items list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Note: Uses same structure as SC_EQUIP_ITEM_LIST for life job equipment
/// Size: 4 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// Note: Variable-length array of EQUIP_ITEM (320 bytes each) follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LIFE_JOB_EQUIP_ITEM_LIST : IPacketVar
{
	/// <summary>
	/// Variable-length array of equipped items (not part of struct layout, handled separately)
	/// This field is not marshaled - it's for C# convenience only
	/// </summary>
	[field: NonSerialized]
	public EQUIP_ITEM[] equipList { get; set; }
}
