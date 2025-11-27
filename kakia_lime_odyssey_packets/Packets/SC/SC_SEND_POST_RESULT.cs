using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_SC_SEND_POST_RESULT @ 3 bytes
/// Server sends this to indicate success/failure of sending mail/post.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SEND_POST_RESULT
{
	public bool isSuccess;
}
