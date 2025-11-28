using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when using an inventory item on self.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_SELF
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes) - inventory slot index
/// Use case: Using consumable items on self (e.g., potions, food, self-buff items)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_INVENTORY_ITEM_SELF : IPacketFixed
{
	public int slot;
}
