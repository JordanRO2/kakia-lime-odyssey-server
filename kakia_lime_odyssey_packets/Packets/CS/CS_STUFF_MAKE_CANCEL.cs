/// <summary>
/// Client->Server packet to cancel material gathering/processing.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_CANCEL
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Response: SC_STUFF_MAKE_FINISH
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_STUFF_MAKE_CANCEL
{
	// Header only - no payload
}
