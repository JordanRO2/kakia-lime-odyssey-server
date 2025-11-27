using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_SC_DELETED_POST @ 6 bytes
/// Server sends this to confirm mail/post message deletion.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DELETED_POST
{
	public int indexNumber;
}
