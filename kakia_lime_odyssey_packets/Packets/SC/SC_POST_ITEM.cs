using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_SC_POST_ITEM @ 6 bytes
/// Server sends this when client successfully takes item from mail/post.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_POST_ITEM
{
	public int indexNumber;
}
