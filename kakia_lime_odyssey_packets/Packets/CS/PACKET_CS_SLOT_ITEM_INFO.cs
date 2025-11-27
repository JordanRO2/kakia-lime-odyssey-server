/// <summary>
/// Client packet requesting detailed information about an item in a specific slot.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SLOT_ITEM_INFO
/// Size: 7 bytes
/// Ordinal: 2678
/// Sent when the player hovers over or inspects an item.
/// Server responds with SC_SLOT_ITEM_INFO containing full item details.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SLOT_ITEM_INFO
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Slot information (type and index)</summary>
	public ITEM_SLOT slot;
}
