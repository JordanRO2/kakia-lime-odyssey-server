using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_EQUIPED_ITEM_REPAIR_PRICE
/// Size: 6 bytes (0x06)
/// Server sends the total repair price for all equipped items
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EQUIPPED_ITEM_REPAIR_PRICE : IPacketFixed
{
	public uint price;         // Offset: 0x02 - Total repair price for all equipped items
}
