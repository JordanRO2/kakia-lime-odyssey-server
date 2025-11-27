/// <summary>
/// Client packet requesting to download a file from the server.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DOWNLOAD_FILE
/// Size: 2 bytes
/// Ordinal: 2417
/// Simple request packet with no payload beyond the header.
/// Server responds with SC_START_DOWNLOAD_FILE.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_DOWNLOAD_FILE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
