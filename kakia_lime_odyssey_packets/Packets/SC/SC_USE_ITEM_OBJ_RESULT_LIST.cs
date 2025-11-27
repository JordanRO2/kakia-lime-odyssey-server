using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_USE_ITEM_OBJ_RESULT_LIST
/// Size: 36 bytes (0x24)
/// Sent when client uses an item on an object/target
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_USE_ITEM_OBJ_RESULT_LIST : IPacketVar
{
	public long fromInstID;    // Offset: 0x04 - User's instance ID
	public long toInstID;      // Offset: 0x0C - Target instance ID
	public int typeID;         // Offset: 0x14 - Item type ID
	public ushort useSP;       // Offset: 0x18 - SP consumed
	public ushort useHP;       // Offset: 0x1A - HP consumed
	public ushort useMP;       // Offset: 0x1C - MP consumed
	public ushort useLP;       // Offset: 0x1E - LP consumed
	public uint coolTime;      // Offset: 0x20 - Cooldown time in milliseconds
}
