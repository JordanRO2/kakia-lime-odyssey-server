namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Equipment inherit/bonus type IDs as defined in inherit.xml
/// </summary>
public enum InheritType
{
	// Base Stats (1-7)
	ExtraSTR = 1,
	ExtraINT = 2,
	ExtraDEX = 3,
	ExtraAGI = 4,
	ExtraVIT = 5,
	ExtraSPI = 6,
	ExtraLUK = 7,

	// Crafting/Life Stats (8-11)
	ExtraIDE = 8,       // Creativity
	ExtraSES = 9,       // Sensitivity
	ExtraMID = 10,      // Mentality
	ExtraCRT = 11,      // Activity

	// HP/MP/LP (12-14)
	ExtraMHP = 12,
	ExtraMMP = 13,
	ExtraMLP = 14,

	// Attack (15-17)
	ExtraMeleeAtk = 15,
	ExtraSpellAtk = 16,
	ExtraRangedAtk = 17,

	// Hit Range/Angle (18-19)
	ExtraHitRange = 18,
	ExtraHitAngle = 19,

	// Hit Rates (20-22)
	ExtraMeleeHitRate = 20,
	ExtraRangedHitRate = 21,
	HitAccurate = 22,

	// Defense (23-24)
	ExtraMeleeDefense = 23,
	ExtraSpellDefense = 24,

	// Dodge/Evasion (25-26)
	ExtraDodgeAccurate = 25,
	ExtraDodge = 26,

	// Other Combat (27-34)
	ExtraResist = 27,
	ExtraParry = 28,
	ExtraBlock = 29,
	ExtraVelocityRatio = 30,
	ExtraHpRegenRate = 31,
	ExtraHpRegenAmount = 32,
	ExtraMpRegenRate = 33,
	ExtraMpRegenAmount = 34,

	// Critical (35-36)
	ExtraCriticalEffect = 35,
	ExtraCriticalRate = 36,

	// Final Attack/Defense (37-41)
	ExtraFinalSpellAtk = 37,
	ExtraFinalRangedAtk = 38,
	ExtraFinalMeleeAtk = 39,
	ExtraFinalMeleeDefense = 40,
	ExtraFinalSpellDefense = 41,

	// Casting (42-45)
	ExtraHitSpeedRatio = 42,
	ExtraCastingTimeDecrease = 43,
	ExtraCastingInterrupt = 44,
	ExtraCastingPreserve = 45,

	// Percentage Bonuses (46-58)
	ExtraMHPRate = 46,
	ExtraMMPRate = 47,
	ExtraMLPRate = 48,
	ExtraMeleeAtkRate = 49,
	ExtraSpellAtkRate = 50,
	ExtraRangedAtkRate = 51,
	ExtraMeleeDefenseRate = 52,
	ExtraSpellDefenseRate = 53,
	DodgeAccurateRate = 54,
	ExtraHitAccurateRate = 55,
	ExtraCastingTimeDecreaseRate = 56,
	ExtraCastingInterruptRate = 57,
	ExtraCastingPreserveRate = 58,

	// Transform (59-61)
	TransMeleeHitRate = 59,
	TransDodge = 60,
	TransCriticalRate = 61
}
