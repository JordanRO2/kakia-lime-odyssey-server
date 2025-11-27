using kakia_lime_odyssey_server.Models.ItemMakeXML;
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

[XmlRoot(ElementName = "ItemMakeInfo")]
public class ItemMakeInfo
{
    [XmlElement(ElementName = "MakeStuffSkill")]
    public List<MakeStuffSkill> MakeStuffSkills { get; set; } = new();

    [XmlElement(ElementName = "MakeItemSkill")]
    public List<MakeItemSkill> MakeItemSkills { get; set; } = new();

    private static ItemMakeInfo? _instance;

    public static ItemMakeInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(ItemMakeInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Items.ItemMakeInfo, FileMode.Open);
        _instance = (ItemMakeInfo)serializer.Deserialize(fileStream)!;

        return _instance;
    }

    public static MakeStuffSkill? GetMakeStuffSkill(int skillId)
    {
        var instance = GetInstance();
        return instance.MakeStuffSkills.FirstOrDefault(s => s.SkillId == skillId);
    }

    public static MakeItemSkill? GetMakeItemSkill(int skillId)
    {
        var instance = GetInstance();
        return instance.MakeItemSkills.FirstOrDefault(s => s.SkillId == skillId);
    }

    public static List<MakeStuffSkill> GetMakeStuffSkillsByGroup(int groupId)
    {
        var instance = GetInstance();
        return instance.MakeStuffSkills.Where(s => s.GroupId == groupId).ToList();
    }

    public static List<MakeItemSkill> GetMakeItemSkillsByGroup(int groupId)
    {
        var instance = GetInstance();
        return instance.MakeItemSkills.Where(s => s.GroupId == groupId).ToList();
    }
}
