using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_MOVE_SLOT_ITEM - Move item from one slot to another
/// IDA Verified: 2025-11-26
/// Structure: PACKET_CS_MOVE_SLOT_ITEM
/// Size: 20 bytes
/// Offsets:
///   0x00: PACKET_FIX header (2 bytes)
///   0x02: ITEM_SLOT from (5 bytes)
///   0x07: ITEM_SLOT to (5 bytes)
///   0x0C: __int64 count (8 bytes)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_MOVE_SLOT_ITEM
{
	public ushort header;
	public ITEM_SLOT from;
	public ITEM_SLOT to;
	public long count;
}
