using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server request to loot an item from a lootable container.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_LOOT_ITEM
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int popSlot (4 bytes)
/// - 0x06: __int64 popCount (8 bytes)
/// - 0x0E: int pushSlot (4 bytes)
/// Response: SC_LOOTED_ITEM
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_LOOT_ITEM : IPacketFixed
{
	/// <summary>Source slot in loot window (offset 0x02)</summary>
	public int popSlot;

	/// <summary>Amount to loot (offset 0x06)</summary>
	public long popCount;

	/// <summary>Destination slot in inventory (offset 0x0E)</summary>
	public int pushSlot;
}
