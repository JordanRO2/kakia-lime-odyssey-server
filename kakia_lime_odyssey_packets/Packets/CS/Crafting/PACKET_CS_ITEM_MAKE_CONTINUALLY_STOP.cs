/// <summary>
/// Client packet to stop continuous crafting.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_CONTINUALLY_STOP
/// Size: 2 bytes
/// Ordinal: 2631
/// Stops the continuous crafting queue.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_ITEM_MAKE_CONTINUALLY_STOP
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
