using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_DISCARD_SLOT_ITEM - Discard/destroy an item from inventory
/// IDA Verified: 2025-11-26
/// Structure: PACKET_CS_DISCARD_SLOT_ITEM
/// Size: 15 bytes
/// Offsets:
///   0x00: PACKET_FIX header (2 bytes)
///   0x02: ITEM_SLOT slot (5 bytes)
///   0x07: __int64 count (8 bytes)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_DISCARD_SLOT_ITEM
{
	public ushort header;
	public ITEM_SLOT slot;
	public long count;
}
