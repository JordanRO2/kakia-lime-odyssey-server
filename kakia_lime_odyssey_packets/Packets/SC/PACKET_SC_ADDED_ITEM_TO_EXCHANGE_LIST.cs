/// <summary>
/// Server packet sent to show the other player's added item with full details.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ADDED_ITEM_TO_EXCHANGE_LIST
/// Size: 330 bytes
/// Ordinal: 2745
/// Fields:
/// - itemTypeID (offset 0x02, 4 bytes): Item type identifier
/// - slot (offset 0x06, 4 bytes): Exchange slot number
/// - count (offset 0x0A, 8 bytes): Number of items
/// - durability (offset 0x12, 4 bytes): Current durability
/// - mdurability (offset 0x16, 4 bytes): Maximum durability
/// - grade (offset 0x1A, 4 bytes): Item grade/quality
/// - inherits (offset 0x1E, 300 bytes): Item enhancement/socket data
/// Sent to the other player to display the traded item.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_ADDED_ITEM_TO_EXCHANGE_LIST
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Item type identifier</summary>
	public int itemTypeID;

	/// <summary>Exchange slot number</summary>
	public int slot;

	/// <summary>Number of items in the stack</summary>
	public long count;

	/// <summary>Current durability of the item</summary>
	public int durability;

	/// <summary>Maximum durability of the item</summary>
	public int mdurability;

	/// <summary>Item grade/quality level</summary>
	public int grade;

	/// <summary>Item enhancement, sockets, and inheritance data</summary>
	public ITEM_INHERITS inherits;
}
