/// <summary>
/// Client packet sent to remove an item from the exchange window.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST
/// Size: 14 bytes
/// Ordinal: 2746
/// Fields:
/// - slot (offset 0x02, 4 bytes): Exchange slot number
/// - count (offset 0x06, 8 bytes): Number of items to remove
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Exchange slot number of the item to remove</summary>
	public int slot;

	/// <summary>Number of items to remove from the exchange</summary>
	public long count;
}
