using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing list of learnable skills.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LEARNABLE_SKILL_LIST
/// Size: 5 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned __int8 jobClass (1 byte)
/// Note: Variable-length - skill list data follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LEARNABLE_SKILL_LIST : IPacketVar
{
	/// <summary>Job class for skill list (offset 0x04)</summary>
	public byte jobClass;

	// Variable length skill list data follows (not included in struct)
}
