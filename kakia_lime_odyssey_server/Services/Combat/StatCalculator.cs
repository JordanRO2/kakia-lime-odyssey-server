using kakia_lime_odyssey_server.Models;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Service for calculating derived combat stats from base stats.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
///
/// Base stats (from COMBAT_JOB_STATUS_):
/// - strength: Affects melee attack power
/// - intelligence: Affects spell attack power
/// - dexterity: Affects hit rate and critical rate
/// - agility: Affects dodge/flee rate and attack speed
/// - vitality: Affects HP and physical defense
/// - spirit: Affects MP and magical defense
/// - lucky: Affects critical rate, dodge, and drop rates
///
/// Derived stats (from STATUS_PC):
/// - meleeAtk: Physical damage dealt
/// - spellAtk: Magical damage dealt
/// - meleeDefense: Physical damage reduction
/// - spellDefense: Magical damage reduction
/// - meleeHitRate: Chance to hit with physical attacks
/// - dodge: Chance to evade physical attacks
/// - parry: Chance to parry (reduce damage) with weapons
/// - block: Chance to block with shields
/// - criticalRate: Chance for critical hits
/// - hitSpeedRatio: Attack speed multiplier
/// - resist: Resistance to status effects
/// </remarks>
public static class StatCalculator
{
	/// <summary>
	/// Stat scaling coefficients based on typical MMORPG formulas.
	/// These can be adjusted for game balance.
	/// </summary>
	private static class Coefficients
	{
		// Attack coefficients
		public const float StrToMeleeAtk = 2.0f;
		public const float DexToMeleeAtk = 0.5f;
		public const float IntToSpellAtk = 2.0f;
		public const float SpiToSpellAtk = 0.5f;

		// Defense coefficients
		public const float VitToMeleeDef = 1.5f;
		public const float AgiToMeleeDef = 0.3f;
		public const float SpiToSpellDef = 1.5f;
		public const float IntToSpellDef = 0.3f;

		// Hit/Dodge coefficients
		public const float DexToHit = 2.0f;
		public const float LuckyToHit = 0.5f;
		public const float AgiToDodge = 1.5f;
		public const float LuckyToDodge = 0.5f;

		// Critical rate coefficients
		public const float DexToCrit = 0.15f;
		public const float LuckyToCrit = 0.25f;

		// Parry/Block coefficients
		public const float DexToParry = 0.1f;
		public const float StrToParry = 0.05f;
		public const float VitToBlock = 0.15f;
		public const float StrToBlock = 0.05f;

		// Attack speed coefficients
		public const float AgiToAttackSpeed = 0.002f;
		public const float DexToAttackSpeed = 0.001f;

		// HP/MP coefficients
		public const float VitToHp = 15.0f;
		public const float LevelToHp = 10.0f;
		public const float BaseHp = 100.0f;
		public const float SpiToMp = 10.0f;
		public const float IntToMp = 5.0f;
		public const float LevelToMp = 5.0f;
		public const float BaseMp = 50.0f;

		// LP (Life Points) coefficients
		public const float VitToLp = 8.0f;
		public const float LevelToLp = 5.0f;
		public const float BaseLp = 50.0f;

		// Resist coefficient
		public const float SpiToResist = 0.5f;
		public const float VitToResist = 0.3f;
	}

	/// <summary>
	/// Calculates all derived stats from base stats and equipment bonuses.
	/// </summary>
	/// <param name="baseStats">The character's base stats (STR, INT, DEX, etc.)</param>
	/// <param name="level">Character combat level</param>
	/// <param name="weaponAtk">Weapon attack power (from equipment)</param>
	/// <param name="weaponSpellAtk">Weapon spell attack power (for magic weapons)</param>
	/// <param name="armorDef">Total defense from armor</param>
	/// <param name="armorSpellDef">Total spell defense from armor</param>
	/// <returns>Calculated derived stats</returns>
	public static DerivedStats CalculateDerivedStats(
		BaseStats baseStats,
		byte level,
		ushort weaponAtk = 0,
		ushort weaponSpellAtk = 0,
		ushort armorDef = 0,
		ushort armorSpellDef = 0)
	{
		var derived = new DerivedStats();

		// Melee Attack = STR * 2 + DEX * 0.5 + Weapon ATK
		derived.MeleeAtk = (uint)(
			baseStats.Strength * Coefficients.StrToMeleeAtk +
			baseStats.Dexterity * Coefficients.DexToMeleeAtk +
			weaponAtk);

		// Spell Attack = INT * 2 + SPI * 0.5 + Weapon Spell ATK
		derived.SpellAtk = (uint)(
			baseStats.Intelligence * Coefficients.IntToSpellAtk +
			baseStats.Spirit * Coefficients.SpiToSpellAtk +
			weaponSpellAtk);

		// Melee Defense = VIT * 1.5 + AGI * 0.3 + Armor DEF
		derived.MeleeDef = (uint)(
			baseStats.Vitality * Coefficients.VitToMeleeDef +
			baseStats.Agility * Coefficients.AgiToMeleeDef +
			armorDef);

		// Spell Defense = SPI * 1.5 + INT * 0.3 + Armor Spell DEF
		derived.SpellDef = (uint)(
			baseStats.Spirit * Coefficients.SpiToSpellDef +
			baseStats.Intelligence * Coefficients.IntToSpellDef +
			armorSpellDef);

		// Hit Rate = DEX * 2 + LUK * 0.5 + Level bonus
		derived.HitRate = (ushort)(
			baseStats.Dexterity * Coefficients.DexToHit +
			baseStats.Lucky * Coefficients.LuckyToHit +
			level);

		// Dodge/Flee = AGI * 1.5 + LUK * 0.5
		derived.Dodge = (float)(
			baseStats.Agility * Coefficients.AgiToDodge +
			baseStats.Lucky * Coefficients.LuckyToDodge);

		// Critical Rate = DEX * 0.15 + LUK * 0.25 (as percentage)
		derived.CriticalRate = (float)(
			baseStats.Dexterity * Coefficients.DexToCrit +
			baseStats.Lucky * Coefficients.LuckyToCrit);
		// Cap at 100%
		derived.CriticalRate = Math.Min(derived.CriticalRate, 100.0f);

		// Parry Rate = DEX * 0.1 + STR * 0.05 (requires weapon)
		derived.Parry = (float)(
			baseStats.Dexterity * Coefficients.DexToParry +
			baseStats.Strength * Coefficients.StrToParry);

		// Block Rate = VIT * 0.15 + STR * 0.05 (requires shield)
		derived.Block = (float)(
			baseStats.Vitality * Coefficients.VitToBlock +
			baseStats.Strength * Coefficients.StrToBlock);

		// Attack Speed Ratio = 1.0 + AGI * 0.002 + DEX * 0.001
		derived.HitSpeedRatio = 1.0f +
			baseStats.Agility * Coefficients.AgiToAttackSpeed +
			baseStats.Dexterity * Coefficients.DexToAttackSpeed;
		// Cap attack speed at 2.0 (double speed)
		derived.HitSpeedRatio = Math.Min(derived.HitSpeedRatio, 2.0f);

		// Max HP = Base + VIT * 15 + Level * 10
		derived.MaxHp = (uint)(
			Coefficients.BaseHp +
			baseStats.Vitality * Coefficients.VitToHp +
			level * Coefficients.LevelToHp);

		// Max MP = Base + SPI * 10 + INT * 5 + Level * 5
		derived.MaxMp = (uint)(
			Coefficients.BaseMp +
			baseStats.Spirit * Coefficients.SpiToMp +
			baseStats.Intelligence * Coefficients.IntToMp +
			level * Coefficients.LevelToMp);

		// Max LP = Base + VIT * 8 + Level * 5
		derived.MaxLp = (uint)(
			Coefficients.BaseLp +
			baseStats.Vitality * Coefficients.VitToLp +
			level * Coefficients.LevelToLp);

		// Resist = SPI * 0.5 + VIT * 0.3
		derived.Resist = (uint)(
			baseStats.Spirit * Coefficients.SpiToResist +
			baseStats.Vitality * Coefficients.VitToResist);

		return derived;
	}

	/// <summary>
	/// Calculates HP at a given level with specified vitality.
	/// </summary>
	public static uint CalculateMaxHp(byte level, ushort vitality)
	{
		return (uint)(
			Coefficients.BaseHp +
			vitality * Coefficients.VitToHp +
			level * Coefficients.LevelToHp);
	}

	/// <summary>
	/// Calculates MP at a given level with specified spirit and intelligence.
	/// </summary>
	public static uint CalculateMaxMp(byte level, ushort spirit, ushort intelligence)
	{
		return (uint)(
			Coefficients.BaseMp +
			spirit * Coefficients.SpiToMp +
			intelligence * Coefficients.IntToMp +
			level * Coefficients.LevelToMp);
	}

	/// <summary>
	/// Calculates LP (Life Points) at a given level with specified vitality.
	/// </summary>
	public static uint CalculateMaxLp(byte level, ushort vitality)
	{
		return (uint)(
			Coefficients.BaseLp +
			vitality * Coefficients.VitToLp +
			level * Coefficients.LevelToLp);
	}

	/// <summary>
	/// Calculates damage dealt from attacker to defender.
	/// </summary>
	/// <param name="attackPower">Attacker's attack power (melee or spell)</param>
	/// <param name="defense">Defender's defense (melee or spell)</param>
	/// <param name="isCritical">Whether this is a critical hit</param>
	/// <param name="variance">Random variance (0.0 to 1.0)</param>
	/// <returns>Final damage value</returns>
	public static uint CalculateDamage(uint attackPower, uint defense, bool isCritical, double variance)
	{
		// Defense reduction uses diminishing returns formula
		// At 0 DEF: 0% reduction
		// At 100 DEF: 50% reduction
		// At 200 DEF: 66% reduction
		// At 400 DEF: 80% reduction
		double defenseReduction = defense / (defense + 100.0);

		// Apply variance (0.8 to 1.2 for normal hits)
		double varianceMultiplier = 0.8 + (variance * 0.4);

		double baseDamage;
		if (isCritical)
		{
			// Critical hits:
			// - Apply higher variance (1.5 to 2.0)
			// - Ignore 50% of defense
			// - 2x damage multiplier
			double critVariance = 1.5 + (variance * 0.5);
			double reducedDefense = defense * 0.5;
			double critDefReduction = reducedDefense / (reducedDefense + 100.0);
			baseDamage = attackPower * (1.0 - critDefReduction) * critVariance * 2.0;
		}
		else
		{
			baseDamage = attackPower * (1.0 - defenseReduction) * varianceMultiplier;
		}

		// Minimum damage of 1
		return (uint)Math.Max(1, baseDamage);
	}

	/// <summary>
	/// Calculates the hit chance based on attacker's hit rate and defender's dodge.
	/// </summary>
	/// <param name="hitRate">Attacker's hit rate</param>
	/// <param name="dodgeRate">Defender's dodge rate</param>
	/// <returns>Hit chance as a percentage (5-95%)</returns>
	public static double CalculateHitChance(ushort hitRate, float dodgeRate)
	{
		if (dodgeRate <= 0)
			return 95.0;

		// Hit chance formula: HIT / (HIT + DODGE) * 100
		// This gives a natural curve where:
		// - Equal HIT/DODGE = 50% hit chance
		// - 2x HIT vs DODGE = 66% hit chance
		// - 3x HIT vs DODGE = 75% hit chance
		double hitChance = (hitRate / (hitRate + dodgeRate)) * 100;

		// Clamp to 5-95% (always some chance to miss/hit)
		return Math.Clamp(hitChance, 5.0, 95.0);
	}

	/// <summary>
	/// Calculates expected experience required for a level.
	/// </summary>
	/// <param name="level">The level to calculate EXP for</param>
	/// <returns>Total EXP needed to reach this level</returns>
	public static ulong CalculateExpForLevel(byte level)
	{
		if (level <= 1)
			return 0;

		// Exponential curve: base * level^exponent
		const double baseExp = 100.0;
		const double exponent = 2.5;
		return (ulong)(baseExp * Math.Pow(level, exponent));
	}

	/// <summary>
	/// Calculates EXP reward for killing a monster.
	/// </summary>
	/// <param name="playerLevel">Killer's level</param>
	/// <param name="monsterLevel">Monster's level</param>
	/// <param name="monsterBaseExp">Monster's base EXP value</param>
	/// <returns>Adjusted EXP reward</returns>
	public static ulong CalculateExpReward(byte playerLevel, byte monsterLevel, ulong monsterBaseExp)
	{
		// Level difference penalty/bonus
		int levelDiff = monsterLevel - playerLevel;

		double multiplier = levelDiff switch
		{
			>= 10 => 1.5,       // Much higher level monster = bonus
			>= 5 => 1.25,       // Higher level monster
			>= -5 => 1.0,       // Similar level = full EXP
			>= -10 => 0.75,     // Lower level monster = reduced
			_ => 0.5            // Much lower level = heavily reduced
		};

		return (ulong)(monsterBaseExp * multiplier);
	}

	/// <summary>
	/// Applies buff modifiers to derived stats.
	/// </summary>
	public static DerivedStats ApplyBuffModifiers(DerivedStats baseStats, BuffModifiers modifiers)
	{
		var result = new DerivedStats
		{
			MeleeAtk = (uint)(baseStats.MeleeAtk * (1.0 + modifiers.AtkPercent / 100.0) + modifiers.AtkFlat),
			SpellAtk = (uint)(baseStats.SpellAtk * (1.0 + modifiers.SpellAtkPercent / 100.0) + modifiers.SpellAtkFlat),
			MeleeDef = (uint)(baseStats.MeleeDef * (1.0 + modifiers.DefPercent / 100.0) + modifiers.DefFlat),
			SpellDef = (uint)(baseStats.SpellDef * (1.0 + modifiers.SpellDefPercent / 100.0) + modifiers.SpellDefFlat),
			HitRate = (ushort)Math.Max(1, baseStats.HitRate + modifiers.HitFlat),
			Dodge = (float)(baseStats.Dodge * (1.0 + modifiers.DodgePercent / 100.0) + modifiers.DodgeFlat),
			CriticalRate = Math.Min(100.0f, baseStats.CriticalRate + modifiers.CritFlat),
			Parry = (float)(baseStats.Parry + modifiers.ParryFlat),
			Block = (float)(baseStats.Block + modifiers.BlockFlat),
			HitSpeedRatio = (float)Math.Min(2.0, baseStats.HitSpeedRatio * (1.0 + modifiers.AttackSpeedPercent / 100.0)),
			MaxHp = (uint)(baseStats.MaxHp * (1.0 + modifiers.MaxHpPercent / 100.0) + modifiers.MaxHpFlat),
			MaxMp = (uint)(baseStats.MaxMp * (1.0 + modifiers.MaxMpPercent / 100.0) + modifiers.MaxMpFlat),
			MaxLp = baseStats.MaxLp,
			Resist = (uint)(baseStats.Resist + modifiers.ResistFlat)
		};

		return result;
	}
}

/// <summary>
/// Derived combat stats calculated from base stats and equipment.
/// </summary>
/// <remarks>
/// Maps to STATUS_PC in IDA (176 bytes).
/// </remarks>
public class DerivedStats
{
	/// <summary>Melee attack power</summary>
	public uint MeleeAtk { get; set; }

	/// <summary>Spell attack power</summary>
	public uint SpellAtk { get; set; }

	/// <summary>Melee defense</summary>
	public uint MeleeDef { get; set; }

	/// <summary>Spell defense</summary>
	public uint SpellDef { get; set; }

	/// <summary>Hit rate for accuracy calculation</summary>
	public ushort HitRate { get; set; }

	/// <summary>Dodge rate for evasion calculation</summary>
	public float Dodge { get; set; }

	/// <summary>Critical hit rate (0-100%)</summary>
	public float CriticalRate { get; set; }

	/// <summary>Parry rate for damage reduction</summary>
	public float Parry { get; set; }

	/// <summary>Block rate for shield blocking</summary>
	public float Block { get; set; }

	/// <summary>Attack speed multiplier</summary>
	public float HitSpeedRatio { get; set; } = 1.0f;

	/// <summary>Maximum HP</summary>
	public uint MaxHp { get; set; }

	/// <summary>Maximum MP</summary>
	public uint MaxMp { get; set; }

	/// <summary>Maximum LP (Life Points)</summary>
	public uint MaxLp { get; set; }

	/// <summary>Status effect resistance</summary>
	public uint Resist { get; set; }
}

/// <summary>
/// Buff modifiers that affect derived stats.
/// </summary>
public class BuffModifiers
{
	// Flat bonuses (added directly)
	public int AtkFlat { get; set; }
	public int SpellAtkFlat { get; set; }
	public int DefFlat { get; set; }
	public int SpellDefFlat { get; set; }
	public int HitFlat { get; set; }
	public int DodgeFlat { get; set; }
	public int CritFlat { get; set; }
	public int ParryFlat { get; set; }
	public int BlockFlat { get; set; }
	public int MaxHpFlat { get; set; }
	public int MaxMpFlat { get; set; }
	public int ResistFlat { get; set; }

	// Percentage bonuses (multiplicative)
	public float AtkPercent { get; set; }
	public float SpellAtkPercent { get; set; }
	public float DefPercent { get; set; }
	public float SpellDefPercent { get; set; }
	public float DodgePercent { get; set; }
	public float AttackSpeedPercent { get; set; }
	public float MaxHpPercent { get; set; }
	public float MaxMpPercent { get; set; }

	// Status flags
	public bool IsStunned { get; set; }
	public bool IsSilenced { get; set; }
	public bool IsRooted { get; set; }
	public bool IsInvincible { get; set; }

	// Movement modifiers
	public float MoveSpeedPercent { get; set; }
}
