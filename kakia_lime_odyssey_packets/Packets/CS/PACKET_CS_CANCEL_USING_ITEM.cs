/// <summary>
/// Client packet requesting to cancel item usage.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CANCEL_USING_ITEM
/// Size: 2 bytes
/// Ordinal: 2660
/// Sent when the player cancels using an item (e.g., during cast time).
/// Server responds with SC_CANCELED_USING_ITEM.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_CANCEL_USING_ITEM
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
