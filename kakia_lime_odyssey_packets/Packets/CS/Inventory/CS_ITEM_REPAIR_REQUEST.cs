using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_CS_ITEM_REPAIR_REQUEST
/// Size: 7 bytes (0x07)
/// Client requests to repair an item
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_ITEM_REPAIR_REQUEST : IPacketFixed
{
	public bool isEquiped;     // Offset: 0x02 - True if item is equipped, false if in inventory
	public int index;          // Offset: 0x03 - Slot index (equipment or inventory)
}
