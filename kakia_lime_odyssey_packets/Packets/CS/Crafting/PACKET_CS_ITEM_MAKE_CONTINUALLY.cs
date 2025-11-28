/// <summary>
/// Client packet to craft multiple items continuously.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_CONTINUALLY
/// Size: 6 bytes
/// Ordinal: 2630
/// Queues multiple crafts to be done one after another.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_ITEM_MAKE_CONTINUALLY
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Number of items to craft continuously</summary>
	public int count;
}
