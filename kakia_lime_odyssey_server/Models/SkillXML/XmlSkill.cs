/// <summary>
/// XML skill definition loaded from Skills.xml.
/// </summary>
/// <remarks>
/// Skill ID ranges:
/// - 1-14, 501-507: Fighter skills (physical)
/// - 1001-1010: Thief/Assassin skills (physical)
/// - 2001-2017: Priest skills (magical/support)
/// - 3001-3012: Mage skills (magical)
/// - 5000+: Life job skills (crafting/gathering)
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.SkillXML;

/// <summary>
/// Skill damage type determines which stats are used for calculation.
/// </summary>
public enum SkillDamageType
{
	/// <summary>Physical damage uses melee attack/defense stats.</summary>
	Physical,
	/// <summary>Magical damage uses spell attack/defense stats.</summary>
	Magical,
	/// <summary>Hybrid damage uses average of melee and spell stats.</summary>
	Hybrid,
	/// <summary>Non-combat skill (crafting, gathering, etc.).</summary>
	None
}

/// <summary>
/// Skill category based on job class.
/// </summary>
public enum SkillCategory
{
	/// <summary>Fighter combat skills (ID 1-14, 501-507).</summary>
	Fighter,
	/// <summary>Thief/Assassin combat skills (ID 1001-1010).</summary>
	Thief,
	/// <summary>Priest combat skills (ID 2001-2017).</summary>
	Priest,
	/// <summary>Mage combat skills (ID 3001-3012).</summary>
	Mage,
	/// <summary>Life job skills (crafting, gathering).</summary>
	LifeJob,
	/// <summary>Unknown or miscellaneous skills.</summary>
	Unknown
}

/// <summary>
/// XML skill definition from Skills.xml.
/// </summary>
public class XmlSkill
{
	[XmlAttribute(AttributeName = "id")] public int Id { get; set; }
	[XmlAttribute(AttributeName = "type")] public string Type { get; set; } = default!;
	[XmlAttribute(AttributeName = "name")] public string Name { get; set; } = default!;
	[XmlAttribute(AttributeName = "nameEng")] public string NameEng { get; set; } = default!;
	[XmlAttribute(AttributeName = "target")] public int Target { get; set; }
	[XmlAttribute(AttributeName = "targetAttribute")] public int TargetAttribute { get; set; }
	[XmlAttribute(AttributeName = "class")] public string Class { get; set; } = default!;
	[XmlAttribute(AttributeName = "castingTime")] public double CastingTime { get; set; }
	[XmlAttribute(AttributeName = "coolTime")] public double CoolTime { get; set; }
	[XmlAttribute(AttributeName = "imageName")] public string ImageName { get; set; } = default!;
	[XmlAttribute(AttributeName = "slot")] public int Slot { get; set; }
	[XmlAttribute(AttributeName = "comBoGauge")] public int ComboGauge { get; set; }
	[XmlAttribute(AttributeName = "isCombo")] public int IsCombo { get; set; }
	[XmlAttribute(AttributeName = "weaponOnBody")] public int WeaponOnBody { get; set; }
	[XmlAttribute(AttributeName = "skillStatusType")] public int SkillStatusType { get; set; }
	[XmlAttribute(AttributeName = "castingAttr")] public int CastingAttr { get; set; }
	[XmlAttribute(AttributeName = "soundid")] public int SoundId { get; set; }
	[XmlAttribute(AttributeName = "range")] public double Range { get; set; }
	[XmlAttribute(AttributeName = "chainSkill")] public int ChainSkill { get; set; }
	[XmlAttribute(AttributeName = "series")] public int Series { get; set; }
	[XmlAttribute(AttributeName = "motionDelay")] public double MotionDelay { get; set; }
	[XmlElement(ElementName = "Subject")] public SkillSubject Subject { get; set; } = default!;

	/// <summary>
	/// Gets the skill category based on skill ID range.
	/// </summary>
	public SkillCategory GetCategory()
	{
		return Id switch
		{
			>= 1 and <= 14 => SkillCategory.Fighter,
			>= 501 and <= 599 => SkillCategory.Fighter,
			>= 1001 and <= 1999 => SkillCategory.Thief,
			>= 2001 and <= 2999 => SkillCategory.Priest,
			>= 3001 and <= 3999 => SkillCategory.Mage,
			>= 5000 => SkillCategory.LifeJob,
			_ => SkillCategory.Unknown
		};
	}

	/// <summary>
	/// Gets the damage type for this skill based on its category.
	/// </summary>
	public SkillDamageType GetDamageType()
	{
		var category = GetCategory();
		return category switch
		{
			SkillCategory.Fighter => SkillDamageType.Physical,
			SkillCategory.Thief => SkillDamageType.Physical,
			SkillCategory.Priest => SkillDamageType.Magical,
			SkillCategory.Mage => SkillDamageType.Magical,
			SkillCategory.LifeJob => SkillDamageType.None,
			_ => SkillDamageType.Physical
		};
	}

	/// <summary>
	/// Gets whether this is a combat skill (deals damage).
	/// </summary>
	public bool IsCombatSkill => GetDamageType() != SkillDamageType.None;

	/// <summary>
	/// Gets whether this is a passive/training skill.
	/// </summary>
	public bool IsPassive => SkillStatusType == 3 || SkillStatusType == 2;

	/// <summary>
	/// Gets whether this is an active skill.
	/// </summary>
	public bool IsActive => SkillStatusType == 1;

	/// <summary>
	/// Gets whether this skill has a casting time.
	/// </summary>
	public bool HasCastTime => CastingTime > 0;

	/// <summary>
	/// Gets the cooldown in seconds.
	/// </summary>
	public double CooldownSeconds => CoolTime / 1000.0;
}
