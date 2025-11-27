/// <summary>
/// XML-serializable quest definition from client quest data files.
/// </summary>
/// <remarks>
/// Parsed from: Data/xmls/quest/quest*.xml files
/// Structure based on client CBT3 quest XML format.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.QuestXML;

/// <summary>
/// Represents a single quest definition from the XML data.
/// </summary>
public class XmlQuest
{
	/// <summary>Unique quest type identifier.</summary>
	[XmlAttribute(AttributeName = "typeID")]
	public uint TypeID { get; set; }

	/// <summary>Quest display name (Korean).</summary>
	[XmlAttribute(AttributeName = "typeName")]
	public string TypeName { get; set; } = string.Empty;

	/// <summary>Quest group/category ID (000=History, 001=Race, etc).</summary>
	[XmlAttribute(AttributeName = "group")]
	public string Group { get; set; } = string.Empty;

	/// <summary>NPC that gives this quest.</summary>
	[XmlAttribute(AttributeName = "NPC")]
	public string NPC { get; set; } = string.Empty;

	/// <summary>NPC to report quest completion to.</summary>
	[XmlAttribute(AttributeName = "reportTo")]
	public string ReportTo { get; set; } = string.Empty;

	/// <summary>Whether the quest can be abandoned (1=yes, 0=no).</summary>
	[XmlAttribute(AttributeName = "cancelable")]
	public int Cancelable { get; set; }

	/// <summary>Whether the quest can be repeated (1=yes, 0=no).</summary>
	[XmlAttribute(AttributeName = "repeatable")]
	public int Repeatable { get; set; }

	/// <summary>Whether the quest can be retried after abandoning (1=yes, 0=no).</summary>
	[XmlAttribute(AttributeName = "retryable")]
	public int Retryable { get; set; }

	/// <summary>Required character level to accept quest.</summary>
	[XmlAttribute(AttributeName = "lv")]
	public int Level { get; set; }

	/// <summary>Quest description text (Korean, may contain formatting).</summary>
	[XmlAttribute(AttributeName = "desc")]
	public string Description { get; set; } = string.Empty;

	/// <summary>Quest difficulty (1-5).</summary>
	[XmlAttribute(AttributeName = "difficulty")]
	public int Difficulty { get; set; }

	/// <summary>Lua function name for quest message handling.</summary>
	[XmlAttribute(AttributeName = "onMessage")]
	public string OnMessage { get; set; } = string.Empty;

	/// <summary>Maximum number of choice rewards player can select.</summary>
	[XmlAttribute(AttributeName = "ChoiceRewardLimit")]
	public int ChoiceRewardLimit { get; set; }

	/// <summary>Base experience reward.</summary>
	[XmlAttribute(AttributeName = "rewardEXP")]
	public uint RewardEXP { get; set; }

	/// <summary>Life job experience reward.</summary>
	[XmlAttribute(AttributeName = "rewardLifeJobEXP")]
	public uint RewardLifeJobEXP { get; set; }

	/// <summary>Combat job experience reward.</summary>
	[XmlAttribute(AttributeName = "rewardCombatJobEXP")]
	public uint RewardCombatJobEXP { get; set; }

	/// <summary>Related job type (COMBAT, LIFE).</summary>
	[XmlAttribute(AttributeName = "relativeJob")]
	public string RelativeJob { get; set; } = string.Empty;

	/// <summary>Quest stages/subjects.</summary>
	[XmlElement(ElementName = "Subject")]
	public List<XmlQuestSubject> Subjects { get; set; } = new();

	/// <summary>Quest storage/objectives.</summary>
	[XmlElement(ElementName = "Storage")]
	public XmlQuestStorage? Storage { get; set; }

	/// <summary>Basic reward items.</summary>
	[XmlElement(ElementName = "BasicReward")]
	public XmlQuestReward? BasicReward { get; set; }

	/// <summary>Choice reward items.</summary>
	[XmlElement(ElementName = "ChoiceReward")]
	public List<XmlQuestReward> ChoiceRewards { get; set; } = new();

	/// <summary>
	/// Gets the quest group type based on group code.
	/// </summary>
	public QuestGroupType GetGroupType()
	{
		return Group switch
		{
			"000" => QuestGroupType.History,
			"001" => QuestGroupType.Race,
			"002" => QuestGroupType.Camp,
			"100" => QuestGroupType.Zone,
			"101" => QuestGroupType.Job,
			"102" => QuestGroupType.Legend,
			"200" => QuestGroupType.Hunting,
			"201" => QuestGroupType.Gather,
			"202" => QuestGroupType.Delivery,
			"203" => QuestGroupType.Default,
			"204" => QuestGroupType.Guard,
			"205" => QuestGroupType.Finding,
			"206" => QuestGroupType.Quiz,
			"207" => QuestGroupType.Making,
			_ => QuestGroupType.Default
		};
	}

	/// <summary>
	/// Checks if this quest is a main story quest.
	/// </summary>
	public bool IsMainQuest => Group == "000" || Group == "001" || Group == "002";

	/// <summary>
	/// Checks if this quest is a side quest.
	/// </summary>
	public bool IsSideQuest => Group == "100" || Group == "101" || Group == "102";

	/// <summary>
	/// Checks if this quest is a normal/repeatable quest.
	/// </summary>
	public bool IsNormalQuest => int.TryParse(Group, out int g) && g >= 200;
}

/// <summary>
/// Quest group/category types matching QuestInfo.xml icons.
/// </summary>
public enum QuestGroupType
{
	/// <summary>Main story quest.</summary>
	History = 0,
	/// <summary>Race-specific quest.</summary>
	Race = 1,
	/// <summary>Camp/faction quest.</summary>
	Camp = 2,
	/// <summary>Zone-specific quest.</summary>
	Zone = 100,
	/// <summary>Job class quest.</summary>
	Job = 101,
	/// <summary>Legend/epic quest.</summary>
	Legend = 102,
	/// <summary>Monster hunting quest.</summary>
	Hunting = 200,
	/// <summary>Gathering quest.</summary>
	Gather = 201,
	/// <summary>Delivery quest.</summary>
	Delivery = 202,
	/// <summary>Default/generic quest.</summary>
	Default = 203,
	/// <summary>Guard/escort quest.</summary>
	Guard = 204,
	/// <summary>Finding/exploration quest.</summary>
	Finding = 205,
	/// <summary>Quiz/puzzle quest.</summary>
	Quiz = 206,
	/// <summary>Crafting quest.</summary>
	Making = 207
}
