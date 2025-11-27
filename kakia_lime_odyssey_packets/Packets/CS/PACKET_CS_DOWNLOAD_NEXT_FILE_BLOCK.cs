/// <summary>
/// Client packet requesting the next block of a file being downloaded.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DOWNLOAD_NEXT_FILE_BLOCK
/// Size: 2 bytes
/// Ordinal: 2420
/// Simple request packet with no payload beyond the header.
/// Server responds with SC_DOWNLOAD_FILE_BLOCK containing the next chunk.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_DOWNLOAD_NEXT_FILE_BLOCK
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
