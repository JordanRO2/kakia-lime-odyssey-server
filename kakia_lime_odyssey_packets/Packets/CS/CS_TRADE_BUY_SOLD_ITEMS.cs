/// <summary>
/// Client->Server packet to buy back previously sold items from NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_TRADE_BUY_SOLD_ITEMS
/// Size: 0 bytes (2 with PACKET_FIX header only)
/// Response: SC_TRADE_BOUGHT_SOLD_ITEMS
/// Note: Allows players to repurchase items they recently sold to the merchant
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_TRADE_BUY_SOLD_ITEMS
{
	// Empty packet - header only
}
