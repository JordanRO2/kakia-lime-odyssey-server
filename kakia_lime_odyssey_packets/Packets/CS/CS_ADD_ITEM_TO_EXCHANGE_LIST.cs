using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when adding an item to the trade exchange list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_ADD_ITEM_TO_EXCHANGE_LIST
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes) - inventory slot index
/// - 0x06: __int64 count (8 bytes) - number of items to add
/// Use case: Adding items to trade window during player-to-player trading
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_ADD_ITEM_TO_EXCHANGE_LIST : IPacketFixed
{
	public int slot;
	public long count;
}
