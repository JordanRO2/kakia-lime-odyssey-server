using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet requesting item info for a specific slot.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_SLOT_ITEM_INFO
/// Size: 7 bytes total (2-byte header + 5-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: ITEM_SLOT slot (5 bytes)
/// Response: SC_SLOT_ITEM_INFO
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SLOT_ITEM_INFO : IPacketFixed
{
	/// <summary>Slot information (type and index)</summary>
	public ITEM_SLOT slot;
}
