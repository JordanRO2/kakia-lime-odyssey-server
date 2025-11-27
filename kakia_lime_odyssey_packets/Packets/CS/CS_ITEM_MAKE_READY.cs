/// <summary>
/// Client->Server packet to prepare for crafting an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_READY
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Response: SC_ITEM_MAKE_UPDATE_REPORT
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_ITEM_MAKE_READY
{
	/// <summary>Type ID of the item recipe to prepare for crafting</summary>
	public uint typeID;
}
