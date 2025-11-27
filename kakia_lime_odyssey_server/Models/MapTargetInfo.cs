/// <summary>
/// Loader and cache for map target (warp points) information XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/World/MapTargetInfo.xml
/// Contains map warp/teleport point definitions.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for map target info XML file.
/// </summary>
[XmlRoot(ElementName = "MapTargetInfo")]
public class MapTargetInfo
{
    /// <summary>List of map target entries.</summary>
    [XmlElement(ElementName = "Target")]
    public List<MapTargetEntry> Targets { get; set; } = new();

    private static MapTargetInfo? _instance;
    private static Dictionary<int, MapTargetEntry>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>MapTargetInfo instance.</returns>
    public static MapTargetInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(MapTargetInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.World.MapTargetInfo, FileMode.Open);
        _instance = (MapTargetInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, MapTargetEntry>();
        foreach (var target in _instance.Targets)
            _cache[target.Id] = target;

        return _instance;
    }

    /// <summary>
    /// Gets a map target by ID.
    /// </summary>
    /// <param name="id">Target ID.</param>
    /// <returns>MapTargetEntry or null if not found.</returns>
    public static MapTargetEntry? GetTarget(int id)
    {
        GetInstance();
        return _cache!.TryGetValue(id, out var target) ? target : null;
    }

    /// <summary>
    /// Gets all map targets.
    /// </summary>
    /// <returns>List of all map target entries.</returns>
    public static List<MapTargetEntry> GetAllTargets()
    {
        return GetInstance().Targets;
    }

    /// <summary>
    /// Gets map targets by zone ID.
    /// </summary>
    /// <param name="zoneId">Zone ID.</param>
    /// <returns>List of targets in the zone.</returns>
    public static List<MapTargetEntry> GetTargetsByZone(int zoneId)
    {
        return GetInstance().Targets.Where(t => t.ZoneId == zoneId).ToList();
    }

    /// <summary>
    /// Gets map targets by type.
    /// </summary>
    /// <param name="type">Target type.</param>
    /// <returns>List of targets of the specified type.</returns>
    public static List<MapTargetEntry> GetTargetsByType(int type)
    {
        return GetInstance().Targets.Where(t => t.Type == type).ToList();
    }
}

/// <summary>
/// Map target entry.
/// </summary>
public class MapTargetEntry
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

    /// <summary>X position.</summary>
    [XmlAttribute(AttributeName = "x")]
    public float X { get; set; }

    /// <summary>Y position.</summary>
    [XmlAttribute(AttributeName = "y")]
    public float Y { get; set; }

    /// <summary>Z position.</summary>
    [XmlAttribute(AttributeName = "z")]
    public float Z { get; set; }

    /// <summary>Icon image name.</summary>
    [XmlAttribute(AttributeName = "iconName")]
    public string IconName { get; set; } = string.Empty;

    /// <summary>Description or comment.</summary>
    [XmlAttribute(AttributeName = "desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Name ID for localization.</summary>
    [XmlAttribute(AttributeName = "nameId")]
    public int NameId { get; set; }
}

/// <summary>
/// Map target types.
/// </summary>
public static class MapTargetType
{
    /// <summary>Town/city.</summary>
    public const int Town = 0;

    /// <summary>NPC location.</summary>
    public const int Npc = 1;

    /// <summary>Dungeon entrance.</summary>
    public const int Dungeon = 2;

    /// <summary>Portal/teleporter.</summary>
    public const int Portal = 3;

    /// <summary>Quest location.</summary>
    public const int Quest = 4;

    /// <summary>Shop/vendor.</summary>
    public const int Shop = 5;

    /// <summary>Landmark.</summary>
    public const int Landmark = 6;
}
