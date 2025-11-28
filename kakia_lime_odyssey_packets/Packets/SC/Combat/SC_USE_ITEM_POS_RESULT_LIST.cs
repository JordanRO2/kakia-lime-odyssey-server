using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_USE_ITEM_POS_RESULT_LIST
/// Size: 40 bytes (0x28)
/// Sent when client uses an item at a position (ground target)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_USE_ITEM_POS_RESULT_LIST : IPacketVar
{
	public long fromInstID;    // Offset: 0x04 - User's instance ID
	public FPOS toPos;         // Offset: 0x0C - Target position (12 bytes: x, y, z)
	public int typeID;         // Offset: 0x18 - Item type ID
	public ushort useSP;       // Offset: 0x1C - SP consumed
	public ushort useHP;       // Offset: 0x1E - HP consumed
	public ushort useMP;       // Offset: 0x20 - MP consumed
	public ushort useLP;       // Offset: 0x22 - LP consumed
	public uint coolTime;      // Offset: 0x24 - Cooldown time in milliseconds
}
