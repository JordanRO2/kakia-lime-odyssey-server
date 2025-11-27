using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.BuffXML;

[XmlRoot(ElementName = "buff")]
public class XmlBuff
{
    [XmlAttribute(AttributeName = "id")] public int Id { get; set; }
    [XmlAttribute(AttributeName = "name")] public string Name { get; set; } = default!;
    [XmlAttribute(AttributeName = "buffExp")] public string BuffExp { get; set; } = default!;
    [XmlAttribute(AttributeName = "imageName")] public string ImageName { get; set; } = default!;
    [XmlAttribute(AttributeName = "smallImageName")] public string SmallImageName { get; set; } = default!;
    [XmlAttribute(AttributeName = "buffType")] public int BuffType { get; set; }
    [XmlAttribute(AttributeName = "stun")] public int Stun { get; set; }
    [XmlElement(ElementName = "buffState")] public BuffState? BuffState { get; set; }
    [XmlElement(ElementName = "Subject")] public BuffSubject? Subject { get; set; }
}

public class BuffState
{
    [XmlElement(ElementName = "evnet")] public List<BuffEvent> Events { get; set; } = new();
}

public class BuffEvent
{
    [XmlAttribute(AttributeName = "motionID")] public int MotionId { get; set; }
    [XmlElement(ElementName = "effectKind")] public List<EffectKind> Effects { get; set; } = new();
}

public class EffectKind
{
    [XmlAttribute(AttributeName = "effectID")] public int EffectId { get; set; }
    [XmlAttribute(AttributeName = "effectOrder")] public string EffectOrder { get; set; } = default!;
}

public class BuffSubject
{
    [XmlElement(ElementName = "SubjectList")] public List<BuffSubjectList> SubjectLists { get; set; } = new();
}

public class BuffSubjectList
{
    [XmlAttribute(AttributeName = "subjectLevel")] public int SubjectLevel { get; set; }
    [XmlAttribute(AttributeName = "detail")] public string Detail { get; set; } = default!;
}
