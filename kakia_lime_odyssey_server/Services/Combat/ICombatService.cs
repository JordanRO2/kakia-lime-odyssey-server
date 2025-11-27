using kakia_lime_odyssey_server.Interfaces;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Interface for combat-related services.
/// </summary>
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
public class DamageResult
{
	public uint Damage { get; set; }
	public bool IsMiss { get; set; }
	public bool IsCritical { get; set; }
	public bool TargetKilled { get; set; }
	public byte[]? Packet { get; set; }

	/// <summary>Off-hand weapon damage (for dual-wielding)</summary>
	public uint SubDamage { get; set; }
	/// <summary>Whether the off-hand attack missed</summary>
	public bool SubIsMiss { get; set; }
	/// <summary>Whether the off-hand attack was a critical</summary>
	public bool SubIsCritical { get; set; }
	/// <summary>Experience reward for killing the target</summary>
	public ulong ExpReward { get; set; }

	/// <summary>Whether this was a ranged attack</summary>
	public bool IsRanged { get; set; }
	/// <summary>Projectile launch packet for ranged attacks (null for melee)</summary>
	public byte[]? BulletPacket { get; set; }
	/// <summary>Delay in milliseconds before the hit lands (for ranged attacks)</summary>
	public uint HitDelay { get; set; }
}
