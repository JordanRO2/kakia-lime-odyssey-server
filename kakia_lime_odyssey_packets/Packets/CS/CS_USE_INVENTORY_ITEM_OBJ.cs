using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when using an inventory item on a target object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_OBJ
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes) - inventory slot index
/// - 0x06: __int64 targetInstID (8 bytes) - target object instance ID
/// Use case: Using items on other players, NPCs, or monsters (e.g., buff items, trade items)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_INVENTORY_ITEM_OBJ : IPacketFixed
{
	public int slot;
	public long targetInstID;
}
