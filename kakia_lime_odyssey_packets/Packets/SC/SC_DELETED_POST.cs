using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to confirm mail/post message deletion.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DELETED_POST
/// Size: 6 bytes total (2-byte header + 4-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int indexNumber (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DELETED_POST : IPacketFixed
{
	/// <summary>Mail index number that was deleted (1-based)</summary>
	public int indexNumber;
}
