/// <summary>
/// XML-serializable quest storage/objectives definition.
/// </summary>
/// <remarks>
/// Contains hunt targets, collect items, and other quest objectives.
/// Uses dynamic attributes for Hunt1_typeID, Collect1_typeID patterns.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.QuestXML;

/// <summary>
/// Represents quest storage with objectives (hunt/collect targets).
/// </summary>
/// <remarks>
/// The client XML uses dynamic attribute names like Hunt1_typeID, Hunt2_typeID,
/// Collect1_typeID, etc. This class captures the common patterns.
/// </remarks>
public class XmlQuestStorage
{
	/// <summary>First hunt target monster type ID.</summary>
	[XmlAttribute(AttributeName = "Hunt1_typeID")]
	public int Hunt1TypeID { get; set; }

	/// <summary>First hunt target goal count.</summary>
	[XmlAttribute(AttributeName = "Hunt1_goal")]
	public int Hunt1Goal { get; set; }

	/// <summary>First hunt current count (always 0 in data).</summary>
	[XmlAttribute(AttributeName = "Hunt1_count")]
	public int Hunt1Count { get; set; }

	/// <summary>First target type (Hunt, Collect, etc).</summary>
	[XmlAttribute(AttributeName = "target1_type")]
	public string Target1Type { get; set; } = string.Empty;

	/// <summary>Second hunt target monster type ID.</summary>
	[XmlAttribute(AttributeName = "Hunt2_typeID")]
	public int Hunt2TypeID { get; set; }

	/// <summary>Second hunt target goal count.</summary>
	[XmlAttribute(AttributeName = "Hunt2_goal")]
	public int Hunt2Goal { get; set; }

	/// <summary>Second hunt current count (always 0 in data).</summary>
	[XmlAttribute(AttributeName = "Hunt2_count")]
	public int Hunt2Count { get; set; }

	/// <summary>Second target type.</summary>
	[XmlAttribute(AttributeName = "target2_type")]
	public string Target2Type { get; set; } = string.Empty;

	/// <summary>Third hunt target monster type ID.</summary>
	[XmlAttribute(AttributeName = "Hunt3_typeID")]
	public int Hunt3TypeID { get; set; }

	/// <summary>Third hunt target goal count.</summary>
	[XmlAttribute(AttributeName = "Hunt3_goal")]
	public int Hunt3Goal { get; set; }

	/// <summary>Third hunt current count.</summary>
	[XmlAttribute(AttributeName = "Hunt3_count")]
	public int Hunt3Count { get; set; }

	/// <summary>Third target type.</summary>
	[XmlAttribute(AttributeName = "target3_type")]
	public string Target3Type { get; set; } = string.Empty;

	/// <summary>Number of targets in this quest.</summary>
	[XmlAttribute(AttributeName = "target_number")]
	public int TargetNumber { get; set; }

	/// <summary>First collect item type ID.</summary>
	[XmlAttribute(AttributeName = "Collect1_typeID")]
	public int Collect1TypeID { get; set; }

	/// <summary>First collect item goal count.</summary>
	[XmlAttribute(AttributeName = "Collect1_goal")]
	public int Collect1Goal { get; set; }

	/// <summary>First collect current count.</summary>
	[XmlAttribute(AttributeName = "Collect1_count")]
	public int Collect1Count { get; set; }

	/// <summary>First collect drop probability (0-100).</summary>
	[XmlAttribute(AttributeName = "target1_prob")]
	public int Target1Probability { get; set; }

	/// <summary>Second collect item type ID.</summary>
	[XmlAttribute(AttributeName = "Collect2_typeID")]
	public int Collect2TypeID { get; set; }

	/// <summary>Second collect item goal count.</summary>
	[XmlAttribute(AttributeName = "Collect2_goal")]
	public int Collect2Goal { get; set; }

	/// <summary>Second collect current count.</summary>
	[XmlAttribute(AttributeName = "Collect2_count")]
	public int Collect2Count { get; set; }

	/// <summary>Third collect item type ID.</summary>
	[XmlAttribute(AttributeName = "Collect3_typeID")]
	public int Collect3TypeID { get; set; }

	/// <summary>Third collect item goal count.</summary>
	[XmlAttribute(AttributeName = "Collect3_goal")]
	public int Collect3Goal { get; set; }

	/// <summary>Third collect current count.</summary>
	[XmlAttribute(AttributeName = "Collect3_count")]
	public int Collect3Count { get; set; }

	/// <summary>Temporary placeholder value.</summary>
	[XmlAttribute(AttributeName = "temp")]
	public int Temp { get; set; }

	/// <summary>
	/// Gets all hunt objectives from this storage.
	/// </summary>
	public List<QuestObjective> GetHuntObjectives()
	{
		var objectives = new List<QuestObjective>();

		if (Hunt1TypeID > 0)
		{
			objectives.Add(new QuestObjective
			{
				Type = QuestObjectiveType.Hunt,
				TargetTypeID = Hunt1TypeID,
				Goal = Hunt1Goal > 0 ? Hunt1Goal : 1,
				Index = 0
			});
		}

		if (Hunt2TypeID > 0)
		{
			objectives.Add(new QuestObjective
			{
				Type = QuestObjectiveType.Hunt,
				TargetTypeID = Hunt2TypeID,
				Goal = Hunt2Goal > 0 ? Hunt2Goal : 1,
				Index = 1
			});
		}

		if (Hunt3TypeID > 0)
		{
			objectives.Add(new QuestObjective
			{
				Type = QuestObjectiveType.Hunt,
				TargetTypeID = Hunt3TypeID,
				Goal = Hunt3Goal > 0 ? Hunt3Goal : 1,
				Index = 2
			});
		}

		return objectives;
	}

	/// <summary>
	/// Gets all collect objectives from this storage.
	/// </summary>
	public List<QuestObjective> GetCollectObjectives()
	{
		var objectives = new List<QuestObjective>();

		if (Collect1TypeID > 0)
		{
			objectives.Add(new QuestObjective
			{
				Type = QuestObjectiveType.Collect,
				TargetTypeID = Collect1TypeID,
				Goal = Collect1Goal > 0 ? Collect1Goal : 1,
				DropProbability = Target1Probability > 0 ? Target1Probability : 100,
				Index = 0
			});
		}

		if (Collect2TypeID > 0)
		{
			objectives.Add(new QuestObjective
			{
				Type = QuestObjectiveType.Collect,
				TargetTypeID = Collect2TypeID,
				Goal = Collect2Goal > 0 ? Collect2Goal : 1,
				Index = 1
			});
		}

		if (Collect3TypeID > 0)
		{
			objectives.Add(new QuestObjective
			{
				Type = QuestObjectiveType.Collect,
				TargetTypeID = Collect3TypeID,
				Goal = Collect3Goal > 0 ? Collect3Goal : 1,
				Index = 2
			});
		}

		return objectives;
	}

	/// <summary>
	/// Gets all objectives from this storage.
	/// </summary>
	public List<QuestObjective> GetAllObjectives()
	{
		var objectives = new List<QuestObjective>();
		objectives.AddRange(GetHuntObjectives());
		objectives.AddRange(GetCollectObjectives());
		return objectives;
	}
}

/// <summary>
/// Represents a single quest objective.
/// </summary>
public class QuestObjective
{
	/// <summary>Type of objective (Hunt, Collect, etc).</summary>
	public QuestObjectiveType Type { get; set; }

	/// <summary>Target type ID (monster ID for hunt, item ID for collect).</summary>
	public int TargetTypeID { get; set; }

	/// <summary>Number required to complete objective.</summary>
	public int Goal { get; set; }

	/// <summary>Drop probability for collect objectives (0-100).</summary>
	public int DropProbability { get; set; } = 100;

	/// <summary>Objective index (0, 1, 2).</summary>
	public int Index { get; set; }
}

/// <summary>
/// Quest objective types.
/// </summary>
public enum QuestObjectiveType
{
	/// <summary>Kill monsters.</summary>
	Hunt,
	/// <summary>Collect items.</summary>
	Collect,
	/// <summary>Talk to NPC.</summary>
	Talk,
	/// <summary>Deliver item to NPC.</summary>
	Deliver,
	/// <summary>Explore/find location.</summary>
	Explore
}
