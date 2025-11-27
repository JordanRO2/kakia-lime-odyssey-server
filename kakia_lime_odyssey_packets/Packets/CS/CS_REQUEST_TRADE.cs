/// <summary>
/// Client->Server packet to open trade window with NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_REQUEST_TRADE
/// Size: 0 bytes (2 with PACKET_FIX header only)
/// Response: SC_TRADE_DESC
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_TRADE
{
	// Empty packet - header only
}
