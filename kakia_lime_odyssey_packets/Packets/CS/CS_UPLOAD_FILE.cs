/// <summary>
/// Client->Server packet to initiate file upload.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_UPLOAD_FILE
/// Size: 2 bytes (header-only)
/// Response: SC_START_UPLOAD_FILE
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_UPLOAD_FILE : IPacketFixed
{
	// Header-only packet - no additional fields
}
