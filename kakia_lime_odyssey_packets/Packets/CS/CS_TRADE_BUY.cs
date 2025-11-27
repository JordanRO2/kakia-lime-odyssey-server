/// <summary>
/// Client->Server packet to buy an item from NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_TRADE_BUY
/// Size: 16 bytes (18 with PACKET_FIX header)
/// Response: SC_TRADE_BOUGHT_SOLD_ITEMS
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_TRADE_BUY
{
	/// <summary>Item type ID to purchase from merchant</summary>
	public int itemTypeID;

	/// <summary>Quantity to purchase</summary>
	public long count;

	/// <summary>Target inventory slot for the purchased item</summary>
	public int slot;
}
