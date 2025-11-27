/// <summary>
/// Client->Server packet to request file download.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_DOWNLOAD_FILE
/// Size: 2 bytes (header-only)
/// Response: SC_START_DOWNLOAD_FILE
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DOWNLOAD_FILE : IPacketFixed
{
	// Header-only packet - no additional fields
}
