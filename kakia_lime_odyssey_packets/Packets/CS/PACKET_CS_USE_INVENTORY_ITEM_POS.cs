/// <summary>
/// Client packet requesting to use an inventory item at a specific position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_POS
/// Size: 18 bytes
/// Ordinal: 2655
/// Sent when using an item targeted at a world position (e.g., placing traps, AoE items).
/// Server responds with SC_USE_ITEM_POS_RESULT_LIST.
/// Requires database update to inventory.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_USE_INVENTORY_ITEM_POS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Inventory slot containing the item to use</summary>
	public int slot;

	/// <summary>Target position in world coordinates</summary>
	public FPOS pos;
}
