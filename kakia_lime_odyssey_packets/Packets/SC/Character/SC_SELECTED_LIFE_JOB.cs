using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming life job selection.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SELECTED_LIFE_JOB
/// Size: 11 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: char jobTypeID (1 byte)
/// Triggered by: CS_CHOICED_LIFE_JOB
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SELECTED_LIFE_JOB : IPacketFixed
{
	/// <summary>Instance ID of the character selecting the job (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Selected life job type ID (offset 0x0A)</summary>
	public sbyte jobTypeID;
}
