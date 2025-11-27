using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when client successfully takes item from mail/post.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_POST_ITEM
/// Size: 6 bytes total (2-byte header + 4-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int indexNumber (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_POST_ITEM : IPacketFixed
{
	/// <summary>Mail index number from which item was taken (1-based)</summary>
	public int indexNumber;
}
