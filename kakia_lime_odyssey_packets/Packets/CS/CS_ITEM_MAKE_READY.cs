using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to prepare for crafting an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_ITEM_MAKE_READY
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// Response: SC_ITEM_MAKE_UPDATE_REPORT
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_ITEM_MAKE_READY : IPacketFixed
{
	/// <summary>Type ID of the item recipe to prepare for crafting (offset 0x02)</summary>
	public uint typeID;
}
