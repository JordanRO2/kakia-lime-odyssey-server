/// <summary>
/// XML-serializable quest reward definition.
/// </summary>
/// <remarks>
/// Represents item rewards from quests (basic or choice rewards).
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.QuestXML;

/// <summary>
/// Represents a quest reward item.
/// </summary>
public class XmlQuestReward
{
	/// <summary>Item type ID to reward.</summary>
	[XmlAttribute(AttributeName = "typeID")]
	public uint TypeID { get; set; }

	/// <summary>Number of items to reward.</summary>
	[XmlAttribute(AttributeName = "count")]
	public int Count { get; set; }

	/// <summary>Reward slot index (for choice rewards).</summary>
	[XmlAttribute(AttributeName = "slot")]
	public int Slot { get; set; }
}
