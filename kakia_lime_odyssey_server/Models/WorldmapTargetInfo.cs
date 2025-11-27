/// <summary>
/// Loader and cache for world map target information XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/World/WorldmapTargetInfo.xml
/// Contains world map interaction points and zone connections.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for world map target info XML file.
/// </summary>
[XmlRoot(ElementName = "WorldmapTargetInfo")]
public class WorldmapTargetInfo
{
    /// <summary>List of world map target entries.</summary>
    [XmlElement(ElementName = "Target")]
    public List<WorldmapTargetEntry> Targets { get; set; } = new();

    private static WorldmapTargetInfo? _instance;
    private static Dictionary<int, WorldmapTargetEntry>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>WorldmapTargetInfo instance.</returns>
    public static WorldmapTargetInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(WorldmapTargetInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.World.WorldmapTargetInfo, FileMode.Open);
        _instance = (WorldmapTargetInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, WorldmapTargetEntry>();
        foreach (var target in _instance.Targets)
            _cache[target.Id] = target;

        return _instance;
    }

    /// <summary>
    /// Gets a world map target by ID.
    /// </summary>
    /// <param name="id">Target ID.</param>
    /// <returns>WorldmapTargetEntry or null if not found.</returns>
    public static WorldmapTargetEntry? GetTarget(int id)
    {
        GetInstance();
        return _cache!.TryGetValue(id, out var target) ? target : null;
    }

    /// <summary>
    /// Gets all world map targets.
    /// </summary>
    /// <returns>List of all world map target entries.</returns>
    public static List<WorldmapTargetEntry> GetAllTargets()
    {
        return GetInstance().Targets;
    }

    /// <summary>
    /// Gets world map targets by zone.
    /// </summary>
    /// <param name="zoneId">Zone ID.</param>
    /// <returns>List of targets in the zone.</returns>
    public static List<WorldmapTargetEntry> GetTargetsByZone(int zoneId)
    {
        return GetInstance().Targets.Where(t => t.ZoneId == zoneId).ToList();
    }
}

/// <summary>
/// World map target entry.
/// </summary>
public class WorldmapTargetEntry
{
    /// <summary>Target ID.</summary>
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    /// <summary>Target name.</summary>
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Target type.</summary>
    [XmlAttribute(AttributeName = "type")]
    public int Type { get; set; }

    /// <summary>Zone ID.</summary>
    [XmlAttribute(AttributeName = "zoneId")]
    public int ZoneId { get; set; }

    /// <summary>X position on world map.</summary>
    [XmlAttribute(AttributeName = "x")]
    public float X { get; set; }

    /// <summary>Y position on world map.</summary>
    [XmlAttribute(AttributeName = "y")]
    public float Y { get; set; }

    /// <summary>Width of the target area.</summary>
    [XmlAttribute(AttributeName = "width")]
    public float Width { get; set; }

    /// <summary>Height of the target area.</summary>
    [XmlAttribute(AttributeName = "height")]
    public float Height { get; set; }

    /// <summary>Icon image name.</summary>
    [XmlAttribute(AttributeName = "iconName")]
    public string IconName { get; set; } = string.Empty;

    /// <summary>Description or comment.</summary>
    [XmlAttribute(AttributeName = "desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Name ID for localization.</summary>
    [XmlAttribute(AttributeName = "nameId")]
    public int NameId { get; set; }

    /// <summary>Level range minimum.</summary>
    [XmlAttribute(AttributeName = "levelMin")]
    public int LevelMin { get; set; }

    /// <summary>Level range maximum.</summary>
    [XmlAttribute(AttributeName = "levelMax")]
    public int LevelMax { get; set; }
}
