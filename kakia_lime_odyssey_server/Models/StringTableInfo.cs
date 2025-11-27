/// <summary>
/// Loader and cache for string table (localization) XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Server/StringTable.xml
/// Contains localized strings for various game elements.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for string table XML file.
/// </summary>
[XmlRoot(ElementName = "Root")]
public class StringTableInfo
{
    /// <summary>Message root container.</summary>
    [XmlElement(ElementName = "MessageRoot")]
    public MessageRoot MessageRoot { get; set; } = new();

    private static StringTableInfo? _instance;
    private static Dictionary<int, StringTableEntry>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>StringTableInfo instance.</returns>
    public static StringTableInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(StringTableInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Server.StringTable, FileMode.Open);
        _instance = (StringTableInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, StringTableEntry>();
        foreach (var msg in _instance.MessageRoot.Messages)
            _cache[msg.Id] = msg;

        return _instance;
    }

    /// <summary>
    /// Gets a string by ID.
    /// </summary>
    /// <param name="id">String ID.</param>
    /// <returns>StringTableEntry or null if not found.</returns>
    public static StringTableEntry? GetEntry(int id)
    {
        GetInstance();
        return _cache!.TryGetValue(id, out var entry) ? entry : null;
    }

    /// <summary>
    /// Gets the text for a string ID.
    /// </summary>
    /// <param name="id">String ID.</param>
    /// <returns>Localized text or empty string if not found.</returns>
    public static string GetString(int id)
    {
        var entry = GetEntry(id);
        return entry?.Text ?? string.Empty;
    }

    /// <summary>
    /// Gets all strings for a description category.
    /// </summary>
    /// <param name="descCategory">Description category (e.g., "BuffInfo", "ActivePropInfo").</param>
    /// <returns>List of matching entries.</returns>
    public static List<StringTableEntry> GetStringsByCategory(string descCategory)
    {
        GetInstance();
        return _cache!.Values
            .Where(e => e.Description.Equals(descCategory, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}

/// <summary>
/// Message root container.
/// </summary>
public class MessageRoot
{
    /// <summary>List of string entries.</summary>
    [XmlElement(ElementName = "Message")]
    public List<StringTableEntry> Messages { get; set; } = new();
}

/// <summary>
/// String table entry.
/// </summary>
public class StringTableEntry
{
    /// <summary>String ID.</summary>
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    /// <summary>Category description.</summary>
    [XmlAttribute(AttributeName = "desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Localized text.</summary>
    [XmlAttribute(AttributeName = "text")]
    public string Text { get; set; } = string.Empty;
}
