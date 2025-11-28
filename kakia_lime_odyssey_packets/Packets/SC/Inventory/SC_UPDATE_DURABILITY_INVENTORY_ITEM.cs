using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client about durability change for an inventory item.
/// Sent when item durability decreases (use, damage) or increases (repair).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_UPDATE_DURABILITY_INVENTORY_ITEM
/// Size: 14 bytes (IDA) = 2 bytes header + 12 bytes data
/// C# Size: 12 bytes (header handled by IPacketFixed)
/// Packet Type: PACKET_FIX (fixed length header, 2 bytes)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes [handled by IPacketFixed]
/// - 0x02: int slot - 4 bytes (inventory slot index)
/// - 0x06: int durability - 4 bytes (current durability)
/// - 0x0A: int mdurability - 4 bytes (maximum durability)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPDATE_DURABILITY_INVENTORY_ITEM : IPacketFixed
{
	public int slot;
	public int durability;
	public int mdurability;
}
