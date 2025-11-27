using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating success/failure of sending mail/post.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SEND_POST_RESULT
/// Size: 3 bytes total (2-byte header + 1-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: bool isSuccess (1 byte)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SEND_POST_RESULT : IPacketFixed
{
	/// <summary>Whether the mail was sent successfully</summary>
	public bool isSuccess;
}
