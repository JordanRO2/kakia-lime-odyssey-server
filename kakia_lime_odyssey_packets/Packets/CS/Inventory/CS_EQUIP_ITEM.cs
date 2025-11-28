using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_CS_EQUIP_ITEM
/// Size: 7 bytes (0x07)
/// Client sends this to equip an item from inventory slot to equipment slot
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EQUIP_ITEM : IPacketFixed
{
	public int invSlot;        // Offset: 0x02 - Inventory slot index
	public byte equipSlot;     // Offset: 0x06 - Equipment slot index
}
