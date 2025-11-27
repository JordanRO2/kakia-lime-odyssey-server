using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_CS_EQUIP_ITEM_INFO
/// Size: 3 bytes (0x03)
/// Client requests information about an equipped item
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EQUIP_ITEM_INFO : IPacketFixed
{
	public byte equipSlot;     // Offset: 0x02 - Equipment slot index to query
}
