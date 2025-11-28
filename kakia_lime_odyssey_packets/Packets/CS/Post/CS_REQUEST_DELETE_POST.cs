using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-27
/// Packet: PACKET_CS_REQUEST_DELETE_POST @ 6 bytes (2 byte header + 4 byte payload)
/// Client sends this to request deletion of a mail/post message.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_REQUEST_DELETE_POST : IPacketFixed
{
	/// <summary>Index number of the post to delete</summary>
	public int indexNumber;
}
