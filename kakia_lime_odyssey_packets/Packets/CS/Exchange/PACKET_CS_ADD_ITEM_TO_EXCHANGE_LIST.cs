/// <summary>
/// Client packet sent to add an item from inventory to the exchange window.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ADD_ITEM_TO_EXCHANGE_LIST
/// Size: 14 bytes
/// Ordinal: 2743
/// Fields:
/// - slot (offset 0x02, 4 bytes): Inventory slot number
/// - count (offset 0x06, 8 bytes): Number of items to add
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_ADD_ITEM_TO_EXCHANGE_LIST
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Inventory slot number of the item to add</summary>
	public int slot;

	/// <summary>Number of items to add to the exchange</summary>
	public long count;
}
