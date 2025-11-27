using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to move an item between inventory slots.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_MOVE_SLOT_ITEM
/// Size: Variable (depends on ITEM_SLOT size)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: ITEM_SLOT from
/// - ITEM_SLOT to
/// - unsigned __int64 count (8 bytes)
/// Response: SC_MOVE_SLOT_ITEM
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_MOVE_SLOT_ITEM : IPacketFixed
{
	/// <summary>Source item slot (offset 0x02)</summary>
	public ITEM_SLOT from;

	/// <summary>Destination item slot</summary>
	public ITEM_SLOT to;

	/// <summary>Number of items to move</summary>
	public ulong count;
}
