using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-27
/// Packet: PACKET_CS_TAKE_POST_ITEM @ 6 bytes (2 byte header + 4 byte payload)
/// Client sends this to take/claim an item from a mail/post message.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_TAKE_POST_ITEM : IPacketFixed
{
	/// <summary>Index number of the post item to take</summary>
	public int indexNumber;
}
