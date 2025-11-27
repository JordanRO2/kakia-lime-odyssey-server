/// <summary>
/// Server packet sent when an entity's MP (Mana Points) changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_MP
/// Size: 18 bytes
/// Ordinal: 17298
/// MP is used for casting spells and skills.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_MP
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object whose MP changed</summary>
	public long objInstID;

	/// <summary>Current MP value</summary>
	public int current;

	/// <summary>MP change amount (delta)</summary>
	public int update;
}
