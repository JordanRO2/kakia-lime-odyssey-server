/// <summary>
/// Server packet containing a block of file data being downloaded.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DOWNLOAD_FILE_BLOCK
/// Size: 2 bytes (base header)
/// Ordinal: 2419
/// Fixed-size header packet. The actual file block data follows the header.
/// Sent in response to CS_DOWNLOAD_NEXT_FILE_BLOCK.
/// Client continues requesting blocks until file transfer is complete.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DOWNLOAD_FILE_BLOCK
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
