using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing mail list count.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_POST_LIST
/// Size: 5 bytes total (4-byte header + 1-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned __int8 count (1 byte)
/// Followed by: count * POST_LIST entries (88 bytes each)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_POST_LIST : IPacketVar
{
	/// <summary>Number of mail entries in the list</summary>
	public byte count;
}
