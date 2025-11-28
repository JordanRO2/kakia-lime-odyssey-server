using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_ITEM_REPAIR_PRICE
/// Size: 11 bytes (0x0B)
/// Server sends the repair price for an item
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ITEM_REPAIR_PRICE : IPacketFixed
{
	public bool isEquiped;     // Offset: 0x02 - True if item is equipped, false if in inventory
	public int slot;           // Offset: 0x03 - Slot index (equipment or inventory)
	public uint price;         // Offset: 0x07 - Repair price in currency
}
