using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_USE_ITEM_SLOT_RESULT
/// Size: 11 bytes (0x0B)
/// Sent when client uses an item in a slot (self-targeted or inventory use)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_USE_ITEM_SLOT_RESULT : IPacketFixed
{
	public long instID;        // Offset: 0x02 - Item instance ID
	public bool isSuccess;     // Offset: 0x0A - Whether item use succeeded
}
