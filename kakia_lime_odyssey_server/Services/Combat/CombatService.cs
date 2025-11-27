using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.SkillXML;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Skill damage type determines which stats are used for calculation.
/// </summary>
public enum SkillDamageType
{
	Physical,
	Magical,
	Hybrid
}

/// <summary>
/// Service for handling combat calculations including damage, hit rolls, and critical strikes.
/// </summary>
public class CombatService : ICombatService
{
	private static readonly ThreadLocal<Random> _random = new(() => new Random());

	// Skill type to damage type mapping based on skill class/type
	private static readonly Dictionary<string, SkillDamageType> SkillTypeMapping = new(StringComparer.OrdinalIgnoreCase)
	{
		// Fighter skills - Physical
		{ "StrikingSword", SkillDamageType.Physical },
		{ "TitanCrush", SkillDamageType.Physical },
		{ "HeavyBlow", SkillDamageType.Physical },
		{ "LimitBreaker", SkillDamageType.Physical },
		{ "MissionCarry", SkillDamageType.Physical },

		// Mage skills - Magical
		{ "Fireball", SkillDamageType.Magical },
		{ "IceBolt", SkillDamageType.Magical },
		{ "Lightning", SkillDamageType.Magical },
		{ "MagicMissile", SkillDamageType.Magical },

		// Hybrid skills
		{ "ElementalStrike", SkillDamageType.Hybrid }
	};

	/// <inheritdoc/>
	public DamageResult DealWeaponDamage(IEntity source, IEntity target)
	{
		var sourceStatus = source.GetEntityStatus();
		var targetStatus = target.GetEntityStatus();
		var rnd = _random.Value!;

		// Calculate main hand hit chance with bounds (5% min, 95% max)
		double hitChance = 95.0;
		if (targetStatus.MeleeAttack.FleeRate > 0)
		{
			hitChance = (sourceStatus.MeleeAttack.Hit / (double)targetStatus.MeleeAttack.FleeRate) * 100;
			hitChance = Math.Clamp(hitChance, 5.0, 95.0);
		}

		bool isMiss = rnd.Next(0, 100) >= hitChance;

		// Critical hit check (capped at 100%)
		int critChance = Math.Clamp(sourceStatus.MeleeAttack.CritRate, 0, 100);
		bool isCrit = !isMiss && rnd.Next(0, 100) < critChance;

		uint damage = CalculateDamage(sourceStatus, targetStatus, isMiss, isCrit, rnd);

		// Check for dual-wielding (off-hand weapon equipped)
		uint subDamage = 0;
		bool subIsMiss = true;
		bool subIsCrit = false;

		if (sourceStatus.SubAttack != null)
		{
			// Off-hand hit chance (slightly reduced accuracy)
			double subHitChance = 95.0;
			if (targetStatus.MeleeAttack.FleeRate > 0)
			{
				// Off-hand has 10% reduced accuracy
				subHitChance = (sourceStatus.SubAttack.Hit * 0.9 / targetStatus.MeleeAttack.FleeRate) * 100;
				subHitChance = Math.Clamp(subHitChance, 5.0, 95.0);
			}

			subIsMiss = rnd.Next(0, 100) >= subHitChance;

			// Off-hand critical (same rate as main)
			int subCritChance = Math.Clamp(sourceStatus.SubAttack.CritRate, 0, 100);
			subIsCrit = !subIsMiss && rnd.Next(0, 100) < subCritChance;

			// Off-hand damage (reduced to 50% of calculated damage)
			subDamage = CalculateSubWeaponDamage(sourceStatus.SubAttack, targetStatus, subIsMiss, subIsCrit, rnd);
		}

		uint totalDamage = damage + subDamage;
		var packet = BuildWeaponHitPacket(source, target, damage, isMiss, isCrit, subDamage, subIsMiss, subIsCrit, sourceStatus);

		// Build bullet packet and calculate delay for ranged attacks
		byte[]? bulletPacket = null;
		uint hitDelay = 0;
		if (sourceStatus.IsRanged)
		{
			hitDelay = CalculateRangeHitDelay(source, target);
			bulletPacket = BuildBulletPacket(source, target, sourceStatus);
		}

		return new DamageResult
		{
			Damage = damage,
			IsMiss = isMiss,
			IsCritical = isCrit,
			SubDamage = subDamage,
			SubIsMiss = subIsMiss,
			SubIsCritical = subIsCrit,
			TargetKilled = targetStatus.BasicStatus.Hp <= totalDamage,
			Packet = packet,
			IsRanged = sourceStatus.IsRanged,
			BulletPacket = bulletPacket,
			HitDelay = hitDelay
		};
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
	public DamageResult DealSkillDamage(IEntity source, IEntity target, int skillId, int skillLevel)
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

		return new DamageResult
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
	public DamageResult DealMagicalDamage(IEntity source, IEntity target)
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

		return new DamageResult
		{
			Damage = damage,
			IsMiss = isMiss,
			IsCritical = isCrit,
			TargetKilled = !isMiss && targetStatus.BasicStatus.Hp <= damage,
			Packet = packet
		};
	}

	/// <summary>
	/// Gets the damage type for a skill based on its type attribute.
	/// </summary>
	private static SkillDamageType GetSkillDamageType(XmlSkill? skill)
	{
		if (skill == null)
			return SkillDamageType.Physical;

		// Check skill type mapping
		if (SkillTypeMapping.TryGetValue(skill.Type, out var damageType))
			return damageType;

		// Default to physical for unknown skills
		return SkillDamageType.Physical;
	}

	/// <summary>
	/// Gets combat stats based on damage type.
	/// </summary>
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
