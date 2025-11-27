using kakia_lime_odyssey_server.Models.InheritXML;
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

[XmlRoot(ElementName = "Inherit")]
public class InheritInfo
{
    [XmlElement(ElementName = "Inherit")]
    public List<XmlInherit> Inherits { get; set; } = default!;

    private static Dictionary<int, XmlInherit>? _cache;

    public static Dictionary<int, XmlInherit> GetEntries()
    {
        if (_cache != null) return _cache;

        XmlSerializer serializer = new XmlSerializer(typeof(InheritInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Characters.Inherit, FileMode.Open);
        var inheritInfo = (InheritInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, XmlInherit>();
        foreach (var inherit in inheritInfo.Inherits)
            _cache.Add(inherit.TypeId, inherit);

        return _cache;
    }

    public static XmlInherit? GetInherit(int typeId)
    {
        var entries = GetEntries();
        return entries.TryGetValue(typeId, out var inherit) ? inherit : null;
    }

    public static XmlInherit? GetInheritByName(string typeName)
    {
        var entries = GetEntries();
        return entries.Values.FirstOrDefault(i => i.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase));
    }

    public static int GetSocketItemId(int inheritTypeId, int socketValue)
    {
        var inherit = GetInherit(inheritTypeId);
        if (inherit == null) return -1;

        var socketItem = inherit.SocketItems.FirstOrDefault(s => s.Value == socketValue);
        return socketItem?.ItemTypeId ?? -1;
    }
}
