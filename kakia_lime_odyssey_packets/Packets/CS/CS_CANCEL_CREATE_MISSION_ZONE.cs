/// <summary>
/// Client->Server packet to cancel mission zone creation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// Size: 0 bytes (2 with PACKET_FIX header)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_CANCEL_CREATE_MISSION_ZONE
{
	// Header only - no payload
}
