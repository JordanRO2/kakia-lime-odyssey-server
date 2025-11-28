/// <summary>
/// SC_ versions of CHANGED status packets without PACKET_FIX header.
/// These are used for server-side packet sending.
/// </summary>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_COMBAT_STATUS (94 bytes total)
/// Contains all combat-related stats.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_COMBAT_STATUS : IPacketFixed
{
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

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_LIFE_STATUS (22 bytes total)
/// Contains gathering and crafting-related status values.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_LIFE_STATUS : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Collection success rate</summary>
	public int collectSucessRate;

	/// <summary>Collection increase rate</summary>
	public int collectionIncreaseRate;

	/// <summary>Crafting time decrease amount</summary>
	public int makeTimeDecreaseAmount;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_VELOCITY_RATIO (18 bytes total)
/// Updates velocity ratio for an entity.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_VELOCITY_RATIO : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current velocity ratio</summary>
	public int current;

	/// <summary>Velocity ratio change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ITEM_USABLE (18 bytes total)
/// Controls whether items can be used.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ITEM_USABLE : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current item usable value</summary>
	public int current;

	/// <summary>Item usable change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_SKILL_USABLE (18 bytes total)
/// Controls whether skills can be used.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_SKILL_USABLE : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill usable value</summary>
	public int current;

	/// <summary>Skill usable change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_MOVABLE (18 bytes total)
/// Controls whether entity can move.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_MOVABLE : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current movable value</summary>
	public int current;

	/// <summary>Movable change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_COLLECT_SUCCESS_RATE (22 bytes total)
/// Affects the success rate for gathering/collecting resources.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_COLLECT_SUCCESS_RATE : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current collect success rate value</summary>
	public float current;

	/// <summary>Collect success rate change amount (delta)</summary>
	public float update;

	/// <summary>Extra value (purpose unknown)</summary>
	public float extra;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_COLLECTION_INCREASE_RATE (22 bytes total)
/// Affects the collection increase rate for gathering.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_COLLECTION_INCREASE_RATE : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current collection increase rate value</summary>
	public float current;

	/// <summary>Collection increase rate change amount (delta)</summary>
	public float update;

	/// <summary>Extra value (purpose unknown)</summary>
	public float extra;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_MAKE_TIME_DECREASE_AMOUNT (18 bytes total)
/// Affects crafting time reduction.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_MAKE_TIME_DECREASE_AMOUNT : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current make time decrease value</summary>
	public int current;

	/// <summary>Make time decrease change amount (delta)</summary>
	public int update;
}
