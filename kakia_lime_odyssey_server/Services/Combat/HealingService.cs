using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.SkillXML;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Service for handling healing calculations including critical heals and regeneration.
/// </summary>
public class HealingService : IHealingService
{
	private static readonly ThreadLocal<Random> _random = new(() => new Random());

	// Base critical heal bonus (50% more healing on crit)
	private const double CriticalHealBonus = 1.5;

	// Spirit stat contribution to healing (per point of Spirit)
	private const double SpiritHealingBonus = 0.5;

	/// <inheritdoc/>
	public HealResult HealTarget(IEntity source, IEntity target, int skillId, int skillLevel)
	{
		var sourceStatus = source.GetEntityStatus();
		var targetStatus = target.GetEntityStatus();

		return CalculateHeal(source, target, sourceStatus, targetStatus, skillId, skillLevel);
	}

	/// <inheritdoc/>
	public HealResult HealSelf(IEntity source, int skillId, int skillLevel)
	{
		var sourceStatus = source.GetEntityStatus();

		return CalculateHeal(source, source, sourceStatus, sourceStatus, skillId, skillLevel);
	}

	/// <inheritdoc/>
	public bool RollCriticalHeal(int critRate)
	{
		int clampedCrit = Math.Clamp(critRate, 0, 100);
		return _random.Value!.Next(0, 100) < clampedCrit;
	}

	/// <inheritdoc/>
	public uint CalculateNaturalRegenHP(IEntity entity)
	{
		var status = entity.GetEntityStatus();

		// Base regen is 1% of max HP
		double baseRegen = status.BasicStatus.MaxHp * 0.01;

		// Vitality increases HP regen
		double vitBonus = status.BaseStats.Vitality * 0.1;

		// Total regen (minimum 1)
		return (uint)Math.Max(1, baseRegen + vitBonus);
	}

	/// <inheritdoc/>
	public uint CalculateNaturalRegenMP(IEntity entity)
	{
		var status = entity.GetEntityStatus();

		// Base regen is 2% of max MP
		double baseRegen = status.BasicStatus.MaxMp * 0.02;

		// Spirit increases MP regen
		double spiBonus = status.BaseStats.Spirit * 0.15;

		// Total regen (minimum 1)
		return (uint)Math.Max(1, baseRegen + spiBonus);
	}

	private HealResult CalculateHeal(
		IEntity source,
		IEntity target,
		EntityStatus sourceStatus,
		EntityStatus targetStatus,
		int skillId,
		int skillLevel)
	{
		var rnd = _random.Value!;

		// Get skill data
		var skill = LimeServer.SkillDB.FirstOrDefault(s => s.Id == skillId);
		var skillLevelData = skill?.Subject.SubjectLists.ElementAtOrDefault(skillLevel - 1);

		// Calculate base heal amount from skill
		uint baseHeal = GetSkillHealAmount(skill, skillLevelData, skillLevel);

		// Apply healer's Spirit bonus
		double spiritBonus = sourceStatus.BaseStats.Spirit * SpiritHealingBonus;
		baseHeal = (uint)(baseHeal + spiritBonus);

		// Roll for critical heal (uses spell crit rate)
		int critRate = sourceStatus.SpellAttack.CritRate;
		bool isCritical = RollCriticalHeal(critRate);

		// Apply critical heal bonus
		uint finalHeal = baseHeal;
		if (isCritical)
		{
			// Critical heals: 1.3x-1.7x variance
			double critVariance = 1.3 + (rnd.NextDouble() * 0.4);
			finalHeal = (uint)(baseHeal * critVariance * CriticalHealBonus);
		}
		else
		{
			// Normal heals: 0.9x-1.1x variance
			double normalVariance = 0.9 + (rnd.NextDouble() * 0.2);
			finalHeal = (uint)(baseHeal * normalVariance);
		}

		// Ensure minimum heal of 1
		finalHeal = Math.Max(1, finalHeal);

		// Calculate overheal
		uint currentHP = targetStatus.BasicStatus.Hp;
		uint maxHP = targetStatus.BasicStatus.MaxHp;
		bool wasOverheal = currentHP >= maxHP;

		// Calculate new HP (capped at max)
		uint newHP = Math.Min(currentHP + finalHeal, maxHP);
		uint actualHeal = newHP - currentHP;

		// Build packets
		var packet = BuildHealPacket(source, target, skillId, skill, skillLevelData);
		var hpChangePacket = BuildHPChangePacket(source, target, (int)actualHeal, isCritical);

		return new HealResult
		{
			HealAmount = actualHeal,
			IsCritical = isCritical,
			NewHP = newHP,
			MaxHP = maxHP,
			WasOverheal = wasOverheal,
			Packet = packet,
			HPChangePacket = hpChangePacket
		};
	}

	private static uint GetSkillHealAmount(XmlSkill? skill, XmlSubjectList? skillLevelData, int skillLevel)
	{
		if (skill == null)
			return 50; // Default heal amount

		// Base heal from skill level data
		uint baseHeal = 50;

		// Scale with skill level (Level 1: 50, Level 2: 75, ... Level 7: 200)
		baseHeal = (uint)(50 + (25 * Math.Max(0, skillLevel - 1)));

		// If skill has specific heal values defined, use those
		if (skillLevelData != null)
		{
			// Skills may define heal amount in effect values
			// For now, use a formula based on skill level
			baseHeal = (uint)(skillLevelData.PowerBase + skillLevelData.PowerGrow * skillLevel);
			if (baseHeal == 0)
				baseHeal = (uint)(50 + (25 * Math.Max(0, skillLevel - 1)));
		}

		return baseHeal;
	}

	private static byte[] BuildHealPacket(
		IEntity source,
		IEntity target,
		int skillId,
		XmlSkill? skill,
		XmlSubjectList? skillLevelData)
	{
		using PacketWriter pw = new();

		SC_USE_SKILL_OBJ_RESULT_LIST skillResult = new()
		{
			fromInstID = source.GetId(),
			toInstID = target.GetId(),
			typeID = (uint)skillId,
			useHP = (short)(skillLevelData?.UseHP ?? 0),
			useMP = (short)(skillLevelData?.UseMP ?? 0),
			useSP = (short)(skillLevelData?.UseSP ?? 0),
			useLP = (short)(skillLevelData?.UseLP ?? 0),
			coolTime = (uint)(skillLevelData?.CoolTime ?? skill?.CoolTime ?? 0)
		};

		pw.Write(skillResult);
		return pw.ToSizedPacket();
	}

	private static byte[] BuildHPChangePacket(IEntity source, IEntity target, int healAmount, bool isCritical)
	{
		using PacketWriter pw = new();

		var targetStatus = target.GetEntityStatus();

		SC_CHANGED_HP changedHP = new()
		{
			objInstID = target.GetId(),
			current = (int)targetStatus.BasicStatus.Hp,
			update = healAmount,
			fromID = source.GetId()
		};

		pw.Write(changedHP);
		return pw.ToPacket();
	}
}
