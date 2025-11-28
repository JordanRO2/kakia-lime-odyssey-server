using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to start continuous crafting of an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_ITEM_MAKE_CONTINUALLY
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int count (4 bytes)
/// Response: SC_ITEM_MAKE_START_CASTING
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_ITEM_MAKE_CONTINUALLY : IPacketFixed
{
	/// <summary>Number of items to craft continuously (offset 0x02)</summary>
	public int count;
}
