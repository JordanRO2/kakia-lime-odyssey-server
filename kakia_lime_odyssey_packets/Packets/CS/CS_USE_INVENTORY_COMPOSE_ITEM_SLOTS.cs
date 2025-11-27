/// <summary>
/// Client->Server packet to execute item composition with materials.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS
/// Size: 24 bytes (26 with PACKET_FIX header)
/// Response: SC_INVENTORY_COMPOSE_ITEM_FINISH
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Common;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS
{
	/// <summary>Inventory slot containing the base item</summary>
	public int slot;

	/// <summary>Array of material slots for composition</summary>
	public COMPOSE_ENCHANTS targetSlots;
}
