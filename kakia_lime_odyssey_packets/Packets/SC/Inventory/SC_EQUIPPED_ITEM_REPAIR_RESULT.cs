using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_EQUIPED_ITEM_REPAIR_RESULT
/// Size: 3 bytes (0x03)
/// Server sends the result of repairing all equipped items
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EQUIPPED_ITEM_REPAIR_RESULT : IPacketFixed
{
	public bool isSuccess;     // Offset: 0x02 - True if repair succeeded, false if failed
}
