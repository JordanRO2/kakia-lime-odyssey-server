using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to buy an item from NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_TRADE_BUY
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int itemTypeID (4 bytes)
/// - 0x06: __int64 count (8 bytes)
/// - 0x0E: int slot (4 bytes)
/// Response: SC_TRADE_BOUGHT_SOLD_ITEMS
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_TRADE_BUY : IPacketFixed
{
	/// <summary>Item type ID to purchase from merchant (offset 0x02)</summary>
	public int itemTypeID;

	/// <summary>Quantity to purchase (offset 0x06)</summary>
	public long count;

	/// <summary>Target inventory slot for the purchased item (offset 0x0E)</summary>
	public int slot;
}
