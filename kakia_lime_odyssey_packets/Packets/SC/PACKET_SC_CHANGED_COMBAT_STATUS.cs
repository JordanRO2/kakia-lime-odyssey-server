/// <summary>
/// Server packet sent when an entity's combat status changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_COMBAT_STATUS
/// Size: 94 bytes
/// Ordinal: 2621
/// Contains all combat-related stats including attack, defense, hit rate, dodge, etc.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_COMBAT_STATUS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Melee hit rate</summary>
	public int meleeHitRate;

	/// <summary>Dodge rate</summary>
	public int dodge;

	/// <summary>Critical hit rate</summary>
	public int criticalRate;

	/// <summary>Melee attack power</summary>
	public int meleeAtk;

	/// <summary>Melee defense</summary>
	public int meleeDefense;

	/// <summary>Spell attack power</summary>
	public int spellAtk;

	/// <summary>Spell defense</summary>
	public int spellDefense;

	/// <summary>Parry rate</summary>
	public int parry;

	/// <summary>Block rate</summary>
	public int block;

	/// <summary>Hit speed ratio (attack speed multiplier)</summary>
	public float hitSpeedRatio;

	/// <summary>Updated velocity values</summary>
	public VELOCITIES velocity;
}
