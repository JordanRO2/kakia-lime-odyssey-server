using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-27
/// Packet: PACKET_CS_REQUEST_POST @ 6 bytes (2 byte header + 4 byte payload)
/// Client sends this to request mail/post details by index number.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_REQUEST_POST : IPacketFixed
{
	/// <summary>Index number of the post to request</summary>
	public int indexNumber;
}
