/// <summary>
/// Server->Client packet confirming life job selection.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SELECTED_LIFE_JOB
/// Size: 9 bytes (11 with PACKET_FIX header)
/// Triggered by: CS_CHOICED_LIFE_JOB
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_SELECTED_LIFE_JOB
{
	/// <summary>Instance ID of the character selecting the job</summary>
	public long objInstID;

	/// <summary>Selected life job type ID (signed byte in IDA but likely job enum)</summary>
	public sbyte jobTypeID;
}
