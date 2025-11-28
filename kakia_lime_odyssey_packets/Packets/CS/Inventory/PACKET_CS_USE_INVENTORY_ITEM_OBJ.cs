/// <summary>
/// Client packet requesting to use an inventory item on a target object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_OBJ
/// Size: 14 bytes
/// Ordinal: 2656
/// Sent when using an item on a target entity (e.g., giving food to pet, using item on NPC).
/// Server responds with SC_USE_ITEM_OBJ_RESULT_LIST.
/// Requires database update to inventory.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_USE_INVENTORY_ITEM_OBJ
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Inventory slot containing the item to use</summary>
	public int slot;

	/// <summary>Instance ID of the target object</summary>
	public long targetInstID;
}
