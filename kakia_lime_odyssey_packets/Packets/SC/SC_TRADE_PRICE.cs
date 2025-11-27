using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing sell price for an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_TRADE_PRICE
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes)
/// - 0x06: __int64 price (8 bytes)
/// Triggered by: CS_REQUEST_TRADE_PRICE
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TRADE_PRICE : IPacketFixed
{
	/// <summary>Inventory slot of the item being priced (offset 0x02)</summary>
	public int slot;

	/// <summary>Amount of currency merchant will pay for the item (offset 0x06)</summary>
	public long price;
}
