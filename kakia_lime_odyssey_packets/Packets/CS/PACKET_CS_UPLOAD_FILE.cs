/// <summary>
/// Client packet initiating a file upload to the server.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_UPLOAD_FILE
/// Size: 2 bytes
/// Ordinal: 2421
/// Simple request packet with no payload beyond the header.
/// Server responds with SC_START_UPLOAD_FILE to begin the upload process.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_UPLOAD_FILE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
