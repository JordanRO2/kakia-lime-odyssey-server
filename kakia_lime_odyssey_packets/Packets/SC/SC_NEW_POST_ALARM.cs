using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_SC_NEW_POST_ALARM @ 6 bytes
/// Server sends this to notify client of new mail received.
/// Contains count of new/unread messages.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_NEW_POST_ALARM
{
	public int count;
}
