/// <summary>
/// Service for handling combat calculations including damage, hit rolls, and critical strikes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// Uses structures: STATUS_PC, HIT_DESC, AttackInfo
/// Hit result types from AttackInfo::HIT_FAIL_TYPE:
/// - HIT_FAIL_NONE (0): Invalid
/// - HIT_FAIL_HIT (1): Normal hit
/// - HIT_FAIL_CRITICAL_HIT (2): Critical hit
/// - HIT_FAIL_MISS (3): Miss
/// - HIT_FAIL_AVOID (4): Dodge
/// - HIT_FAIL_SHIELD (5): Block
/// - HIT_FAIL_GUARD (6): Parry
///
/// Skill damage types are determined by XmlSkill.GetDamageType() based on skill ID ranges:
/// - Fighter/Thief skills: Physical damage
/// - Priest/Mage skills: Magical damage
/// - Life job skills: Non-combat
/// </remarks>
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.SkillXML;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Service for handling combat calculations including damage, hit rolls, and critical strikes.
/// </summary>
public class CombatService : ICombatService
{
	private static readonly ThreadLocal<Random> _random = new(() => new Random());

	/// <summary>
	/// Parry damage reduction percentage.
	/// </summary>
	private const float ParryDamageReduction = 0.5f;

	/// <summary>
	/// Block damage reduction percentage.
	/// </summary>
	private const float BlockDamageReduction = 0.75f;

	/// <inheritdoc/>
	public AttackResult DealWeaponDamage(IEntity source, IEntity target)
	{
		var sourceStatus = source.GetEntityStatus();
		var targetStatus = target.GetEntityStatus();
		var rnd = _random.Value!;

		// Determine hit result using full combat chain
		var hitResult = DetermineHitResult(sourceStatus, targetStatus, rnd);

		// Calculate damage based on hit result
		uint damage = CalculateDamageWithResult(sourceStatus, targetStatus, hitResult, rnd);

		// Check for dual-wielding (off-hand weapon equipped)
		uint subDamage = 0;
		HIT_FAIL_TYPE subHitResult = HIT_FAIL_TYPE.HIT_FAIL_MISS;

		if (sourceStatus.SubAttack != null)
		{
			subHitResult = DetermineSubHandHitResult(sourceStatus.SubAttack, targetStatus, rnd);
			subDamage = CalculateSubWeaponDamageWithResult(sourceStatus.SubAttack, targetStatus, subHitResult, rnd);
		}

		uint totalDamage = damage + subDamage;
		var packet = BuildWeaponHitPacketWithResult(source, target, damage, hitResult, subDamage, subHitResult, sourceStatus);

		// Build bullet packet and calculate delay for ranged attacks
		byte[]? bulletPacket = null;
		uint hitDelay = 0;
		if (sourceStatus.IsRanged)
		{
			hitDelay = CalculateRangeHitDelay(source, target);
			bulletPacket = BuildBulletPacket(source, target, sourceStatus);
		}

		return new AttackResult
		{
			Damage = damage,
			IsMiss = hitResult == HIT_FAIL_TYPE.HIT_FAIL_MISS || hitResult == HIT_FAIL_TYPE.HIT_FAIL_AVOID,
			IsCritical = hitResult == HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT,
			HitResultType = hitResult,
			SubDamage = subDamage,
			SubIsMiss = subHitResult == HIT_FAIL_TYPE.HIT_FAIL_MISS || subHitResult == HIT_FAIL_TYPE.HIT_FAIL_AVOID,
			SubIsCritical = subHitResult == HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT,
			SubHitResultType = subHitResult,
			TargetKilled = targetStatus.BasicStatus.Hp <= totalDamage,
			Packet = packet,
			IsRanged = sourceStatus.IsRanged,
			BulletPacket = bulletPacket,
			HitDelay = hitDelay
		};
	}

	/// <summary>
	/// Determines the hit result using the full combat chain:
	/// Miss -> Dodge -> Block -> Parry -> Critical -> Normal Hit
	/// </summary>
	private static HIT_FAIL_TYPE DetermineHitResult(EntityStatus source, EntityStatus target, Random rnd)
	{
		// Step 1: Check for miss (based on hit rate vs dodge)
		double hitChance = StatCalculator.CalculateHitChance(source.MeleeAttack.Hit, target.Dodge);
		if (rnd.Next(0, 100) >= hitChance)
		{
			// Determine if it was a miss or a dodge
			// If target has high dodge, it's more likely an active dodge
			if (target.Dodge > 0 && rnd.Next(0, 100) < 50)
				return HIT_FAIL_TYPE.HIT_FAIL_AVOID; // Active dodge
			return HIT_FAIL_TYPE.HIT_FAIL_MISS; // Miss
		}

		// Step 2: Check for block (requires shield)
		if (target.Block > 0)
		{
			float blockChance = Math.Min(target.Block, 50.0f); // Cap at 50%
			if (rnd.Next(0, 100) < blockChance)
				return HIT_FAIL_TYPE.HIT_FAIL_SHIELD;
		}

		// Step 3: Check for parry (requires weapon)
		if (target.Parry > 0)
		{
			float parryChance = Math.Min(target.Parry, 40.0f); // Cap at 40%
			if (rnd.Next(0, 100) < parryChance)
				return HIT_FAIL_TYPE.HIT_FAIL_GUARD;
		}

		// Step 4: Check for critical hit
		int critChance = Math.Clamp(source.MeleeAttack.CritRate, 0, 100);
		if (rnd.Next(0, 100) < critChance)
			return HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT;

		// Step 5: Normal hit
		return HIT_FAIL_TYPE.HIT_FAIL_HIT;
	}

	/// <summary>
	/// Determines hit result for off-hand weapon (reduced accuracy).
	/// </summary>
	private static HIT_FAIL_TYPE DetermineSubHandHitResult(AttackStatus subAttack, EntityStatus target, Random rnd)
	{
		// Off-hand has 10% reduced accuracy
		double hitChance = StatCalculator.CalculateHitChance((ushort)(subAttack.Hit * 0.9), target.Dodge);
		if (rnd.Next(0, 100) >= hitChance)
		{
			if (target.Dodge > 0 && rnd.Next(0, 100) < 50)
				return HIT_FAIL_TYPE.HIT_FAIL_AVOID;
			return HIT_FAIL_TYPE.HIT_FAIL_MISS;
		}

		// Block check
		if (target.Block > 0)
		{
			float blockChance = Math.Min(target.Block, 50.0f);
			if (rnd.Next(0, 100) < blockChance)
				return HIT_FAIL_TYPE.HIT_FAIL_SHIELD;
		}

		// Parry check
		if (target.Parry > 0)
		{
			float parryChance = Math.Min(target.Parry, 40.0f);
			if (rnd.Next(0, 100) < parryChance)
				return HIT_FAIL_TYPE.HIT_FAIL_GUARD;
		}

		// Critical check
		int critChance = Math.Clamp(subAttack.CritRate, 0, 100);
		if (rnd.Next(0, 100) < critChance)
			return HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT;

		return HIT_FAIL_TYPE.HIT_FAIL_HIT;
	}

	/// <summary>
	/// Calculates damage based on hit result type.
	/// </summary>
	private static uint CalculateDamageWithResult(EntityStatus source, EntityStatus target, HIT_FAIL_TYPE hitResult, Random rnd)
	{
		return hitResult switch
		{
			HIT_FAIL_TYPE.HIT_FAIL_MISS => 0,
			HIT_FAIL_TYPE.HIT_FAIL_AVOID => 0,
			HIT_FAIL_TYPE.HIT_FAIL_SHIELD => CalculateBlockedDamage(source, target, rnd),
			HIT_FAIL_TYPE.HIT_FAIL_GUARD => CalculateParriedDamage(source, target, rnd),
			HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT => CalculateCriticalDamage(source, target, rnd),
			_ => CalculateNormalDamage(source, target, rnd)
		};
	}

	/// <summary>
	/// Calculates normal hit damage.
	/// </summary>
	private static uint CalculateNormalDamage(EntityStatus source, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		return StatCalculator.CalculateDamage(source.MeleeAttack.Atk, target.MeleeAttack.Def, false, variance);
	}

	/// <summary>
	/// Calculates critical hit damage.
	/// </summary>
	private static uint CalculateCriticalDamage(EntityStatus source, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		return StatCalculator.CalculateDamage(source.MeleeAttack.Atk, target.MeleeAttack.Def, true, variance);
	}

	/// <summary>
	/// Calculates blocked damage (reduced by shield).
	/// </summary>
	private static uint CalculateBlockedDamage(EntityStatus source, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		uint baseDamage = StatCalculator.CalculateDamage(source.MeleeAttack.Atk, target.MeleeAttack.Def, false, variance);
		return (uint)(baseDamage * (1.0 - BlockDamageReduction));
	}

	/// <summary>
	/// Calculates parried damage (reduced by weapon).
	/// </summary>
	private static uint CalculateParriedDamage(EntityStatus source, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		uint baseDamage = StatCalculator.CalculateDamage(source.MeleeAttack.Atk, target.MeleeAttack.Def, false, variance);
		return (uint)(baseDamage * (1.0 - ParryDamageReduction));
	}

	/// <summary>
	/// Calculates sub-weapon damage based on hit result.
	/// </summary>
	private static uint CalculateSubWeaponDamageWithResult(AttackStatus subAttack, EntityStatus target, HIT_FAIL_TYPE hitResult, Random rnd)
	{
		const double offHandPenalty = 0.5;

		uint baseDamage = hitResult switch
		{
			HIT_FAIL_TYPE.HIT_FAIL_MISS => 0,
			HIT_FAIL_TYPE.HIT_FAIL_AVOID => 0,
			HIT_FAIL_TYPE.HIT_FAIL_SHIELD => CalculateSubBlockedDamage(subAttack, target, rnd),
			HIT_FAIL_TYPE.HIT_FAIL_GUARD => CalculateSubParriedDamage(subAttack, target, rnd),
			HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT => CalculateSubCriticalDamage(subAttack, target, rnd),
			_ => CalculateSubNormalDamage(subAttack, target, rnd)
		};

		return (uint)(baseDamage * offHandPenalty);
	}

	private static uint CalculateSubNormalDamage(AttackStatus subAttack, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		return StatCalculator.CalculateDamage(subAttack.Atk, target.MeleeAttack.Def, false, variance);
	}

	private static uint CalculateSubCriticalDamage(AttackStatus subAttack, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		return StatCalculator.CalculateDamage(subAttack.Atk, target.MeleeAttack.Def, true, variance);
	}

	private static uint CalculateSubBlockedDamage(AttackStatus subAttack, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		uint baseDamage = StatCalculator.CalculateDamage(subAttack.Atk, target.MeleeAttack.Def, false, variance);
		return (uint)(baseDamage * (1.0 - BlockDamageReduction));
	}

	private static uint CalculateSubParriedDamage(AttackStatus subAttack, EntityStatus target, Random rnd)
	{
		double variance = rnd.NextDouble();
		uint baseDamage = StatCalculator.CalculateDamage(subAttack.Atk, target.MeleeAttack.Def, false, variance);
		return (uint)(baseDamage * (1.0 - ParryDamageReduction));
	}

	/// <summary>
	/// Calculates off-hand weapon damage (reduced effectiveness).
	/// </summary>
	private static uint CalculateSubWeaponDamage(
		AttackStatus subAttack,
		EntityStatus targetStatus,
		bool isMiss,
		bool isCrit,
		Random rnd)
	{
		if (isMiss)
			return 0;

		// Off-hand damage is 50% of normal damage
		const double offHandPenalty = 0.5;

		if (isCrit)
		{
			double critVariance = 1.5 + (rnd.NextDouble() * 0.5);
			return (uint)(subAttack.Atk * critVariance * 2.0 * offHandPenalty);
		}

		double normalVariance = 0.8 + (rnd.NextDouble() * 0.4);
		double defReduction = targetStatus.MeleeAttack.Def / (targetStatus.MeleeAttack.Def + 100.0);
		double baseDamage = subAttack.Atk * (1.0 - defReduction) * normalVariance * offHandPenalty;
		return (uint)Math.Max(1, baseDamage);
	}

	/// <inheritdoc/>
	public AttackResult DealSkillDamage(IEntity source, IEntity target, int skillId, int skillLevel)
	{
		var sourceStatus = source.GetEntityStatus();
		var targetStatus = target.GetEntityStatus();
		var rnd = _random.Value!;

		// Get skill data from database
		var skill = Network.LimeServer.SkillDB.FirstOrDefault(s => s.Id == skillId);

		// Determine damage type based on skill
		var damageType = GetSkillDamageType(skill);

		// Get appropriate attack/defense stats based on damage type
		var (attackStat, defenseStat, hitRate, fleeRate, critRate) = GetCombatStats(sourceStatus, targetStatus, damageType);

		// Calculate hit chance with bounds (5% min, 95% max)
		double hitChance = 95.0;
		if (fleeRate > 0)
		{
			hitChance = (hitRate / (double)fleeRate) * 100;
			hitChance = Math.Clamp(hitChance, 5.0, 95.0);
		}

		bool isMiss = rnd.Next(0, 100) >= hitChance;

		// Critical hit check (capped at 100%)
		int clampedCritRate = Math.Clamp(critRate, 0, 100);
		bool isCrit = !isMiss && rnd.Next(0, 100) < clampedCritRate;

		// Calculate skill multiplier based on skill level
		double skillMultiplier = GetSkillMultiplier(skill, skillLevel);

		// Calculate damage
		uint damage = CalculateSkillDamage(attackStat, defenseStat, isMiss, isCrit, skillMultiplier, rnd);

		// Build packet (using weapon hit for now - could create skill-specific packet)
		var packet = BuildWeaponHitPacket(source, target, damage, isMiss, isCrit, sourceStatus);

		return new AttackResult
		{
			Damage = damage,
			IsMiss = isMiss,
			IsCritical = isCrit,
			TargetKilled = !isMiss && targetStatus.BasicStatus.Hp <= damage,
			Packet = packet
		};
	}

	/// <summary>
	/// Deals magical damage from source to target.
	/// </summary>
	public AttackResult DealMagicalDamage(IEntity source, IEntity target)
	{
		var sourceStatus = source.GetEntityStatus();
		var targetStatus = target.GetEntityStatus();
		var rnd = _random.Value!;

		// Use spell attack stats
		ushort spellAtk = sourceStatus.SpellAttack.Atk;
		ushort spellDef = targetStatus.SpellAttack.Def;
		ushort hitRate = sourceStatus.SpellAttack.Hit;
		ushort fleeRate = targetStatus.SpellAttack.FleeRate;
		int critRate = sourceStatus.SpellAttack.CritRate;

		// Calculate hit chance with bounds (5% min, 95% max)
		double hitChance = 95.0;
		if (fleeRate > 0)
		{
			hitChance = (hitRate / (double)fleeRate) * 100;
			hitChance = Math.Clamp(hitChance, 5.0, 95.0);
		}

		bool isMiss = rnd.Next(0, 100) >= hitChance;

		// Critical hit check (capped at 100%)
		int clampedCrit = Math.Clamp(critRate, 0, 100);
		bool isCrit = !isMiss && rnd.Next(0, 100) < clampedCrit;

		uint damage = CalculateMagicalDamage(spellAtk, spellDef, isMiss, isCrit, rnd);

		var packet = BuildWeaponHitPacket(source, target, damage, isMiss, isCrit, sourceStatus);

		return new AttackResult
		{
			Damage = damage,
			IsMiss = isMiss,
			IsCritical = isCrit,
			TargetKilled = !isMiss && targetStatus.BasicStatus.Hp <= damage,
			Packet = packet
		};
	}

	/// <summary>
	/// Gets the damage type for a skill from its XML definition.
	/// </summary>
	/// <remarks>
	/// Uses XmlSkill.GetDamageType() which determines type by skill ID range:
	/// - Fighter/Thief (ID 1-14, 501-599, 1001-1999): Physical
	/// - Priest/Mage (ID 2001-2999, 3001-3999): Magical
	/// - Life jobs (ID 5000+): None
	/// </remarks>
	private static SkillDamageType GetSkillDamageType(XmlSkill? skill)
	{
		if (skill == null)
			return SkillDamageType.Physical;

		return skill.GetDamageType();
	}

	/// <summary>
	/// Gets combat stats based on damage type.
	/// </summary>
	/// <remarks>
	/// Maps skill damage types to appropriate attack/defense stats:
	/// - Physical: Uses melee attack/defense stats
	/// - Magical: Uses spell attack/defense stats
	/// - Hybrid: Uses average of both
	/// - None: Uses melee stats (non-combat skills shouldn't call this)
	/// </remarks>
	private static (ushort atk, ushort def, ushort hit, ushort flee, int crit) GetCombatStats(
		EntityStatus source, EntityStatus target, SkillDamageType damageType)
	{
		return damageType switch
		{
			SkillDamageType.Magical => (
				source.SpellAttack.Atk,
				target.SpellAttack.Def,
				source.SpellAttack.Hit,
				target.SpellAttack.FleeRate,
				source.SpellAttack.CritRate
			),
			SkillDamageType.Hybrid => (
				// Hybrid uses average of melee and spell
				(ushort)((source.MeleeAttack.Atk + source.SpellAttack.Atk) / 2),
				(ushort)((target.MeleeAttack.Def + target.SpellAttack.Def) / 2),
				(ushort)((source.MeleeAttack.Hit + source.SpellAttack.Hit) / 2),
				(ushort)((target.MeleeAttack.FleeRate + target.SpellAttack.FleeRate) / 2),
				(source.MeleeAttack.CritRate + source.SpellAttack.CritRate) / 2
			),
			// Physical and None both use melee stats
			_ => (
				source.MeleeAttack.Atk,
				target.MeleeAttack.Def,
				source.MeleeAttack.Hit,
				target.MeleeAttack.FleeRate,
				source.MeleeAttack.CritRate
			)
		};
	}

	/// <summary>
	/// Gets skill damage multiplier based on skill level.
	/// </summary>
	private static double GetSkillMultiplier(XmlSkill? skill, int skillLevel)
	{
		if (skill == null)
			return 1.0;

		// Base multiplier increases with skill level
		// Level 1: 1.2x, Level 2: 1.4x, ... Level 7: 2.4x
		double baseMultiplier = 1.0 + (0.2 * Math.Max(1, skillLevel));

		// Combo skills get bonus damage
		if (skill.IsCombo == 1)
			baseMultiplier *= 1.1;

		// Skills with longer cast time deal more damage
		if (skill.CastingTime > 0)
			baseMultiplier *= 1.0 + (skill.CastingTime / 5000.0); // +20% per second of cast time

		return baseMultiplier;
	}

	/// <summary>
	/// Calculates skill damage with multiplier.
	/// </summary>
	private static uint CalculateSkillDamage(
		ushort attackStat,
		ushort defenseStat,
		bool isMiss,
		bool isCrit,
		double skillMultiplier,
		Random rnd)
	{
		if (isMiss)
			return 0;

		double baseDamage;

		if (isCrit)
		{
			// Critical hits: 1.5x-2.0x variance, reduced defense, 2x multiplier
			double critVariance = 1.5 + (rnd.NextDouble() * 0.5);
			// Crits only apply 50% of target defense
			double defReduction = (defenseStat * 0.5) / ((defenseStat * 0.5) + 100.0);
			baseDamage = attackStat * (1.0 - defReduction) * critVariance * 2.0 * skillMultiplier;
		}
		else
		{
			// Normal hits: 0.8x-1.2x variance with defense diminishing returns
			double normalVariance = 0.8 + (rnd.NextDouble() * 0.4);
			double defReduction = defenseStat / (defenseStat + 100.0);
			baseDamage = attackStat * (1.0 - defReduction) * normalVariance * skillMultiplier;
		}

		return (uint)Math.Max(1, baseDamage);
	}

	/// <summary>
	/// Calculates magical damage.
	/// </summary>
	private static uint CalculateMagicalDamage(
		ushort spellAtk,
		ushort spellDef,
		bool isMiss,
		bool isCrit,
		Random rnd)
	{
		if (isMiss)
			return 0;

		if (isCrit)
		{
			// Magical crits: 1.4x-1.8x variance (slightly lower than physical)
			double critVariance = 1.4 + (rnd.NextDouble() * 0.4);
			// Magic ignores more defense on crit
			double defReduction = (spellDef * 0.3) / ((spellDef * 0.3) + 100.0);
			return (uint)(spellAtk * (1.0 - defReduction) * critVariance * 2.0);
		}

		// Normal magical hits: 0.9x-1.1x variance (more consistent than physical)
		double normalVariance = 0.9 + (rnd.NextDouble() * 0.2);
		double normalDefReduction = spellDef / (spellDef + 120.0); // Magic defense slightly less effective
		double baseDamage = spellAtk * (1.0 - normalDefReduction) * normalVariance;
		return (uint)Math.Max(1, baseDamage);
	}

	/// <inheritdoc/>
	public bool RollHit(int hitRate, int dodgeRate)
	{
		if (dodgeRate <= 0)
			return true;

		double hitChance = (hitRate / (double)dodgeRate) * 100;
		hitChance = Math.Clamp(hitChance, 5.0, 95.0);

		return _random.Value!.Next(0, 100) < hitChance;
	}

	/// <inheritdoc/>
	public bool RollCritical(int critRate)
	{
		int clampedCrit = Math.Clamp(critRate, 0, 100);
		return _random.Value!.Next(0, 100) < clampedCrit;
	}

	private static uint CalculateDamage(
		EntityStatus sourceStatus,
		EntityStatus targetStatus,
		bool isMiss,
		bool isCrit,
		Random rnd)
	{
		if (isMiss)
			return 0;

		if (isCrit)
		{
			// Critical hits: 1.5x-2.0x variance, ignores defense, 2x multiplier
			double critVariance = 1.5 + (rnd.NextDouble() * 0.5);
			return (uint)(sourceStatus.MeleeAttack.Atk * critVariance * 2.0);
		}

		// Normal hits: 0.8x-1.2x variance with defense diminishing returns
		double normalVariance = 0.8 + (rnd.NextDouble() * 0.4);
		double defReduction = targetStatus.MeleeAttack.Def / (targetStatus.MeleeAttack.Def + 100.0);
		double baseDamage = sourceStatus.MeleeAttack.Atk * (1.0 - defReduction) * normalVariance;
		return (uint)Math.Max(1, baseDamage);
	}

	/// <summary>
	/// Builds weapon hit packet (single weapon, no dual-wield).
	/// </summary>
	private static byte[] BuildWeaponHitPacket(
		IEntity source,
		IEntity target,
		uint damage,
		bool isMiss,
		bool isCrit,
		EntityStatus sourceStatus)
	{
		return BuildWeaponHitPacket(source, target, damage, isMiss, isCrit, 0, true, false, sourceStatus);
	}

	private static byte[] BuildWeaponHitPacket(
		IEntity source,
		IEntity target,
		uint damage,
		bool isMiss,
		bool isCrit,
		uint subDamage,
		bool subIsMiss,
		bool subIsCrit,
		EntityStatus sourceStatus)
	{
		using PacketWriter pw = new();

		// Determine if either hit was a crit (glared shows crit animation)
		bool anyCrit = isCrit || subIsCrit;

		SC_WEAPON_HIT_RESULT hit = new()
		{
			fromInstID = source.GetId(),
			targetInstID = target.GetId(),
			glared = anyCrit,
			aniSpeedRatio = 1,
			main = new()
			{
				result = (byte)(isMiss ? HIT_FAIL_TYPE.HIT_FAIL_MISS : HIT_FAIL_TYPE.HIT_FAIL_HIT),
				weaponTypeID = (int)sourceStatus.MeleeAttack.WeaponTypeId,
				damage = damage,
				bonusDamage = 0
			},
			sub = new()
			{
				result = sourceStatus.SubAttack != null
					? (byte)(subIsMiss ? HIT_FAIL_TYPE.HIT_FAIL_MISS : HIT_FAIL_TYPE.HIT_FAIL_HIT)
					: (byte)0,
				weaponTypeID = sourceStatus.SubAttack != null ? (int)sourceStatus.SubAttack.WeaponTypeId : 0,
				damage = subDamage,
				bonusDamage = 0
			},
			ranged = sourceStatus.IsRanged,
			rangeHitDelay = sourceStatus.IsRanged ? CalculateRangeHitDelay(source, target) : 0,
			rangedVelocity = sourceStatus.IsRanged ? GetProjectileVelocity(sourceStatus) : 0
		};

		pw.Write(hit);
		return pw.ToPacket();
	}

	/// <summary>
	/// Builds weapon hit packet with detailed hit result types.
	/// </summary>
	/// <remarks>
	/// IDA Verified: Yes (2025-11-27)
	/// Uses HIT_DESC structure for main and sub weapon results.
	/// Maps to: PACKET_SC_WEAPON_HIT_RESULT (64 bytes)
	/// </remarks>
	private static byte[] BuildWeaponHitPacketWithResult(
		IEntity source,
		IEntity target,
		uint damage,
		HIT_FAIL_TYPE hitResult,
		uint subDamage,
		HIT_FAIL_TYPE subHitResult,
		EntityStatus sourceStatus)
	{
		using PacketWriter pw = new();

		// Determine if either hit was a crit (glared shows crit animation)
		bool anyCrit = hitResult == HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT ||
					   subHitResult == HIT_FAIL_TYPE.HIT_FAIL_CRITICAL_HIT;

		SC_WEAPON_HIT_RESULT hit = new()
		{
			fromInstID = source.GetId(),
			targetInstID = target.GetId(),
			glared = anyCrit,
			aniSpeedRatio = sourceStatus.HitSpeedRatio,
			main = new()
			{
				result = (byte)hitResult,
				weaponTypeID = (int)sourceStatus.MeleeAttack.WeaponTypeId,
				damage = damage,
				bonusDamage = 0
			},
			sub = new()
			{
				result = sourceStatus.SubAttack != null ? (byte)subHitResult : (byte)0,
				weaponTypeID = sourceStatus.SubAttack != null ? (int)sourceStatus.SubAttack.WeaponTypeId : 0,
				damage = subDamage,
				bonusDamage = 0
			},
			ranged = sourceStatus.IsRanged,
			rangeHitDelay = sourceStatus.IsRanged ? CalculateRangeHitDelay(source, target) : 0,
			rangedVelocity = sourceStatus.IsRanged ? GetProjectileVelocity(sourceStatus) : 0
		};

		pw.Write(hit);
		return pw.ToPacket();
	}

	/// <summary>
	/// Calculates the delay for ranged attacks based on actual distance between entities.
	/// </summary>
	private static uint CalculateRangeHitDelay(IEntity source, IEntity target)
	{
		var sourcePos = source.GetPosition();
		var targetPos = target.GetPosition();

		// Calculate 3D distance
		float dx = targetPos.x - sourcePos.x;
		float dy = targetPos.y - sourcePos.y;
		float dz = targetPos.z - sourcePos.z;
		float distance = MathF.Sqrt(dx * dx + dy * dy + dz * dz);

		// Minimum distance to prevent zero/very low delays
		distance = MathF.Max(distance, 1.0f);

		const float projectileSpeed = 15.0f; // units per second

		// Delay in milliseconds
		return (uint)(distance / projectileSpeed * 1000);
	}

	/// <summary>
	/// Gets the projectile velocity for ranged attacks based on weapon type.
	/// </summary>
	private static float GetProjectileVelocity(EntityStatus sourceStatus)
	{
		// Different weapon types have different projectile speeds
		var weaponType = (WeaponType)sourceStatus.MeleeAttack.WeaponTypeId;

		return weaponType switch
		{
			WeaponType.Bow => 20.0f,        // Faster arrow velocity
			WeaponType.CrossBow => 25.0f,   // Crossbow bolts are fastest
			WeaponType.Gun => 30.0f,        // Bullets are very fast
			WeaponType.Wand => 12.0f,       // Magic projectiles are slower
			WeaponType.Staff => 10.0f,      // Staff magic even slower
			_ => 15.0f                       // Default projectile speed
		};
	}

	/// <summary>
	/// Generates a unique bullet ID for projectile tracking.
	/// </summary>
	private static long _bulletIdCounter;
	private static long GenerateBulletId()
	{
		return Interlocked.Increment(ref _bulletIdCounter);
	}

	/// <summary>
	/// Builds a bullet launch packet for ranged attacks.
	/// </summary>
	private static byte[] BuildBulletPacket(IEntity source, IEntity target, EntityStatus sourceStatus)
	{
		using PacketWriter pw = new();

		var velocity = GetProjectileVelocity(sourceStatus);

		SC_LAUNCH_BULLET_ITEM_OBJ bullet = new()
		{
			fromInstID = source.GetId(),
			toInstID = target.GetId(),
			typeID = (int)sourceStatus.MeleeAttack.WeaponTypeId,
			useSP = 0,
			useHP = 0,
			useMP = 0,
			useLP = 0,
			coolTime = 0,
			bulletID = GenerateBulletId(),
			tick = (uint)Environment.TickCount,
			velocity = velocity
		};

		pw.Write(bullet);
		return pw.ToPacket();
	}
}
