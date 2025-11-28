/// <summary>
/// Client->Server packet to request next file block during download.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_DOWNLOAD_NEXT_FILE_BLOCK
/// Size: 2 bytes (header-only)
/// Response: SC_DOWNLOAD_FILE_BLOCK
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DOWNLOAD_NEXT_FILE_BLOCK : IPacketFixed
{
	// Header-only packet - no additional fields
}
