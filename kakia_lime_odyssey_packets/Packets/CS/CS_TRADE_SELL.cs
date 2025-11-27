/// <summary>
/// Client->Server packet to sell an item to NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_TRADE_SELL
/// Size: 12 bytes (14 with PACKET_FIX header)
/// Response: SC_TRADE_BOUGHT_SOLD_ITEMS
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_TRADE_SELL
{
	/// <summary>Inventory slot containing the item to sell</summary>
	public int slot;

	/// <summary>Quantity to sell</summary>
	public long count;
}
