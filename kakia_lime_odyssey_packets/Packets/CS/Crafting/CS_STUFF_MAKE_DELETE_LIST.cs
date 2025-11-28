using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Common;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to remove item from material processing queue.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_STUFF_MAKE_DELETE_LIST
/// Size: 18 bytes total (2-byte header + 16-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: STUFF_MAKE_SLOT deleteItem (16 bytes)
/// Response: SC_STUFF_MAKE_DELETE_LIST_SUCCESS
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_STUFF_MAKE_DELETE_LIST : IPacketFixed
{
	/// <summary>Item to remove from the processing queue</summary>
	public STUFF_MAKE_SLOT deleteItem;
}
