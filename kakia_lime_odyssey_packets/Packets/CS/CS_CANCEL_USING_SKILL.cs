/// <summary>
/// Client->Server packet to cancel skill currently being cast.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CANCEL_USING_SKILL
/// Size: 0 bytes (2 with PACKET_FIX header)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_CANCEL_USING_SKILL
{
	// Header only - no payload
}
