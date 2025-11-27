using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.InheritXML;

[XmlRoot(ElementName = "Inherit")]
public class XmlInherit
{
    [XmlAttribute(AttributeName = "typeID")] public int TypeId { get; set; }
    [XmlAttribute(AttributeName = "typeName")] public string TypeName { get; set; } = default!;
    [XmlAttribute(AttributeName = "desc")] public string Description { get; set; } = default!;
    [XmlElement(ElementName = "soketItemTypeID")] public List<SocketItem> SocketItems { get; set; } = new();
}

public class SocketItem
{
    [XmlAttribute(AttributeName = "value")] public int Value { get; set; }
    [XmlAttribute(AttributeName = "itemTypeID")] public int ItemTypeId { get; set; }
}
