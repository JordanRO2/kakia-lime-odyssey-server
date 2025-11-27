/// <summary>
/// Server packet confirming item was successfully removed from local player's exchange list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SUCCESS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST
/// Size: 18 bytes
/// Ordinal: 2747
/// Fields:
/// - itemTypeID (offset 0x02, 4 bytes): Item type identifier
/// - slot (offset 0x06, 4 bytes): Exchange slot number
/// - count (offset 0x0A, 8 bytes): Number of items removed
/// Sent to the player who removed the item.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SUCCESS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Item type identifier</summary>
	public int itemTypeID;

	/// <summary>Exchange slot number</summary>
	public int slot;

	/// <summary>Number of items removed from the exchange</summary>
	public long count;
}
