using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_CS_UNEQUIP_ITEM
/// Size: 7 bytes (0x07)
/// Client sends this to unequip an item from equipment slot to inventory slot
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_UNEQUIP_ITEM : IPacketFixed
{
	public byte equipSlot;     // Offset: 0x02 - Equipment slot index
	public int invSlot;        // Offset: 0x03 - Inventory slot index (destination)
}
