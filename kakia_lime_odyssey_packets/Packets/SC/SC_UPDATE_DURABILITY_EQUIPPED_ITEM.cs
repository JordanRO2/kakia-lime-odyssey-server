using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client about durability change for an equipped item.
/// Sent when equipped item durability decreases during combat/use or increases via repair.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_UPDATE_DURABILITY_EQUIPPED_ITEM
/// Size: 11 bytes (IDA) = 2 bytes header + 9 bytes data
/// C# Size: 9 bytes (header handled by IPacketFixed)
/// Packet Type: PACKET_FIX (fixed length header, 2 bytes)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes [handled by IPacketFixed]
/// - 0x02: unsigned __int8 equipSlot - 1 byte (equipment slot: 0=head, 1=chest, etc.)
/// - 0x03: int durability - 4 bytes (current durability)
/// - 0x07: int mdurability - 4 bytes (maximum durability)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPDATE_DURABILITY_EQUIPPED_ITEM : IPacketFixed
{
	public byte equipSlot;
	public int durability;
	public int mdurability;
}
