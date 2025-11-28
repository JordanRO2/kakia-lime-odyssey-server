using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_DELETE_SLOT_ITEM - Confirms item deletion/discard to client
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_DELETE_SLOT_ITEM
/// Size: 7 bytes
/// Offsets:
///   0x00: PACKET_FIX header (2 bytes)
///   0x02: ITEM_SLOT slot (5 bytes)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DELETE_SLOT_ITEM
{
	public ushort header;
	public ITEM_SLOT slot;
}
