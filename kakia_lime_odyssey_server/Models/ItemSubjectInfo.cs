using kakia_lime_odyssey_server.Models.ItemSubjectXML;
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

[XmlRoot(ElementName = "Root")]
public class ItemSubjectInfo
{
    [XmlElement(ElementName = "series")]
    public List<ItemSeries> Series { get; set; } = new();

    [XmlElement(ElementName = "ItemType")]
    public List<ItemType> ItemTypes { get; set; } = new();

    private static ItemSubjectInfo? _instance;
    private static Dictionary<int, ItemSeries>? _seriesCache;
    private static Dictionary<int, ItemType>? _itemTypeCache;

    public static ItemSubjectInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(ItemSubjectInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Items.Categories, FileMode.Open);
        _instance = (ItemSubjectInfo)serializer.Deserialize(fileStream)!;

        _seriesCache = new Dictionary<int, ItemSeries>();
        foreach (var series in _instance.Series)
        {
            if (!_seriesCache.ContainsKey(series.TypeId))
                _seriesCache.Add(series.TypeId, series);
        }

        _itemTypeCache = new Dictionary<int, ItemType>();
        foreach (var itemType in _instance.ItemTypes)
        {
            if (!_itemTypeCache.ContainsKey(itemType.TypeId))
                _itemTypeCache.Add(itemType.TypeId, itemType);
        }

        return _instance;
    }

    public static ItemSeries? GetSeries(int typeId)
    {
        GetInstance();
        return _seriesCache!.TryGetValue(typeId, out var series) ? series : null;
    }

    public static ItemType? GetItemType(int typeId)
    {
        GetInstance();
        return _itemTypeCache!.TryGetValue(typeId, out var itemType) ? itemType : null;
    }

    public static string GetSeriesName(int typeId)
    {
        var series = GetSeries(typeId);
        return series?.TypeName ?? "Unknown";
    }

    public static string GetItemTypeName(int typeId)
    {
        var itemType = GetItemType(typeId);
        return itemType?.TypeName ?? "Unknown";
    }
}
