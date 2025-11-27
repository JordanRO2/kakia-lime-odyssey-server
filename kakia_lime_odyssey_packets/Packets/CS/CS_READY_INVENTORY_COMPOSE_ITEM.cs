/// <summary>
/// Client->Server packet to prepare for item composition/enchantment.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_READY_INVENTORY_COMPOSE_ITEM
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Response: SC_READY_INVENTORY_COMPOSE_ITEM
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_READY_INVENTORY_COMPOSE_ITEM
{
	/// <summary>Inventory slot containing the item to compose</summary>
	public int slot;
}
