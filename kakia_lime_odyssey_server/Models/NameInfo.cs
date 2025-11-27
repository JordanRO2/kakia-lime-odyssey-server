/// <summary>
/// Loader and cache for name display information XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Misc/NameInfo.xml
/// Contains name display settings for different races and relations.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for name info XML file.
/// </summary>
[XmlRoot(ElementName = "NameInfo")]
public class NameInfoRoot
{
    /// <summary>PC name display settings.</summary>
    [XmlElement(ElementName = "Pc")]
    public PcNameSection Pc { get; set; } = new();

    private static NameInfoRoot? _instance;
    private static Dictionary<string, NameDisplayInfo>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>NameInfoRoot instance.</returns>
    public static NameInfoRoot GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(NameInfoRoot));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Misc.NameInfo, FileMode.Open);
        _instance = (NameInfoRoot)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<string, NameDisplayInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in _instance.Pc.Names)
        {
            _cache[name.Type] = name;
        }

        return _instance;
    }

    /// <summary>
    /// Gets name display info for a race type.
    /// </summary>
    /// <param name="raceType">Race type string (e.g., "HUMAN_MALE").</param>
    /// <returns>NameDisplayInfo or null if not found.</returns>
    public static NameDisplayInfo? GetNameInfo(string raceType)
    {
        GetInstance();
        return _cache!.TryGetValue(raceType, out var info) ? info : null;
    }

    /// <summary>
    /// Gets name display settings for a specific race and relation.
    /// </summary>
    /// <param name="raceType">Race type string.</param>
    /// <param name="relationType">Relation type string.</param>
    /// <returns>PcNameStyle or null if not found.</returns>
    public static PcNameStyle? GetNameStyle(string raceType, string relationType)
    {
        var info = GetNameInfo(raceType);
        if (info == null) return null;

        return info.Styles.FirstOrDefault(s =>
            s.Type.Equals(relationType, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets name display info by race ID.
    /// </summary>
    /// <param name="raceId">Race ID (0-5).</param>
    /// <returns>NameDisplayInfo or null if not found.</returns>
    public static NameDisplayInfo? GetNameInfoByRaceId(int raceId)
    {
        string raceType = raceId switch
        {
            0 => "PAM_MALE",
            1 => "PAM_FEMALE",
            2 => "HUMAN_MALE",
            3 => "HUMAN_FEMALE",
            4 => "TURGA_MALE",
            5 => "TURGA_FEMALE",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(raceType)) return null;
        return GetNameInfo(raceType);
    }
}

/// <summary>
/// PC name display section.
/// </summary>
public class PcNameSection
{
    /// <summary>Name display entries.</summary>
    [XmlElement(ElementName = "Name")]
    public List<NameDisplayInfo> Names { get; set; } = new();
}

/// <summary>
/// Name display information for a race.
/// </summary>
public class NameDisplayInfo
{
    /// <summary>Race type identifier (e.g., "HUMAN_MALE").</summary>
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Name display height offset.</summary>
    [XmlAttribute(AttributeName = "height")]
    public float Height { get; set; }

    /// <summary>Name display scale.</summary>
    [XmlAttribute(AttributeName = "scale")]
    public float Scale { get; set; }

    /// <summary>Relation-specific display styles.</summary>
    [XmlElement(ElementName = "PcName")]
    public List<PcNameStyle> Styles { get; set; } = new();
}

/// <summary>
/// PC name display style for a specific relation.
/// </summary>
public class PcNameStyle
{
    /// <summary>Relation type (e.g., "RACE_RELATION_NORMAL").</summary>
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Text color (RGB space-separated).</summary>
    [XmlAttribute(AttributeName = "textColor")]
    public string TextColor { get; set; } = string.Empty;

    /// <summary>Font name.</summary>
    [XmlAttribute(AttributeName = "fontname")]
    public string FontName { get; set; } = string.Empty;

    /// <summary>Font height.</summary>
    [XmlAttribute(AttributeName = "fontHeight")]
    public int FontHeight { get; set; }

    /// <summary>Font weight.</summary>
    [XmlAttribute(AttributeName = "fontWeight")]
    public int FontWeight { get; set; }

    /// <summary>Name display size.</summary>
    [XmlAttribute(AttributeName = "nameSize")]
    public float NameSize { get; set; }

    /// <summary>
    /// Parses the text color into RGB values.
    /// </summary>
    /// <returns>Tuple of (R, G, B) values.</returns>
    public (int R, int G, int B) GetColorRGB()
    {
        var parts = TextColor.Split(' ');
        if (parts.Length < 3) return (255, 255, 255);

        int.TryParse(parts[0], out int r);
        int.TryParse(parts[1], out int g);
        int.TryParse(parts[2], out int b);

        return (r, g, b);
    }
}

/// <summary>
/// Race relation types.
/// </summary>
public static class RaceRelationType
{
    /// <summary>Very bad relation (enemy).</summary>
    public const string VeryBad = "RACE_RELATION_VERY_BAD";

    /// <summary>Bad relation.</summary>
    public const string Bad = "RACE_RELATION_BAD";

    /// <summary>Normal relation.</summary>
    public const string Normal = "RACE_RELATION_NORMAL";

    /// <summary>Good relation.</summary>
    public const string Good = "RACE_RELATION_GOOD";

    /// <summary>Very good relation (ally).</summary>
    public const string VeryGood = "RACE_RELATION_VERY_GOOD";

    /// <summary>No relation.</summary>
    public const string None = "RACE_RELATION_NONE";
}
