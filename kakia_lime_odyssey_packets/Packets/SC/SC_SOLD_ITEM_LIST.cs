using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing list of items player recently sold to merchant.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SOLD_ITEM_LIST
/// Size: 4 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// Note: Variable-length packet - contains array of sold items that can be bought back
/// Triggered by: CS_REQUEST_SOLD_ITEMS
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SOLD_ITEM_LIST : IPacketVar
{
	// Variable-length packet - header only
	// Followed by array of sold items (handled separately)
}
