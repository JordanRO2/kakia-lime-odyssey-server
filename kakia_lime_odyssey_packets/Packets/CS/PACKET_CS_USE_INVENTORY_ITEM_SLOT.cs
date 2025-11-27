/// <summary>
/// Client packet requesting to use an inventory item on another inventory slot.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_SLOT
/// Size: 10 bytes
/// Ordinal: 2659
/// Sent when using an item on another item in inventory (e.g., combining, upgrading).
/// Server responds with SC_USE_ITEM_SLOT_RESULT.
/// Requires database update to inventory.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_USE_INVENTORY_ITEM_SLOT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Inventory slot containing the item to use</summary>
	public int slot;

	/// <summary>Target inventory slot</summary>
	public int targetSlot;
}
