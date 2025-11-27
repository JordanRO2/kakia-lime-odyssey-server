/// <summary>
/// Server->Client packet to prompt life job selection (variable length packet).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHOICE_LIFE_JOB
/// Size: 8 bytes + variable (12 with PACKET_VAR header)
/// Triggered by: Life job selection prompt
/// Note: Uses PACKET_VAR (4 bytes) instead of PACKET_FIX (2 bytes)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CHOICE_LIFE_JOB
{
	/// <summary>Instance ID of the object triggering life job choice</summary>
	public long objInstID;
}
