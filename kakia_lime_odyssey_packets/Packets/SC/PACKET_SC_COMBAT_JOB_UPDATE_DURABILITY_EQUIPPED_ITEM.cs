/// <summary>
/// Server packet to update equipment durability for combat job items.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_COMBAT_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM
/// Size: 11 bytes
/// Ordinal: 20950
/// Updates durability of equipped combat job items (weapons, armor, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_COMBAT_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Equipment slot index</summary>
	public byte equipSlot;

	/// <summary>Current durability value</summary>
	public int durability;

	/// <summary>Maximum durability value</summary>
	public int mdurability;
}
