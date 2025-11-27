namespace kakia_lime_odyssey_server.Models;

public class EntityStatus
{
	public byte Lv;
	public ulong Exp;
	public BasicStatus BasicStatus = new();
	public BaseStats BaseStats = new();
	public AttackStatus MeleeAttack = new();
	public AttackStatus SpellAttack = new();
	/// <summary>Off-hand weapon attack stats (null if no off-hand weapon)</summary>
	public AttackStatus? SubAttack;
	/// <summary>Life job stats (populated only when in life job mode)</summary>
	public LifeJobStats? LifeJobStats;
	/// <summary>Whether the entity is using ranged weapons</summary>
	public bool IsRanged;
}

public class AttackStatus
{
	public uint WeaponTypeId;
	public ushort Atk;
	public ushort Hit;
	public ushort Def;
	public ushort CritRate;
	public ushort FleeRate;
}

public class BaseStats
{
	public ushort Strength;
	public ushort Intelligence;
	public ushort Dexterity;
	public ushort Agility;
	public ushort Vitality;
	public ushort Spirit;
	public ushort Lucky;
}

public class BasicStatus
{
	public uint Hp;
	public uint MaxHp;
	public uint Mp;
	public uint MaxMp;
}

/// <summary>
/// Life Job stats used for crafting, gathering, and other life skills.
/// Based on inherit.xml: IDE (creativity), MID (mentality), CRT (activity), SES (sensitivity)
/// </summary>
public class LifeJobStats
{
	public byte Lv;
	public uint Exp;
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