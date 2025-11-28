using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for combat job equipment list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_EQUIPMENT_LIST
/// Size: 4 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// Note: Variable-length equipment list data follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_COMBAT_JOB_EQUIPMENT_LIST : IPacketVar
{
	/// <summary>
	/// Variable-length array of equipment (not part of struct layout, handled separately)
	/// This field is not marshaled - it's for C# convenience only
	/// </summary>
	[field: NonSerialized]
	public EQUIPMENT[] equips { get; set; }
}

/// <summary>
/// Server->Client packet for life job equipment list.
/// </summary>
/// <remarks>
/// Variable-length packet with equipment list data
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LIFE_JOB_EQUIPMENT_LIST : IPacketVar
{
	/// <summary>
	/// Variable-length array of equipment (not part of struct layout, handled separately)
	/// This field is not marshaled - it's for C# convenience only
	/// </summary>
	[field: NonSerialized]
	public EQUIPMENT[] equips { get; set; }
}
