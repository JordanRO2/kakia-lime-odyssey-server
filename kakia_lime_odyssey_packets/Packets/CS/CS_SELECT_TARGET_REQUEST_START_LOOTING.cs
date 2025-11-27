/// <summary>
/// Client->Server request to select a target and start looting it.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_SELECT_TARGET_REQUEST_START_LOOTING
/// Size: 10 bytes (8 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - objInstID: Instance ID of the object to loot (8 bytes at offset 0x02)
/// Response: SC_LOOTABLE_ITEM_LIST
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_SELECT_TARGET_REQUEST_START_LOOTING : IPacketFixed
{
	/// <summary>Instance ID of the object to loot</summary>
	public long objInstID;
}
