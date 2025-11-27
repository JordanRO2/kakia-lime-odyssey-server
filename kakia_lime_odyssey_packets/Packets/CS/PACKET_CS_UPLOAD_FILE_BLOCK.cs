/// <summary>
/// Client packet sending a block of data for a file being uploaded.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_UPLOAD_FILE_BLOCK
/// Size: 2 bytes
/// Ordinal: 2423
/// Simple request packet with no payload beyond the header.
/// Server responds with SC_UPLOAD_NEXT_FILE_BLOCK to acknowledge and request next block.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_UPLOAD_FILE_BLOCK
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
