using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when using an inventory item at a specific position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_USE_INVENTORY_ITEM_POS
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes) - inventory slot index
/// - 0x06: FPOS pos (12 bytes) - target position
/// Use case: Using items that target a ground position (e.g., area effect items)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_INVENTORY_ITEM_POS : IPacketFixed
{
	public int slot;
	public FPOS pos;
}
