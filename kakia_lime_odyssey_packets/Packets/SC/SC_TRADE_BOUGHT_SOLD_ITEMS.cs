/// <summary>
/// Server->Client packet confirming successful buy/sell transaction.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_TRADE_BOUGHT_SOLD_ITEMS
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Triggered by: CS_TRADE_BUY, CS_TRADE_SELL, CS_TRADE_BUY_SOLD_ITEMS
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_TRADE_BOUGHT_SOLD_ITEMS
{
	/// <summary>Inventory slot affected by the transaction</summary>
	public int slot;
}
