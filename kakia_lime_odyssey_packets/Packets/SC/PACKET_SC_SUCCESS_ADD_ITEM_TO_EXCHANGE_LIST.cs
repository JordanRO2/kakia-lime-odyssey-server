/// <summary>
/// Server packet confirming item was successfully added to local player's exchange list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SUCCESS_ADD_ITEM_TO_EXCHANGE_LIST
/// Size: 18 bytes
/// Ordinal: 2744
/// Fields:
/// - itemTypeID (offset 0x02, 4 bytes): Item type identifier
/// - slot (offset 0x06, 4 bytes): Exchange slot number
/// - count (offset 0x0A, 8 bytes): Number of items added
/// Sent to the player who added the item.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SUCCESS_ADD_ITEM_TO_EXCHANGE_LIST : IPacketFixed
{
	/// <summary>Item type identifier</summary>
	public int itemTypeID;

	/// <summary>Exchange slot number where item was placed</summary>
	public int slot;

	/// <summary>Number of items added to the exchange</summary>
	public long count;
}
