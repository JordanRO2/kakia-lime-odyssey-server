using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to prompt life job selection (variable length packet).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHOICE_LIFE_JOB
/// Size: 12 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 objInstID (8 bytes)
/// Note: Uses PACKET_VAR (4 bytes) instead of PACKET_FIX (2 bytes)
/// Triggered by: Life job selection prompt
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHOICE_LIFE_JOB : IPacketVar
{
	/// <summary>Instance ID of the object triggering life job choice (offset 0x04)</summary>
	public long objInstID;
}
