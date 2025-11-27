/// <summary>
/// Server packet acknowledging file upload initiation from client.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_START_UPLOAD_FILE
/// Size: 2 bytes
/// Ordinal: 2422
/// Simple acknowledgment packet with no payload beyond the header.
/// Sent in response to CS_UPLOAD_FILE to allow client to begin uploading.
/// Client should send CS_UPLOAD_FILE_BLOCK with first data chunk.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_START_UPLOAD_FILE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
