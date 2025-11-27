/// <summary>
/// Client->Server request to start looting (without selecting target first).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_REQUEST_START_LOOTING
/// Size: 2 bytes (PACKET_FIX header only, no additional fields)
/// Response: SC_START_LOOTING
/// Note: This is used when the target is already selected or implied from context.
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_START_LOOTING : IPacketFixed
{
	// This packet has no fields beyond the PACKET_FIX header
}
