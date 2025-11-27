using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to prepare for item composition/enchantment.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_READY_INVENTORY_COMPOSE_ITEM
/// Size: 6 bytes total (2-byte header + 4-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes)
/// Response: SC_READY_INVENTORY_COMPOSE_ITEM
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_READY_INVENTORY_COMPOSE_ITEM : IPacketFixed
{
	/// <summary>Inventory slot containing the item to compose</summary>
	public int slot;
}
