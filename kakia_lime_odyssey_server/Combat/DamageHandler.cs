using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Combat;
using kakia_lime_odyssey_packets.Packets.Enums;

namespace kakia_lime_odyssey_server.Combat;

public static class DamageHandler
{
	private static readonly ThreadLocal<Random> _random = new(() => new Random());
	private static long _bulletIdCounter;

	/// <summary>
	/// Validates and deals weapon damage from source to target.
	/// Returns null result if validation fails.
	/// </summary>
	public static DamageDealtResult? DealWeaponHitDamageValidated(IEntity source, IEntity target)
	{
		// Run full combat validation
		var validation = CombatValidator.ValidateAttack(source, target);
		if (!validation.IsValid)
		{
			CombatValidator.LogValidationFailure(source, target, validation);
			return null;
		}

		return DealWeaponHitDamage(source, target);
	}

	public static DamageDealtResult DealWeaponHitDamage(IEntity source, IEntity target)
	{
		var sourceStatus = source.GetEntityStatus();
		var targetStatus = target.GetEntityStatus();

		// Get buff modifiers for both source and target
		var sourceBuffs = LimeServer.BuffService.GetBuffModifiers(source);
		var targetBuffs = LimeServer.BuffService.GetBuffModifiers(target);

		// Check if source is stunned (cannot attack)
		if (sourceBuffs.IsStunned)
		{
			return new DamageDealtResult
			{
				Damage = 0,
				MainDamage = 0,
				IsMiss = true,
				Packet = null,
				ValidationFailed = true,
				ValidationError = CombatValidator.CombatValidationError.AttackerStunned
			};
		}

		// Check if target is invincible
		if (targetBuffs.IsInvincible)
		{
			return new DamageDealtResult
			{
				Damage = 0,
				MainDamage = 0,
				IsMiss = true,
				Packet = null,
				ValidationFailed = true,
				ValidationError = CombatValidator.CombatValidationError.TargetInvincible
			};
		}

		var rnd = _random.Value!;

		// Apply buff modifiers to stats
		int sourceAtk = sourceStatus.MeleeAttack.Atk + sourceBuffs.MeleeAtkFlat;
		sourceAtk = (int)(sourceAtk * (1 + sourceBuffs.MeleeAtkPercent / 100.0));

		int targetDef = targetStatus.MeleeAttack.Def + targetBuffs.MeleeDefFlat;
		targetDef = (int)(targetDef * (1 + targetBuffs.MeleeDefPercent / 100.0));

		int sourceHit = sourceStatus.MeleeAttack.Hit + sourceBuffs.HitRateFlat;
		int targetFlee = targetStatus.MeleeAttack.FleeRate + targetBuffs.FleeRateFlat;
		int sourceCrit = sourceStatus.MeleeAttack.CritRate + sourceBuffs.CritRateFlat;

		// Calculate main hand hit chance with bounds (5% min, 95% max)
		double hitChance = 95.0;
		if (targetFlee > 0)
		{
			hitChance = (sourceHit / (double)targetFlee) * 100;
			hitChance = Math.Clamp(hitChance, 5.0, 95.0);
		}

		bool isMiss = rnd.Next(0, 100) >= hitChance;

		// Critical hit check (capped at 100%)
		int critChance = Math.Clamp(sourceCrit, 0, 100);
		bool isCrit = !isMiss && rnd.Next(0, 100) < critChance;

		uint damage = CalculateDamage((ushort)Math.Max(0, sourceAtk), (ushort)Math.Max(0, targetDef), isMiss, isCrit, rnd);

		// Check for dual-wielding (off-hand weapon equipped)
		uint subDamage = 0;
		bool subIsMiss = true;
		bool subIsCrit = false;

		if (sourceStatus.SubAttack != null)
		{
			// Off-hand hit chance (10% reduced accuracy)
			double subHitChance = 95.0;
			if (targetStatus.MeleeAttack.FleeRate > 0)
			{
				subHitChance = (sourceStatus.SubAttack.Hit * 0.9 / targetStatus.MeleeAttack.FleeRate) * 100;
				subHitChance = Math.Clamp(subHitChance, 5.0, 95.0);
			}

			subIsMiss = rnd.Next(0, 100) >= subHitChance;

			// Off-hand critical
			int subCritChance = Math.Clamp((int)sourceStatus.SubAttack.CritRate, 0, 100);
			subIsCrit = !subIsMiss && rnd.Next(0, 100) < subCritChance;

			// Off-hand damage (50% penalty)
			subDamage = CalculateSubWeaponDamage(sourceStatus.SubAttack, targetStatus, subIsMiss, subIsCrit, rnd);
		}

		uint totalDamage = damage + subDamage;
		bool anyCrit = isCrit || subIsCrit;

		// Calculate ranged attack delay
		uint rangeHitDelay = 0;
		byte[]? bulletPacket = null;
		if (sourceStatus.IsRanged)
		{
			rangeHitDelay = CalculateRangeHitDelay(source, target);
			bulletPacket = BuildBulletPacket(source, target, sourceStatus);
		}

		using PacketWriter pw = new();
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
			rangeHitDelay = rangeHitDelay,
			rangedVelocity = sourceStatus.IsRanged ? GetProjectileVelocity(sourceStatus) : 0
		};

		pw.Write(hit);

		return new DamageDealtResult()
		{
			Damage = totalDamage,
			MainDamage = damage,
			SubDamage = subDamage,
			IsMiss = isMiss,
			IsCritical = isCrit,
			SubIsMiss = subIsMiss,
			SubIsCritical = subIsCrit,
			IsRanged = sourceStatus.IsRanged,
			HitDelay = rangeHitDelay,
			Packet = pw.ToPacket(),
			BulletPacket = bulletPacket
		};
	}

	private static uint CalculateDamage(ushort atk, ushort def, bool isMiss, bool isCrit, Random rnd)
	{
		if (isMiss)
			return 0;

		if (isCrit)
		{
			// Critical hits: 1.5x-2.0x variance, ignores defense, 2x multiplier
			double critVariance = 1.5 + (rnd.NextDouble() * 0.5);
			return (uint)(atk * critVariance * 2.0);
		}

		// Normal hits: 0.8x-1.2x variance with defense diminishing returns
		double normalVariance = 0.8 + (rnd.NextDouble() * 0.4);
		double defReduction = def / (def + 100.0);
		double baseDamage = atk * (1.0 - defReduction) * normalVariance;
		return (uint)Math.Max(1, baseDamage);
	}

	private static uint CalculateSubWeaponDamage(AttackStatus subAttack, EntityStatus targetStatus, bool isMiss, bool isCrit, Random rnd)
	{
		if (isMiss)
			return 0;

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

	private static uint CalculateRangeHitDelay(IEntity source, IEntity target)
	{
		var sourcePos = source.GetPosition();
		var targetPos = target.GetPosition();

		float dx = targetPos.x - sourcePos.x;
		float dy = targetPos.y - sourcePos.y;
		float dz = targetPos.z - sourcePos.z;
		float distance = MathF.Sqrt(dx * dx + dy * dy + dz * dz);

		distance = MathF.Max(distance, 1.0f);
		const float projectileSpeed = 15.0f;

		return (uint)(distance / projectileSpeed * 1000);
	}

	private static float GetProjectileVelocity(EntityStatus sourceStatus)
	{
		var weaponType = (WeaponType)sourceStatus.MeleeAttack.WeaponTypeId;

		return weaponType switch
		{
			WeaponType.Bow => 20.0f,
			WeaponType.CrossBow => 25.0f,
			WeaponType.Gun => 30.0f,
			WeaponType.Wand => 12.0f,
			WeaponType.Staff => 10.0f,
			_ => 15.0f
		};
	}

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
			bulletID = Interlocked.Increment(ref _bulletIdCounter),
			tick = (uint)Environment.TickCount,
			velocity = velocity
		};

		pw.Write(bullet);
		return pw.ToPacket();
	}
}

public class DamageDealtResult
{
	public uint Damage;
	public uint MainDamage;
	public uint SubDamage;
	public bool IsMiss;
	public bool IsCritical;
	public bool SubIsMiss;
	public bool SubIsCritical;
	public bool IsRanged;
	public uint HitDelay;
	public byte[]? Packet;
	public byte[]? BulletPacket;
	public bool ValidationFailed;
	public CombatValidator.CombatValidationError ValidationError;
}
