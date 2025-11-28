using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_COMBAT_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM
/// Size: 11 bytes (0x0B)
/// Server notifies client about durability changes for combat job equipped items.
/// Sent when combat equipment (weapons, armor) loses durability during combat.
/// </summary>
/// <remarks>
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes [handled by IPacketFixed]
/// - 0x02: unsigned __int8 equipSlot - 1 byte (equipment slot: weapon, armor, etc.)
/// - 0x03: int durability - 4 bytes (current durability)
/// - 0x07: int mdurability - 4 bytes (maximum durability)
/// Total: 11 bytes
///
/// This is the combat job specific variant of durability updates.
/// Combat jobs use different equipment slots than life jobs (tools).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_COMBAT_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM : IPacketFixed
{
	public byte equipSlot;      // Offset: 0x02 - Equipment slot being updated
	public int durability;      // Offset: 0x03 - Current durability
	public int mdurability;     // Offset: 0x07 - Maximum durability
}
