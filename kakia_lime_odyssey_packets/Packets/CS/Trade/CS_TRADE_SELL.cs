using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to sell an item to NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_TRADE_SELL
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes)
/// - 0x06: __int64 count (8 bytes)
/// Response: SC_TRADE_BOUGHT_SOLD_ITEMS
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_TRADE_SELL : IPacketFixed
{
	/// <summary>Inventory slot containing the item to sell (offset 0x02)</summary>
	public int slot;

	/// <summary>Quantity to sell (offset 0x06)</summary>
	public long count;
}
