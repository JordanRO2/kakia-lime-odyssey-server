using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming successful buy/sell transaction.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_TRADE_BOUGHT_SOLD_ITEMS
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes)
/// Triggered by: CS_TRADE_BUY, CS_TRADE_SELL, CS_TRADE_BUY_SOLD_ITEMS
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TRADE_BOUGHT_SOLD_ITEMS : IPacketFixed
{
	/// <summary>Inventory slot affected by the transaction (offset 0x02)</summary>
	public int slot;
}
