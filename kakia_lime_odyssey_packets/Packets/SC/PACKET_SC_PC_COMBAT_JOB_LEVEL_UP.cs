/// <summary>
/// Server packet sent when a player's combat job levels up.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PC_COMBAT_JOB_LEVEL_UP
/// Size: 27 bytes
/// Ordinal: 2610
/// Contains new level, experience, and all updated combat job stats.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_PC_COMBAT_JOB_LEVEL_UP
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the player who leveled up</summary>
	public long objInstID;

	/// <summary>New combat job level</summary>
	public byte lv;

	/// <summary>Experience after level up</summary>
	public uint exp;

	/// <summary>New Strength stat value</summary>
	public ushort newStr;

	/// <summary>New Agility stat value</summary>
	public ushort newAgi;

	/// <summary>New Vitality stat value</summary>
	public ushort newVit;

	/// <summary>New Intelligence stat value</summary>
	public ushort newInl;

	/// <summary>New Dexterity stat value</summary>
	public ushort newDex;

	/// <summary>New Spirit stat value</summary>
	public ushort newSpi;
}
