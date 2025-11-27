namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Entity status containing all combat-relevant stats.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Maps to: STATUS_PC (176 bytes) in IDA
/// Contains: COMMON_STATUS, COMBAT_JOB_STATUS_, LIFE_JOB_STATUS_, VELOCITIES
/// </remarks>
public class EntityStatus
{
	/// <summary>Combat job level</summary>
	public byte Lv;

	/// <summary>Current experience points</summary>
	public ulong Exp;

	/// <summary>Basic status (HP/MP)</summary>
	public BasicStatus BasicStatus = new();

	/// <summary>Base stats (STR, INT, DEX, AGI, VIT, SPI, LUK)</summary>
	public BaseStats BaseStats = new();

	/// <summary>Derived melee attack stats</summary>
	public AttackStatus MeleeAttack = new();

	/// <summary>Derived spell attack stats</summary>
	public AttackStatus SpellAttack = new();

	/// <summary>Off-hand weapon attack stats (null if no off-hand weapon)</summary>
	public AttackStatus? SubAttack;

	/// <summary>Life job stats (populated only when in life job mode)</summary>
	public LifeJobStats? LifeJobStats;

	/// <summary>Whether the entity is using ranged weapons</summary>
	public bool IsRanged;

	/// <summary>Dodge/Flee rate (from STATUS_PC.dodge)</summary>
	public float Dodge;

	/// <summary>Parry rate for weapon-based damage reduction</summary>
	public float Parry;

	/// <summary>Block rate for shield-based damage reduction</summary>
	public float Block;

	/// <summary>Attack speed multiplier (from STATUS_PC.hitSpeedRatio)</summary>
	public float HitSpeedRatio = 1.0f;

	/// <summary>Status effect resistance</summary>
	public uint Resist;

	/// <summary>LP (Life Points)</summary>
	public uint Lp;

	/// <summary>Maximum LP</summary>
	public uint MaxLp;

	/// <summary>Stream Points (SP) for skills</summary>
	public uint StreamPoint;
}

/// <summary>
/// Attack statistics for a weapon slot.
/// </summary>
/// <remarks>
/// Derived from base stats + equipment using StatCalculator.
/// </remarks>
public class AttackStatus
{
	/// <summary>Weapon type ID for animation/effect selection</summary>
	public uint WeaponTypeId;

	/// <summary>Attack power (melee or spell)</summary>
	public ushort Atk;

	/// <summary>Hit rate for accuracy calculation</summary>
	public ushort Hit;

	/// <summary>Defense value</summary>
	public ushort Def;

	/// <summary>Critical hit rate (0-100)</summary>
	public ushort CritRate;

	/// <summary>Flee/Dodge rate</summary>
	public ushort FleeRate;
}

/// <summary>
/// Base stats that determine derived combat stats.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Maps to: COMBAT_JOB_STATUS_ (24 bytes) in IDA
/// Memory Layout:
/// - 0x00: lv (1 byte)
/// - 0x04: exp (4 bytes)
/// - 0x08: strength (2 bytes)
/// - 0x0A: intelligence (2 bytes)
/// - 0x0C: dexterity (2 bytes)
/// - 0x0E: agility (2 bytes)
/// - 0x10: vitality (2 bytes)
/// - 0x12: spirit (2 bytes)
/// - 0x14: lucky (2 bytes)
/// </remarks>
public class BaseStats
{
	/// <summary>Strength - affects melee attack power</summary>
	public ushort Strength;

	/// <summary>Intelligence - affects spell attack power</summary>
	public ushort Intelligence;

	/// <summary>Dexterity - affects hit rate, critical rate</summary>
	public ushort Dexterity;

	/// <summary>Agility - affects dodge, attack speed</summary>
	public ushort Agility;

	/// <summary>Vitality - affects HP, physical defense</summary>
	public ushort Vitality;

	/// <summary>Spirit - affects MP, magical defense</summary>
	public ushort Spirit;

	/// <summary>Lucky - affects crit, dodge, drop rates</summary>
	public ushort Lucky;
}

/// <summary>
/// Basic HP/MP status.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Maps to: COMMON_STATUS (20 bytes) in IDA
/// Memory Layout:
/// - 0x00: lv (1 byte) - stored in EntityStatus.Lv
/// - 0x04: hp (4 bytes)
/// - 0x08: mhp (4 bytes)
/// - 0x0C: mp (4 bytes)
/// - 0x10: mmp (4 bytes)
/// </remarks>
public class BasicStatus
{
	/// <summary>Current HP</summary>
	public uint Hp;

	/// <summary>Maximum HP</summary>
	public uint MaxHp;

	/// <summary>Current MP</summary>
	public uint Mp;

	/// <summary>Maximum MP</summary>
	public uint MaxMp;
}

/// <summary>
/// Life Job stats used for crafting, gathering, and other life skills.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Maps to: LIFE_JOB_STATUS_ (20 bytes) in IDA
/// Memory Layout:
/// - 0x00: lv (1 byte)
/// - 0x01-0x03: padding (3 bytes)
/// - 0x04: exp (4 bytes)
/// - 0x08: statusPoint (2 bytes)
/// - 0x0A: idea (2 bytes)
/// - 0x0C: mind (2 bytes)
/// - 0x0E: craft (2 bytes)
/// - 0x10: sense (2 bytes)
/// Based on inherit.xml: IDE (creativity), MID (mentality), CRT (activity), SES (sensitivity)
/// </remarks>
public class LifeJobStats
{
	/// <summary>Life job level</summary>
	public byte Lv;

	/// <summary>Current experience points</summary>
	public uint Exp;

	/// <summary>Available status points to distribute</summary>
	public ushort StatusPoint;

	/// <summary>Idea (creativity) - affects crafting innovation and quality variance</summary>
	public ushort Idea;

	/// <summary>Mind (mentality) - affects concentration and failure resistance</summary>
	public ushort Mind;

	/// <summary>Craft (activity) - affects crafting speed and efficiency</summary>
	public ushort Craft;

	/// <summary>Sense (sensitivity) - affects gathering yield and rare finds</summary>
	public ushort Sense;

	// Derived stats with equipment bonuses
	/// <summary>Collection success rate bonus</summary>
	public int CollectSuccessRate;

	/// <summary>Collection yield increase rate</summary>
	public int CollectionIncreaseRate;

	/// <summary>Crafting time reduction amount</summary>
	public int MakeTimeDecrease;
}
