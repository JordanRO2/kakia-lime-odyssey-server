/// <summary>
/// Client packet requesting to use an inventory item on self.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_SELF
/// Size: 6 bytes
/// Ordinal: 2658
/// Sent when using an item on the player character (e.g., potions, buffs).
/// Server responds with SC_USE_ITEM_SLOT_RESULT.
/// Requires database update to inventory.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_USE_INVENTORY_ITEM_SELF
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Inventory slot containing the item to use</summary>
	public int slot;
}
