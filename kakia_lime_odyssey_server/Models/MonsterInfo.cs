using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Models.MonsterXML;
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

[XmlRoot(ElementName = "NPC")]
public class MonsterInfo
{
	[XmlElement(ElementName = "Monster")]
	public List<XmlMonster> Monsters {	get; set; } = default!;

	public static List<XmlMonster> GetEntries()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(MonsterInfo));
		using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Entities.Monsters, FileMode.Open);
		var mobInfo = (MonsterInfo)serializer.Deserialize(fileStream)!;

		var models = ModelInfo.GetEntries();

		foreach (var mob in mobInfo.Monsters)
		{
			// Name is used directly from XML (localization is client-side)
			mob.Model = models.FirstOrDefault(model => model.TypeId.Equals(mob.ModelTypeID)) ?? new();
		}

		return mobInfo.Monsters;
	}
}

public class MapMob
{
	public int Id { get; set; }
	public int ZoneId { get; set; }
	public int ModelTypeId { get; set; }
	public string Name { get; set; } = default!;
	public FPOS Pos { get; set; }
	public int LootTableId { get; set; }
}