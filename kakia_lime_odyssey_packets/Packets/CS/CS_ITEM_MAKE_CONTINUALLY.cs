/// <summary>
/// Client->Server packet to start continuous crafting of an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_CONTINUALLY
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Response: SC_ITEM_MAKE_START_CASTING
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_ITEM_MAKE_CONTINUALLY
{
	/// <summary>Number of items to craft continuously</summary>
	public int count;
}
