/// <summary>
/// Server packet sent when life job tool durability changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_LIFE_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM
/// Size: 11 bytes
/// Ordinal: 21986
/// Updates durability for life job tools (gathering picks, crafting hammers, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_LIFE_JOB_UPDATE_DURABILITY_EQUIPPED_ITEM
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Equipment slot of the item</summary>
	public byte equipSlot;

	/// <summary>Current durability</summary>
	public int durability;

	/// <summary>Maximum durability</summary>
	public int mdurability;
}
