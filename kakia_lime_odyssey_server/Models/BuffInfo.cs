using kakia_lime_odyssey_server.Models.BuffXML;
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

[XmlRoot(ElementName = "BuffInfo")]
public class BuffInfo
{
    [XmlElement(ElementName = "buff")]
    public List<XmlBuff> Buffs { get; set; } = default!;

    public static Dictionary<int, XmlBuff> GetEntries()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(BuffInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Buffs.BuffInfo, FileMode.Open);
        var buffInfo = (BuffInfo)serializer.Deserialize(fileStream)!;

        var entries = new Dictionary<int, XmlBuff>();
        foreach (var buff in buffInfo.Buffs)
            entries.Add(buff.Id, buff);

        return entries;
    }

    public static XmlBuff? GetBuff(int id)
    {
        var entries = GetEntries();
        return entries.TryGetValue(id, out var buff) ? buff : null;
    }

    public static XmlBuff? GetBuffByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        var entries = GetEntries();
        return entries.Values.FirstOrDefault(b =>
            b.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
