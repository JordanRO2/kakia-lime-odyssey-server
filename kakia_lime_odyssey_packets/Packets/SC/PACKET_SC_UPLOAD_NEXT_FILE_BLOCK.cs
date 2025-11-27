/// <summary>
/// Server packet acknowledging receipt of upload block and requesting next block.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_UPLOAD_NEXT_FILE_BLOCK
/// Size: 2 bytes
/// Ordinal: 2424
/// Simple acknowledgment packet with no payload beyond the header.
/// Sent in response to CS_UPLOAD_FILE_BLOCK to acknowledge data received.
/// Client should send next CS_UPLOAD_FILE_BLOCK until upload is complete.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_UPLOAD_NEXT_FILE_BLOCK
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
