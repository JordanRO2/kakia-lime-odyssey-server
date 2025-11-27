/// <summary>
/// Client->Server packet to request player-to-player exchange.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_REQUEST_EXCHANGE
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Note: Different from NPC trade - this is P2P exchange
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_EXCHANGE
{
	// Header only - no payload
}
