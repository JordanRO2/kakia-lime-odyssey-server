using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_server.Interfaces;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Interface for combat-related services.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Uses hit result types from AttackInfo::HIT_FAIL_TYPE
/// </remarks>
public interface ICombatService
{
	/// <summary>
	/// Calculates and deals weapon hit damage from source to target.
	/// </summary>
	DamageResult DealWeaponDamage(IEntity source, IEntity target);

	/// <summary>
	/// Calculates and deals skill damage from source to target.
	/// </summary>
	DamageResult DealSkillDamage(IEntity source, IEntity target, int skillId, int skillLevel);

	/// <summary>
	/// Checks if an attack hits based on hit rate and dodge.
	/// </summary>
	bool RollHit(int hitRate, int dodgeRate);

	/// <summary>
	/// Checks if an attack is a critical hit.
	/// </summary>
	bool RollCritical(int critRate);
}

/// <summary>
/// Result of a damage calculation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Maps to: AttackInfo (40 bytes) structure for damage/critical/hitFailType
/// </remarks>
public class DamageResult
{
	/// <summary>Main hand damage dealt</summary>
	public uint Damage { get; set; }

	/// <summary>Whether the main hand attack missed (HIT_FAIL_MISS or HIT_FAIL_AVOID)</summary>
	public bool IsMiss { get; set; }

	/// <summary>Whether the main hand attack was a critical hit</summary>
	public bool IsCritical { get; set; }

	/// <summary>
	/// Detailed hit result type for main hand.
	/// </summary>
	/// <remarks>
	/// Maps to AttackInfo::HIT_FAIL_TYPE in IDA:
	/// - HIT_FAIL_NONE (0): Invalid
	/// - HIT_FAIL_HIT (1): Normal hit
	/// - HIT_FAIL_CRITICAL_HIT (2): Critical hit
	/// - HIT_FAIL_MISS (3): Miss
	/// - HIT_FAIL_AVOID (4): Dodge
	/// - HIT_FAIL_SHIELD (5): Block
	/// - HIT_FAIL_GUARD (6): Parry
	/// </remarks>
	public HIT_FAIL_TYPE HitResultType { get; set; } = HIT_FAIL_TYPE.HIT_FAIL_NONE;

	/// <summary>Whether the target was killed by this attack</summary>
	public bool TargetKilled { get; set; }

	/// <summary>Packet data for SC_WEAPON_HIT_RESULT</summary>
	public byte[]? Packet { get; set; }

	/// <summary>Off-hand weapon damage (for dual-wielding)</summary>
	public uint SubDamage { get; set; }

	/// <summary>Whether the off-hand attack missed</summary>
	public bool SubIsMiss { get; set; }

	/// <summary>Whether the off-hand attack was a critical</summary>
	public bool SubIsCritical { get; set; }

	/// <summary>Detailed hit result type for off-hand</summary>
	public HIT_FAIL_TYPE SubHitResultType { get; set; } = HIT_FAIL_TYPE.HIT_FAIL_NONE;

	/// <summary>Experience reward for killing the target</summary>
	public ulong ExpReward { get; set; }

	/// <summary>Whether this was a ranged attack</summary>
	public bool IsRanged { get; set; }

	/// <summary>Projectile launch packet for ranged attacks (null for melee)</summary>
	public byte[]? BulletPacket { get; set; }

	/// <summary>Delay in milliseconds before the hit lands (for ranged attacks)</summary>
	public uint HitDelay { get; set; }

	/// <summary>Whether the attack was blocked (reduced damage)</summary>
	public bool IsBlocked => HitResultType == HIT_FAIL_TYPE.HIT_FAIL_SHIELD;

	/// <summary>Whether the attack was parried (reduced damage)</summary>
	public bool IsParried => HitResultType == HIT_FAIL_TYPE.HIT_FAIL_GUARD;

	/// <summary>Whether the attack was dodged (no damage)</summary>
	public bool IsDodged => HitResultType == HIT_FAIL_TYPE.HIT_FAIL_AVOID;
}
