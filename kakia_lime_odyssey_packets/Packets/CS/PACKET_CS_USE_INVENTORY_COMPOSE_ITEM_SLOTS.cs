/// <summary>
/// Client packet requesting to compose/enchant an item using specified material slots.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS
/// Size: 26 bytes
/// Ordinal: 2674
/// Sent when confirming item composition/enchantment with selected materials.
/// Server responds with SC_INVENTORY_COMPOSE_ITEM_FINISH.
/// Requires database update to inventory.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Inventory slot containing the base item to compose/enchant</summary>
	public int slot;

	/// <summary>Array of up to 5 material slot indices</summary>
	public COMPOSE_ENCHANTS targetSlots;
}
