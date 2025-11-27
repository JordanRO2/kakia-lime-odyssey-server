using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_CS_TAKE_POST_ITEM @ 6 bytes
/// Client sends this to take/claim an item from a mail/post message.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_TAKE_POST_ITEM
{
	public int indexNumber;
}
