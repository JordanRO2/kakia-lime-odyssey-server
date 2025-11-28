using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for private chatroom message broadcast.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PRIVATE_CHATROOM_SAY
/// Size: 12 bytes total (header + fixed fields)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 instID (8 bytes)
/// Note: Variable-length message string follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PRIVATE_CHATROOM_SAY : IPacketVar
{
	/// <summary>Sender's instance ID (offset 0x04)</summary>
	public long instID;

	// Note: Variable-length message string follows, handled separately
}
