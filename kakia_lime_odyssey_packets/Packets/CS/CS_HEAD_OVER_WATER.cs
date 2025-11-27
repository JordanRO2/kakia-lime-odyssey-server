/// <summary>
/// Client->Server packet when player's head surfaces above water.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_HEAD_OVER_WATER
/// Size: 0 bytes (2 with PACKET_FIX header)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_HEAD_OVER_WATER
{
	// Header only - no payload
}
