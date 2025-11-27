using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_MOVE_SLOT_ITEM - Confirms item slot move to client
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_MOVE_SLOT_ITEM
/// Size: 4 bytes (PACKET_VAR header only - variable length packet)
/// Offsets:
///   0x00: unsigned __int16 header (2 bytes)
///   0x02: unsigned __int16 size (2 bytes)
/// Note: This is a variable-length packet. The actual data follows the header.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_MOVE_SLOT_ITEM
{
	public ushort header;
	public ushort size;
}
