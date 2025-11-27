using kakia_lime_odyssey_server.Interfaces;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Interface for buff/debuff/status effect management.
/// </summary>
public interface IBuffService
{
	/// <summary>
	/// Applies a buff to an entity.
	/// </summary>
	/// <param name="target">Entity receiving the buff</param>
	/// <param name="buffTypeId">Buff type ID from BuffInfo.xml</param>
	/// <param name="level">Buff level</param>
	/// <param name="durationMs">Duration in milliseconds (0 for permanent)</param>
	/// <param name="caster">Entity that applied the buff (optional)</param>
	/// <returns>Result of the buff application</returns>
	BuffResult ApplyBuff(IEntity target, int buffTypeId, int level, int durationMs, IEntity? caster = null);

	/// <summary>
	/// Removes a specific buff instance from an entity.
	/// </summary>
	/// <param name="target">Entity losing the buff</param>
	/// <param name="buffInstId">Instance ID of the buff to remove</param>
	/// <returns>True if buff was removed</returns>
	bool RemoveBuff(IEntity target, uint buffInstId);

	/// <summary>
	/// Removes all buffs of a specific type from an entity.
	/// </summary>
	/// <param name="target">Entity losing the buffs</param>
	/// <param name="buffTypeId">Type ID of buffs to remove</param>
	/// <returns>Number of buffs removed</returns>
	int RemoveBuffsByType(IEntity target, int buffTypeId);

	/// <summary>
	/// Removes all buffs from an entity (used on death, etc.).
	/// </summary>
	/// <param name="target">Entity to clear buffs from</param>
	/// <param name="includeDebuffs">Whether to also clear debuffs</param>
	/// <returns>Number of buffs removed</returns>
	int ClearAllBuffs(IEntity target, bool includeDebuffs = true);

	/// <summary>
	/// Gets all active buffs on an entity.
	/// </summary>
	/// <param name="target">Entity to check</param>
	/// <returns>List of active buffs</returns>
	IReadOnlyList<ActiveBuff> GetActiveBuffs(IEntity target);

	/// <summary>
	/// Checks if an entity has a specific buff type active.
	/// </summary>
	/// <param name="target">Entity to check</param>
	/// <param name="buffTypeId">Buff type to look for</param>
	/// <returns>True if buff is active</returns>
	bool HasBuff(IEntity target, int buffTypeId);

	/// <summary>
	/// Updates all buff timers and removes expired buffs.
	/// Should be called periodically (e.g., every server tick).
	/// </summary>
	/// <param name="deltaMs">Milliseconds since last update</param>
	/// <returns>List of expired buffs with their entities for packet sending</returns>
	List<(IEntity Entity, ActiveBuff Buff)> UpdateBuffTimers(int deltaMs);

	/// <summary>
	/// Gets the stat modifiers from all active buffs on an entity.
	/// </summary>
	/// <param name="target">Entity to calculate modifiers for</param>
	/// <returns>Aggregated stat modifiers</returns>
	BuffStatModifiers GetBuffModifiers(IEntity target);
}

/// <summary>
/// Represents an active buff on an entity.
/// </summary>
public class ActiveBuff
{
	/// <summary>Unique instance ID for this buff</summary>
	public uint InstId { get; set; }

	/// <summary>Buff type ID from BuffInfo.xml</summary>
	public int TypeId { get; set; }

	/// <summary>Buff level</summary>
	public int Level { get; set; }

	/// <summary>Remaining duration in milliseconds (0 = permanent)</summary>
	public int RemainingDurationMs { get; set; }

	/// <summary>Total duration when applied</summary>
	public int TotalDurationMs { get; set; }

	/// <summary>When the buff was applied</summary>
	public DateTime AppliedAt { get; set; }

	/// <summary>Entity ID that applied this buff</summary>
	public long CasterId { get; set; }

	/// <summary>Whether this is a debuff (negative effect)</summary>
	public bool IsDebuff { get; set; }

	/// <summary>Whether the buff stuns the target</summary>
	public bool IsStun { get; set; }

	/// <summary>Buff name for logging</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Stat modifiers provided by this buff</summary>
	public BuffStatModifiers Modifiers { get; set; } = new();
}

/// <summary>
/// Stat modifiers from buffs.
/// </summary>
public class BuffStatModifiers
{
	// Flat bonuses
	public int StrengthFlat { get; set; }
	public int IntelligenceFlat { get; set; }
	public int DexterityFlat { get; set; }
	public int AgilityFlat { get; set; }
	public int VitalityFlat { get; set; }
	public int SpiritFlat { get; set; }
	public int LuckyFlat { get; set; }

	public int MeleeAtkFlat { get; set; }
	public int MeleeDefFlat { get; set; }
	public int SpellAtkFlat { get; set; }
	public int SpellDefFlat { get; set; }
	public int HitRateFlat { get; set; }
	public int FleeRateFlat { get; set; }
	public int CritRateFlat { get; set; }

	public int MaxHpFlat { get; set; }
	public int MaxMpFlat { get; set; }

	// Percentage bonuses (additive, e.g., 10 = +10%)
	public int MeleeAtkPercent { get; set; }
	public int MeleeDefPercent { get; set; }
	public int SpellAtkPercent { get; set; }
	public int SpellDefPercent { get; set; }
	public int MovementSpeedPercent { get; set; }
	public int AttackSpeedPercent { get; set; }

	// Special effects
	public bool IsStunned { get; set; }
	public bool IsSilenced { get; set; }
	public bool IsRooted { get; set; }
	public bool IsInvincible { get; set; }

	/// <summary>
	/// Combines modifiers from another buff.
	/// </summary>
	public void Add(BuffStatModifiers other)
	{
		StrengthFlat += other.StrengthFlat;
		IntelligenceFlat += other.IntelligenceFlat;
		DexterityFlat += other.DexterityFlat;
		AgilityFlat += other.AgilityFlat;
		VitalityFlat += other.VitalityFlat;
		SpiritFlat += other.SpiritFlat;
		LuckyFlat += other.LuckyFlat;

		MeleeAtkFlat += other.MeleeAtkFlat;
		MeleeDefFlat += other.MeleeDefFlat;
		SpellAtkFlat += other.SpellAtkFlat;
		SpellDefFlat += other.SpellDefFlat;
		HitRateFlat += other.HitRateFlat;
		FleeRateFlat += other.FleeRateFlat;
		CritRateFlat += other.CritRateFlat;

		MaxHpFlat += other.MaxHpFlat;
		MaxMpFlat += other.MaxMpFlat;

		MeleeAtkPercent += other.MeleeAtkPercent;
		MeleeDefPercent += other.MeleeDefPercent;
		SpellAtkPercent += other.SpellAtkPercent;
		SpellDefPercent += other.SpellDefPercent;
		MovementSpeedPercent += other.MovementSpeedPercent;
		AttackSpeedPercent += other.AttackSpeedPercent;

		IsStunned |= other.IsStunned;
		IsSilenced |= other.IsSilenced;
		IsRooted |= other.IsRooted;
		IsInvincible |= other.IsInvincible;
	}
}

/// <summary>
/// Result of applying a buff.
/// </summary>
public class BuffResult
{
	/// <summary>Whether the buff was successfully applied</summary>
	public bool Success { get; set; }

	/// <summary>Instance ID of the applied buff (0 if failed)</summary>
	public uint BuffInstId { get; set; }

	/// <summary>Reason for failure (if applicable)</summary>
	public BuffFailReason FailReason { get; set; }

	/// <summary>Packet to send to clients</summary>
	public byte[]? Packet { get; set; }

	/// <summary>Whether this buff replaced an existing buff of same type</summary>
	public bool ReplacedExisting { get; set; }
}

/// <summary>
/// Reasons a buff application can fail.
/// </summary>
public enum BuffFailReason
{
	None,
	BuffNotFound,
	TargetImmune,
	MaxBuffsReached,
	AlreadyActive,
	TargetDead,
	InvalidLevel
}
