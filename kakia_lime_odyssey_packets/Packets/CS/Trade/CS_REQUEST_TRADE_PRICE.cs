using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to request the price for selling an item to NPC merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_REQUEST_TRADE_PRICE
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes)
/// Response: SC_TRADE_PRICE
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_REQUEST_TRADE_PRICE : IPacketFixed
{
	/// <summary>Inventory slot of the item to get sell price for (offset 0x02)</summary>
	public int slot;
}
