using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.ItemSubjectXML;

public class ItemSeries
{
    [XmlAttribute(AttributeName = "typeID")] public int TypeId { get; set; }
    [XmlAttribute(AttributeName = "typeName")] public string TypeName { get; set; } = default!;
}

public class ItemType
{
    [XmlAttribute(AttributeName = "typeID")] public int TypeId { get; set; }
    [XmlAttribute(AttributeName = "typeName")] public string TypeName { get; set; } = default!;
}
