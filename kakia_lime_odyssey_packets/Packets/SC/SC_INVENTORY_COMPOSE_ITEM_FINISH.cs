using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_INVENTORY_COMPOSE_ITEM_FINISH @ 10 bytes
/// Sent when an item composition/combination process finishes.
/// This is a fixed-length packet (PACKET_FIX header = 2 bytes).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_INVENTORY_COMPOSE_ITEM_FINISH
{
	public long instID;
}
