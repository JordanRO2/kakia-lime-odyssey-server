using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to notify client of new mail received.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_NEW_POST_ALARM
/// Size: 6 bytes total (2-byte header + 4-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int count (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_NEW_POST_ALARM : IPacketFixed
{
	/// <summary>Number of new/unread mail messages</summary>
	public int count;
}
