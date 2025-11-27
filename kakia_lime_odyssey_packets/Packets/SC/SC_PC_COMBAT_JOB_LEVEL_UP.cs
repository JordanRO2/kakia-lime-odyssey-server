/// <summary>
/// Server->Client packet sent when a character levels up their combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PC_COMBAT_JOB_LEVEL_UP
/// Size: 25 bytes (27 with PACKET_FIX header)
/// Triggered by: Combat job experience reaching threshold
/// Note: Stats are new base values including the increase from level up
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PC_COMBAT_JOB_LEVEL_UP
{
	/// <summary>Instance ID of the character leveling up</summary>
	public long objInstID;

	/// <summary>New combat job level</summary>
	public byte lv;

	/// <summary>Current experience after level up</summary>
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