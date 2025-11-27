/// <summary>
/// Client->Server packet requesting return to character select lobby.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_RETURN_LOBBY
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Response: SC_REENTER_LOBBY
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_RETURN_LOBBY
{
	// Header only - no payload
}
