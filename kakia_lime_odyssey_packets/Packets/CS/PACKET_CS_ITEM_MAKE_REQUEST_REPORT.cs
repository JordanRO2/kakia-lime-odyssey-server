/// <summary>
/// Client packet to request crafting report update.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_REQUEST_REPORT
/// Size: 2 bytes
/// Ordinal: 2624
/// Requests server to send updated crafting information (success rates, materials, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_ITEM_MAKE_REQUEST_REPORT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
