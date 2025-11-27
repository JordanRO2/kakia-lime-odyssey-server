using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Confirms that an item was successfully looted from a corpse.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LOOTED_ITEM
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes) - Inventory slot where item was placed
/// - 0x06: __int64 count (8 bytes) - Amount looted
/// Triggered by: CS_LOOT_ITEM
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LOOTED_ITEM : IPacketFixed
{
	/// <summary>Inventory slot where item was placed (offset 0x02)</summary>
	public int slot;

	/// <summary>Amount looted (offset 0x06)</summary>
	public long count;
}
