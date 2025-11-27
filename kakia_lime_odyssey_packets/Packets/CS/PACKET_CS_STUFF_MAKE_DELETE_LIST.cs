/// <summary>
/// Client packet requesting to remove an item from the stuff/material making queue.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_DELETE_LIST
/// Size: 18 bytes
/// Ordinal: 2638
/// Sent when removing materials from the gathering/processing queue.
/// Server responds with SC_STUFF_MAKE_DELETE_LIST_SUCCESS.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_STUFF_MAKE_DELETE_LIST
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Item to remove from the processing queue</summary>
	public STUFF_MAKE_SLOT deleteItem;
}
