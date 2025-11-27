/// <summary>
/// XML-serializable quest subject/stage definition.
/// </summary>
/// <remarks>
/// Represents a quest stage (Begin, Success, etc) with objectives and map targets.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.QuestXML;

/// <summary>
/// Represents a quest stage/subject with objectives and navigation hints.
/// </summary>
public class XmlQuestSubject
{
	/// <summary>Stage type name (Begin, Success, etc).</summary>
	[XmlAttribute(AttributeName = "typeName")]
	public string TypeName { get; set; } = string.Empty;

	/// <summary>Lua function to call for this stage.</summary>
	[XmlAttribute(AttributeName = "func")]
	public string Function { get; set; } = string.Empty;

	/// <summary>Stage description/objective text.</summary>
	[XmlAttribute(AttributeName = "desc")]
	public string Description { get; set; } = string.Empty;

	/// <summary>Draft/detailed explanation text.</summary>
	[XmlAttribute(AttributeName = "draft")]
	public string Draft { get; set; } = string.Empty;

	/// <summary>NPC associated with this stage.</summary>
	[XmlAttribute(AttributeName = "npc")]
	public string NPC { get; set; } = string.Empty;

	/// <summary>Map positions for quest objectives.</summary>
	[XmlElement(ElementName = "subjectPos")]
	public List<XmlQuestSubjectPos> Positions { get; set; } = new();

	/// <summary>
	/// Gets the stage type enum value.
	/// </summary>
	public QuestStageType GetStageType()
	{
		return TypeName?.ToLower() switch
		{
			"begin" => QuestStageType.Begin,
			"success" => QuestStageType.Success,
			"fail" => QuestStageType.Fail,
			"progress" => QuestStageType.Progress,
			_ => QuestStageType.Unknown
		};
	}
}

/// <summary>
/// Represents a map position marker for quest objectives.
/// </summary>
public class XmlQuestSubjectPos
{
	/// <summary>Display text for this objective marker.</summary>
	[XmlAttribute(AttributeName = "subjectStr")]
	public string SubjectString { get; set; } = string.Empty;

	/// <summary>Map target ID for navigation.</summary>
	[XmlAttribute(AttributeName = "mapTargetId")]
	public int MapTargetId { get; set; }
}

/// <summary>
/// Quest stage types.
/// </summary>
public enum QuestStageType
{
	/// <summary>Unknown stage type.</summary>
	Unknown = 0,
	/// <summary>Initial quest acceptance stage.</summary>
	Begin = 1,
	/// <summary>Quest in progress.</summary>
	Progress = 2,
	/// <summary>Quest completion stage.</summary>
	Success = 3,
	/// <summary>Quest failed state.</summary>
	Fail = 4
}
