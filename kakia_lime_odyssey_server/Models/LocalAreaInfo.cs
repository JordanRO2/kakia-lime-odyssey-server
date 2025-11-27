/// <summary>
/// Loader and cache for local area (zone) information XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/World/LocalAreaInfo.xml
/// Contains zone/area definitions with display information.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for local area info XML file.
/// </summary>
[XmlRoot(ElementName = "AreaInfoRoot")]
public class LocalAreaInfo
{
    /// <summary>Area display position settings.</summary>
    [XmlElement(ElementName = "AreaInfoPos")]
    public AreaInfoPosition Position { get; set; } = new();

    /// <summary>List of area definitions.</summary>
    [XmlElement(ElementName = "AreaInfo")]
    public List<AreaInfoEntry> Areas { get; set; } = new();

    private static LocalAreaInfo? _instance;
    private static Dictionary<int, AreaInfoEntry>? _indexCache;
    private static Dictionary<int, AreaInfoEntry>? _nameIdCache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>LocalAreaInfo instance.</returns>
    public static LocalAreaInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(LocalAreaInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.World.LocalAreaInfo, FileMode.Open);
        _instance = (LocalAreaInfo)serializer.Deserialize(fileStream)!;

        _indexCache = new Dictionary<int, AreaInfoEntry>();
        _nameIdCache = new Dictionary<int, AreaInfoEntry>();

        foreach (var area in _instance.Areas)
        {
            _indexCache[area.Index] = area;
            _nameIdCache[area.NameId] = area;
        }

        return _instance;
    }

    /// <summary>
    /// Gets an area by index.
    /// </summary>
    /// <param name="index">Area index.</param>
    /// <returns>AreaInfoEntry or null if not found.</returns>
    public static AreaInfoEntry? GetAreaByIndex(int index)
    {
        GetInstance();
        return _indexCache!.TryGetValue(index, out var area) ? area : null;
    }

    /// <summary>
    /// Gets an area by name ID.
    /// </summary>
    /// <param name="nameId">Area name ID.</param>
    /// <returns>AreaInfoEntry or null if not found.</returns>
    public static AreaInfoEntry? GetAreaByNameId(int nameId)
    {
        GetInstance();
        return _nameIdCache!.TryGetValue(nameId, out var area) ? area : null;
    }

    /// <summary>
    /// Gets all areas.
    /// </summary>
    /// <returns>List of all area entries.</returns>
    public static List<AreaInfoEntry> GetAllAreas()
    {
        return GetInstance().Areas;
    }

    /// <summary>
    /// Gets the area name by index.
    /// </summary>
    /// <param name="index">Area index.</param>
    /// <returns>Area name or empty string if not found.</returns>
    public static string GetAreaName(int index)
    {
        var area = GetAreaByIndex(index);
        return area?.Name ?? string.Empty;
    }
}

/// <summary>
/// Area display position settings.
/// </summary>
public class AreaInfoPosition
{
    /// <summary>X position ratio.</summary>
    [XmlAttribute(AttributeName = "posX")]
    public float PosX { get; set; }

    /// <summary>Y position ratio.</summary>
    [XmlAttribute(AttributeName = "posY")]
    public float PosY { get; set; }
}

/// <summary>
/// Area information entry.
/// </summary>
public class AreaInfoEntry
{
    /// <summary>Area index.</summary>
    [XmlAttribute(AttributeName = "index")]
    public int Index { get; set; }

    /// <summary>Area name.</summary>
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Name ID reference.</summary>
    [XmlAttribute(AttributeName = "nameID")]
    public int NameId { get; set; }

    /// <summary>Display time in milliseconds.</summary>
    [XmlAttribute(AttributeName = "showtime")]
    public int ShowTime { get; set; }

    /// <summary>Display width.</summary>
    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    /// <summary>Display height.</summary>
    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }

    /// <summary>Wallpaper/background image path.</summary>
    [XmlAttribute(AttributeName = "wallpapername")]
    public string WallpaperName { get; set; } = string.Empty;

    /// <summary>Comment/description.</summary>
    [XmlAttribute(AttributeName = "comment")]
    public string Comment { get; set; } = string.Empty;
}
