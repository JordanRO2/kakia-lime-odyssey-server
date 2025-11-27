/// <summary>
/// Server->Client packet containing sell price for an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_TRADE_PRICE
/// Size: 12 bytes (14 with PACKET_FIX header)
/// Triggered by: CS_REQUEST_TRADE_PRICE
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_TRADE_PRICE
{
	/// <summary>Inventory slot of the item being priced</summary>
	public int slot;

	/// <summary>Amount of currency merchant will pay for the item</summary>
	public long price;
}
