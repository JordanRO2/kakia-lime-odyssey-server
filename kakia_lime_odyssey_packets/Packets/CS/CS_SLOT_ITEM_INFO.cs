/// <summary>
/// Client->Server packet requesting item info for a specific slot.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SLOT_ITEM_INFO
/// Size: 5 bytes (7 with PACKET_FIX header)
/// Response: SC_SLOT_ITEM_INFO
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Common;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SLOT_ITEM_INFO
{
	/// <summary>Slot information (type and index)</summary>
	public ITEM_SLOT slot;
}
