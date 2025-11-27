using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when using an inventory item on another inventory slot.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_SLOT
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes) - source inventory slot index
/// - 0x06: int targetSlot (4 bytes) - target inventory slot index
/// Use case: Using items on other items (e.g., combining items, upgrading, enchanting)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_INVENTORY_ITEM_SLOT : IPacketFixed
{
	public int slot;
	public int targetSlot;
}
