using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_SC_POST_LIST @ 5 bytes
/// Server sends this to inform client of mail/post list count.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_POST_LIST
{
	public byte count;
}
