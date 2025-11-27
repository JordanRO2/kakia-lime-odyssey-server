/// <summary>
/// Loader and cache for quest icons XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Misc/QuestIcons.xml
/// Contains quest type icons and difficulty level icons.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for quest icons XML file.
/// </summary>
[XmlRoot(ElementName = "QuestInfo")]
public class QuestIconsInfo
{
    /// <summary>Quest icon definitions.</summary>
    [XmlElement(ElementName = "QuestIcon")]
    public QuestIconGroup QuestIcons { get; set; } = new();

    /// <summary>Level icon definitions.</summary>
    [XmlElement(ElementName = "LevelList")]
    public LevelIconGroup LevelIcons { get; set; } = new();

    private static QuestIconsInfo? _instance;
    private static Dictionary<int, QuestIconEntry>? _questIconCache;
    private static Dictionary<int, LevelIconEntry>? _levelIconCache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>QuestIconsInfo instance.</returns>
    public static QuestIconsInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(QuestIconsInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Misc.QuestIcons, FileMode.Open);
        _instance = (QuestIconsInfo)serializer.Deserialize(fileStream)!;

        _questIconCache = new Dictionary<int, QuestIconEntry>();
        foreach (var icon in _instance.QuestIcons.Groups)
            _questIconCache[icon.TypeId] = icon;

        _levelIconCache = new Dictionary<int, LevelIconEntry>();
        foreach (var level in _instance.LevelIcons.Levels)
            _levelIconCache[level.TypeId] = level;

        return _instance;
    }

    /// <summary>
    /// Gets a quest icon by type ID.
    /// </summary>
    /// <param name="typeId">Quest type ID.</param>
    /// <returns>QuestIconEntry or null if not found.</returns>
    public static QuestIconEntry? GetQuestIcon(int typeId)
    {
        GetInstance();
        return _questIconCache!.TryGetValue(typeId, out var icon) ? icon : null;
    }

    /// <summary>
    /// Gets a level icon by level.
    /// </summary>
    /// <param name="level">Difficulty level (0-5).</param>
    /// <returns>LevelIconEntry or null if not found.</returns>
    public static LevelIconEntry? GetLevelIcon(int level)
    {
        GetInstance();
        return _levelIconCache!.TryGetValue(level, out var icon) ? icon : null;
    }

    /// <summary>
    /// Gets the image path for a quest type.
    /// </summary>
    /// <param name="typeId">Quest type ID.</param>
    /// <returns>Image path or empty string if not found.</returns>
    public static string GetQuestIconImage(int typeId)
    {
        var icon = GetQuestIcon(typeId);
        return icon?.ImageName ?? string.Empty;
    }

    /// <summary>
    /// Gets the image path for a difficulty level.
    /// </summary>
    /// <param name="level">Difficulty level.</param>
    /// <returns>Image path or empty string if not found.</returns>
    public static string GetLevelIconImage(int level)
    {
        var icon = GetLevelIcon(level);
        return icon?.ImageName ?? string.Empty;
    }
}

/// <summary>
/// Quest icon group container.
/// </summary>
public class QuestIconGroup
{
    /// <summary>List of quest icon entries.</summary>
    [XmlElement(ElementName = "Group")]
    public List<QuestIconEntry> Groups { get; set; } = new();
}

/// <summary>
/// Level icon group container.
/// </summary>
public class LevelIconGroup
{
    /// <summary>List of level icon entries.</summary>
    [XmlElement(ElementName = "Level")]
    public List<LevelIconEntry> Levels { get; set; } = new();
}

/// <summary>
/// Quest icon entry.
/// </summary>
public class QuestIconEntry
{
    /// <summary>Quest type ID.</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Icon image path.</summary>
    [XmlAttribute(AttributeName = "imageName")]
    public string ImageName { get; set; } = string.Empty;
}

/// <summary>
/// Level icon entry.
/// </summary>
public class LevelIconEntry
{
    /// <summary>Level type ID (0-5).</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Icon image path.</summary>
    [XmlAttribute(AttributeName = "imageName")]
    public string ImageName { get; set; } = string.Empty;
}

/// <summary>
/// Quest type group IDs.
/// </summary>
public static class QuestTypeGroup
{
    /// <summary>History/Story quests.</summary>
    public const int History = 0;

    /// <summary>Race-specific quests.</summary>
    public const int Race = 1;

    /// <summary>Camp/Faction quests.</summary>
    public const int Camp = 2;

    /// <summary>Zone quests.</summary>
    public const int Zone = 100;

    /// <summary>Job quests.</summary>
    public const int Job = 101;

    /// <summary>Legend quests.</summary>
    public const int Legend = 102;

    /// <summary>Hunting quests.</summary>
    public const int Hunting = 200;

    /// <summary>Gathering quests.</summary>
    public const int Gather = 201;

    /// <summary>Delivery quests.</summary>
    public const int Delivery = 202;

    /// <summary>Default quests.</summary>
    public const int Default = 203;

    /// <summary>Guard/Protection quests.</summary>
    public const int Guard = 204;

    /// <summary>Finding/Search quests.</summary>
    public const int Finding = 205;

    /// <summary>Quiz quests.</summary>
    public const int Quiz = 206;

    /// <summary>Crafting quests.</summary>
    public const int Making = 207;
}
