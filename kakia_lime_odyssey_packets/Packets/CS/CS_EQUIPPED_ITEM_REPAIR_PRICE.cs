using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_CS_EQUIPED_ITEM_REPAIR_PRICE
/// Size: 2 bytes (0x02)
/// Client requests repair price for all equipped items (no parameters)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EQUIPPED_ITEM_REPAIR_PRICE : IPacketFixed
{
	// No additional fields - just the packet header
}
