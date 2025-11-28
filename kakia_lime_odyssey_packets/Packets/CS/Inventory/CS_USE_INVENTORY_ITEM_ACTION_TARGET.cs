using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to use inventory item on selected action target.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_ACTION_TARGET
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_INVENTORY_ITEM_ACTION_TARGET : IPacketFixed
{
	/// <summary>Inventory slot of item to use (offset 0x02)</summary>
	public int slot;
}
