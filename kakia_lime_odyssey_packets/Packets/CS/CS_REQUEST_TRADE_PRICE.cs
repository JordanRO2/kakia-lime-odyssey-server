/// <summary>
/// Client->Server packet to request the price for selling an item to NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_REQUEST_TRADE_PRICE
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Response: SC_TRADE_PRICE
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_TRADE_PRICE
{
	/// <summary>Inventory slot of the item to get sell price for</summary>
	public int slot;
}
