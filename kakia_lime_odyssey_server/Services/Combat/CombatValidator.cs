using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Validates combat actions before they are executed.
/// Ensures all combat rules are enforced server-side.
/// </summary>
public static class CombatValidator
{
	/// <summary>
	/// Maximum attack range for melee weapons (in game units).
	/// </summary>
	public const float MaxMeleeRange = 5.0f;

	/// <summary>
	/// Maximum attack range for ranged weapons (in game units).
	/// </summary>
	public const float MaxRangedRange = 30.0f;

	/// <summary>
	/// Maximum critical rate percentage.
	/// </summary>
	public const int MaxCritRate = 100;

	/// <summary>
	/// Maximum hit rate percentage.
	/// </summary>
	public const int MaxHitRate = 95;

	/// <summary>
	/// Minimum hit rate percentage.
	/// </summary>
	public const int MinHitRate = 5;

	/// <summary>
	/// Result of combat validation.
	/// </summary>
	public class ValidationResult
	{
		public bool IsValid { get; set; }
		public string FailReason { get; set; } = string.Empty;
		public CombatValidationError ErrorType { get; set; } = CombatValidationError.None;

		public static ValidationResult Success() => new() { IsValid = true };

		public static ValidationResult Failure(CombatValidationError error, string reason) => new()
		{
			IsValid = false,
			ErrorType = error,
			FailReason = reason
		};
	}

	/// <summary>
	/// Types of combat validation errors.
	/// </summary>
	public enum CombatValidationError
	{
		None,
		AttackerDead,
		AttackerStunned,
		AttackerSilenced,
		TargetDead,
		TargetInvalid,
		TargetInvincible,
		OutOfRange,
		NoWeaponEquipped,
		WeaponBroken,
		InvalidStats,
		CooldownNotReady,
		PvPDisabled,
		SameFaction
	}

	/// <summary>
	/// Validates whether an attack can be performed.
	/// </summary>
	/// <param name="attacker">The attacking entity</param>
	/// <param name="target">The target entity</param>
	/// <returns>Validation result indicating success or failure reason</returns>
	public static ValidationResult ValidateAttack(IEntity attacker, IEntity target)
	{
		// 1. Validate attacker state
		var attackerValidation = ValidateAttackerState(attacker);
		if (!attackerValidation.IsValid)
			return attackerValidation;

		// 2. Validate target state
		var targetValidation = ValidateTargetState(target);
		if (!targetValidation.IsValid)
			return targetValidation;

		// 3. Validate range
		var rangeValidation = ValidateRange(attacker, target);
		if (!rangeValidation.IsValid)
			return rangeValidation;

		// 4. Validate weapon
		var weaponValidation = ValidateWeapon(attacker);
		if (!weaponValidation.IsValid)
			return weaponValidation;

		// 5. Validate PvP rules (if applicable)
		var pvpValidation = ValidatePvPRules(attacker, target);
		if (!pvpValidation.IsValid)
			return pvpValidation;

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates whether a skill can be used.
	/// </summary>
	public static ValidationResult ValidateSkillUse(IEntity caster, IEntity? target, int skillId, bool requiresTarget)
	{
		// 1. Validate caster state
		var casterValidation = ValidateCasterState(caster);
		if (!casterValidation.IsValid)
			return casterValidation;

		// 2. Validate target if required
		if (requiresTarget && target != null)
		{
			var targetValidation = ValidateTargetState(target);
			if (!targetValidation.IsValid)
				return targetValidation;
		}

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates the attacker's state for physical attacks.
	/// </summary>
	private static ValidationResult ValidateAttackerState(IEntity attacker)
	{
		var status = attacker.GetEntityStatus();

		// Check if attacker is alive
		if (status.BasicStatus.Hp <= 0)
		{
			return ValidationResult.Failure(
				CombatValidationError.AttackerDead,
				"Attacker is dead");
		}

		// Check for stun status
		var buffModifiers = LimeServer.BuffService.GetBuffModifiers(attacker);
		if (buffModifiers.IsStunned)
		{
			return ValidationResult.Failure(
				CombatValidationError.AttackerStunned,
				"Attacker is stunned");
		}

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates the caster's state for skill use.
	/// </summary>
	private static ValidationResult ValidateCasterState(IEntity caster)
	{
		var status = caster.GetEntityStatus();

		// Check if caster is alive
		if (status.BasicStatus.Hp <= 0)
		{
			return ValidationResult.Failure(
				CombatValidationError.AttackerDead,
				"Caster is dead");
		}

		// Check for stun status
		var buffModifiers = LimeServer.BuffService.GetBuffModifiers(caster);
		if (buffModifiers.IsStunned)
		{
			return ValidationResult.Failure(
				CombatValidationError.AttackerStunned,
				"Caster is stunned");
		}

		// Check for silence status (for magic skills)
		if (buffModifiers.IsSilenced)
		{
			return ValidationResult.Failure(
				CombatValidationError.AttackerSilenced,
				"Caster is silenced");
		}

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates the target's state.
	/// </summary>
	private static ValidationResult ValidateTargetState(IEntity target)
	{
		var status = target.GetEntityStatus();

		// Check if target is alive
		if (status.BasicStatus.Hp <= 0)
		{
			return ValidationResult.Failure(
				CombatValidationError.TargetDead,
				"Target is already dead");
		}

		// Check for invincibility
		var buffModifiers = LimeServer.BuffService.GetBuffModifiers(target);
		if (buffModifiers.IsInvincible)
		{
			return ValidationResult.Failure(
				CombatValidationError.TargetInvincible,
				"Target is invincible");
		}

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates the distance between attacker and target.
	/// </summary>
	private static ValidationResult ValidateRange(IEntity attacker, IEntity target)
	{
		var attackerPos = attacker.GetPosition();
		var targetPos = target.GetPosition();

		float dx = targetPos.x - attackerPos.x;
		float dy = targetPos.y - attackerPos.y;
		float dz = targetPos.z - attackerPos.z;
		float distance = MathF.Sqrt(dx * dx + dy * dy + dz * dz);

		// Determine max range based on weapon type
		var status = attacker.GetEntityStatus();
		float maxRange = status.IsRanged ? MaxRangedRange : MaxMeleeRange;

		// Add some tolerance for network latency
		maxRange *= 1.2f;

		if (distance > maxRange)
		{
			return ValidationResult.Failure(
				CombatValidationError.OutOfRange,
				$"Target is out of range (distance: {distance:F1}, max: {maxRange:F1})");
		}

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates that the attacker has a valid weapon equipped.
	/// </summary>
	private static ValidationResult ValidateWeapon(IEntity attacker)
	{
		var status = attacker.GetEntityStatus();

		// Check if attack stat is valid (indicates weapon is equipped)
		if (status.MeleeAttack.Atk <= 0)
		{
			// Allow unarmed combat with minimal damage
			// This is not necessarily an error, just reduced effectiveness
		}

		// Future: Check weapon durability
		// if (weaponDurability <= 0)
		// {
		//     return ValidationResult.Failure(
		//         CombatValidationError.WeaponBroken,
		//         "Weapon is broken");
		// }

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates PvP rules between attacker and target.
	/// </summary>
	private static ValidationResult ValidatePvPRules(IEntity attacker, IEntity target)
	{
		// Check if both are players
		bool attackerIsPlayer = attacker is PlayerClient;
		bool targetIsPlayer = target is PlayerClient;

		if (attackerIsPlayer && targetIsPlayer)
		{
			// Future: Check PvP flags
			// if (!attacker.IsPvPEnabled() || !target.IsPvPEnabled())
			// {
			//     return ValidationResult.Failure(
			//         CombatValidationError.PvPDisabled,
			//         "PvP is not enabled");
			// }

			// Future: Check faction rules
			// if (attacker.GetFaction() == target.GetFaction())
			// {
			//     return ValidationResult.Failure(
			//         CombatValidationError.SameFaction,
			//         "Cannot attack same faction");
			// }
		}

		return ValidationResult.Success();
	}

	/// <summary>
	/// Validates and clamps combat stats to valid ranges.
	/// </summary>
	/// <param name="status">The entity status to validate</param>
	/// <returns>A validated copy of the stats</returns>
	public static EntityStatus ValidateStats(EntityStatus status)
	{
		// Ensure attack is non-negative
		if (status.MeleeAttack.Atk < 0)
			status.MeleeAttack.Atk = 0;

		// Ensure defense is non-negative
		if (status.MeleeAttack.Def < 0)
			status.MeleeAttack.Def = 0;

		// Clamp crit rate
		status.MeleeAttack.CritRate = Math.Clamp(status.MeleeAttack.CritRate, 0, MaxCritRate);

		// Clamp hit rate
		status.MeleeAttack.Hit = Math.Max(status.MeleeAttack.Hit, 1);

		// Clamp flee rate
		status.MeleeAttack.FleeRate = Math.Max(status.MeleeAttack.FleeRate, 0);

		return status;
	}

	/// <summary>
	/// Calculates and clamps hit chance to valid range.
	/// </summary>
	public static double CalculateHitChance(int attackerHit, int targetFlee)
	{
		if (targetFlee <= 0)
			return MaxHitRate;

		double hitChance = (attackerHit / (double)targetFlee) * 100;
		return Math.Clamp(hitChance, MinHitRate, MaxHitRate);
	}

	/// <summary>
	/// Logs a combat validation failure.
	/// </summary>
	public static void LogValidationFailure(IEntity attacker, IEntity target, ValidationResult result)
	{
		string attackerName = GetEntityName(attacker);
		string targetName = GetEntityName(target);

		Logger.Log(
			$"[COMBAT VALIDATION] {attackerName} -> {targetName}: {result.ErrorType} - {result.FailReason}",
			LogLevel.Debug);
	}

	private static string GetEntityName(IEntity entity)
	{
		if (entity is PlayerClient pc)
			return pc.GetCurrentCharacter()?.appearance.name ?? $"Player({entity.GetId()})";
		return $"Entity({entity.GetId()})";
	}
}
