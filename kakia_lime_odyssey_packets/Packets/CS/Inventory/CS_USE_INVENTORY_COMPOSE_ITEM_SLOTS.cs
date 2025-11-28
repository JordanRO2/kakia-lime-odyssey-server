using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Common;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to execute item composition with materials.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS
/// Size: 26 bytes total (2-byte header + 24-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int slot (4 bytes)
/// - 0x06: COMPOSE_ENCHANTS targetSlots (20 bytes)
/// Response: SC_INVENTORY_COMPOSE_ITEM_FINISH
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_INVENTORY_COMPOSE_ITEM_SLOTS : IPacketFixed
{
	/// <summary>Inventory slot containing the base item</summary>
	public int slot;

	/// <summary>Array of material slots for composition</summary>
	public COMPOSE_ENCHANTS targetSlots;
}
