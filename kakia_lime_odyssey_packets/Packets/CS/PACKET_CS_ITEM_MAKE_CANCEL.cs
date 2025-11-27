/// <summary>
/// Client packet to cancel crafting in progress.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_CANCEL
/// Size: 2 bytes
/// Ordinal: 2629
/// Stops the current crafting cast.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_ITEM_MAKE_CANCEL
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
