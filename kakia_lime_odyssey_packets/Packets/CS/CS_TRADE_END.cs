/// <summary>
/// Client->Server packet to close the trade window with NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_TRADE_END
/// Size: 0 bytes (2 with PACKET_FIX header only)
/// Response: SC_TRADE_END
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_TRADE_END
{
	// Empty packet - header only
}
