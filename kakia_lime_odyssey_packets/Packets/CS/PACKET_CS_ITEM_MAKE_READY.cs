/// <summary>
/// Client packet to prepare for crafting an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_READY
/// Size: 6 bytes
/// Ordinal: 2623
/// Opens crafting UI with recipe details for the specified item type.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_ITEM_MAKE_READY
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Item type ID to craft</summary>
	public uint typeID;
}
