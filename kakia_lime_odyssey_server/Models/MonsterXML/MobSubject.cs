using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.MonsterXML;

public class MobSubject
{
	[XmlAttribute(AttributeName = "typeName")]
	public string TypeName { get; set; } = default!;
	[XmlAttribute(AttributeName = "eventID")]
	public uint EventID { get; set; }
}
