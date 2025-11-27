/// <summary>
/// Server->Client packet containing list of items player recently sold to merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SOLD_ITEM_LIST
/// Size: 0 bytes (4 with PACKET_VAR header only)
/// Triggered by: CS_REQUEST_SOLD_ITEMS
/// Note: Variable-length packet - contains array of sold items that can be bought back
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_SOLD_ITEM_LIST
{
	// Variable-length packet - header only
	// Followed by array of sold items (handled separately)
}
