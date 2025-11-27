using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when removing an item from the trade exchange list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes) - inventory slot index
/// - 0x06: __int64 count (8 bytes) - number of items to remove
/// Use case: Removing items from trade window during player-to-player trading
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST : IPacketFixed
{
	public int slot;
	public long count;
}
