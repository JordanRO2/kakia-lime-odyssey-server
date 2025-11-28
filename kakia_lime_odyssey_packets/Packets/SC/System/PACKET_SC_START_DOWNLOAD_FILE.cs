/// <summary>
/// Server packet initiating a file download to the client.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_START_DOWNLOAD_FILE
/// Size: 2 bytes
/// Ordinal: 2418
/// Simple acknowledgment packet with no payload beyond the header.
/// Sent in response to CS_DOWNLOAD_FILE to begin file download process.
/// Client should respond with CS_DOWNLOAD_NEXT_FILE_BLOCK to request first block.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_START_DOWNLOAD_FILE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
