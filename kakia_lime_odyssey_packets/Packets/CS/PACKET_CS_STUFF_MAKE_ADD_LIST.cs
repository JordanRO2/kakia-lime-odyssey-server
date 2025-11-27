/// <summary>
/// Client packet requesting to add an item to the stuff/material making queue.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_ADD_LIST
/// Size: 18 bytes
/// Ordinal: 2635
/// Sent when adding materials to the gathering/processing queue.
/// Server responds with SC_STUFF_MAKE_ADD_LIST_SUCCESS.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_STUFF_MAKE_ADD_LIST
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Item to add to the processing queue</summary>
	public STUFF_MAKE_SLOT addItem;
}
