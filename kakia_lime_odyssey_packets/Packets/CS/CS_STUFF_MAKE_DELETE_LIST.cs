/// <summary>
/// Client->Server packet to remove item from material processing queue.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_DELETE_LIST
/// Size: 16 bytes (18 with PACKET_FIX header)
/// Response: SC_STUFF_MAKE_DELETE_LIST_SUCCESS
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Common;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_STUFF_MAKE_DELETE_LIST
{
	/// <summary>Item to remove from the processing queue</summary>
	public STUFF_MAKE_SLOT deleteItem;
}
