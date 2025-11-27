using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models.ItemMakeXML;

public class MakeStuffSkill
{
    [XmlAttribute(AttributeName = "skillID")] public int SkillId { get; set; }
    [XmlAttribute(AttributeName = "motionID")] public int MotionId { get; set; }
    [XmlAttribute(AttributeName = "groupId")] public int GroupId { get; set; }
    [XmlAttribute(AttributeName = "stuffType")] public int StuffType { get; set; }
}

public class MakeItemSkill
{
    [XmlAttribute(AttributeName = "skillID")] public int SkillId { get; set; }
    [XmlAttribute(AttributeName = "motionID")] public int MotionId { get; set; }
    [XmlAttribute(AttributeName = "groupId")] public int GroupId { get; set; }
    [XmlAttribute(AttributeName = "itemID1")] public int ItemId1 { get; set; }
    [XmlAttribute(AttributeName = "count1")] public int Count1 { get; set; }
    [XmlAttribute(AttributeName = "itemID2")] public int ItemId2 { get; set; }
    [XmlAttribute(AttributeName = "count2")] public int Count2 { get; set; }
    [XmlAttribute(AttributeName = "itemID3")] public int ItemId3 { get; set; }
    [XmlAttribute(AttributeName = "count3")] public int Count3 { get; set; }
    [XmlAttribute(AttributeName = "itemID4")] public int ItemId4 { get; set; }
    [XmlAttribute(AttributeName = "count4")] public int Count4 { get; set; }
    [XmlAttribute(AttributeName = "itemID5")] public int ItemId5 { get; set; }
    [XmlAttribute(AttributeName = "count5")] public int Count5 { get; set; }

    public List<(int itemId, int count)> GetRequiredItems()
    {
        var items = new List<(int itemId, int count)>();
        if (ItemId1 > 0 && Count1 > 0) items.Add((ItemId1, Count1));
        if (ItemId2 > 0 && Count2 > 0) items.Add((ItemId2, Count2));
        if (ItemId3 > 0 && Count3 > 0) items.Add((ItemId3, Count3));
        if (ItemId4 > 0 && Count4 > 0) items.Add((ItemId4, Count4));
        if (ItemId5 > 0 && Count5 > 0) items.Add((ItemId5, Count5));
        return items;
    }
}
