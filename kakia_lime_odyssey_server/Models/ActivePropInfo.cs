/// <summary>
/// Loader and cache for active prop (gatherable objects) XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Props/ActivePropInfo.xml
/// Defines gatherable props like trees, minerals, etc.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for active prop XML file.
/// </summary>
[XmlRoot(ElementName = "ActivePropInfo")]
public class ActivePropInfo
{
    /// <summary>List of active prop definitions.</summary>
    [XmlElement(ElementName = "Prop")]
    public List<ActiveProp> Props { get; set; } = new();

    private static ActivePropInfo? _instance;
    private static Dictionary<int, ActiveProp>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>ActivePropInfo instance.</returns>
    public static ActivePropInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(ActivePropInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Props.ActivePropInfo, FileMode.Open);
        _instance = (ActivePropInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, ActiveProp>();
        foreach (var prop in _instance.Props)
            _cache[prop.TypeId] = prop;

        return _instance;
    }

    /// <summary>
    /// Gets a prop by its type ID.
    /// </summary>
    /// <param name="typeId">Prop type ID.</param>
    /// <returns>ActiveProp or null if not found.</returns>
    public static ActiveProp? GetProp(int typeId)
    {
        GetInstance();
        return _cache!.TryGetValue(typeId, out var prop) ? prop : null;
    }

    /// <summary>
    /// Gets all props.
    /// </summary>
    /// <returns>List of all active props.</returns>
    public static List<ActiveProp> GetAllProps()
    {
        return GetInstance().Props;
    }

    /// <summary>
    /// Gets all gatherable props.
    /// </summary>
    /// <returns>List of gatherable props.</returns>
    public static List<ActiveProp> GetGatherableProps()
    {
        return GetInstance().Props.Where(p => p.Gather == 1).ToList();
    }

    /// <summary>
    /// Gets all cuttable props (trees, etc).
    /// </summary>
    /// <returns>List of cuttable props.</returns>
    public static List<ActiveProp> GetCuttableProps()
    {
        return GetInstance().Props.Where(p => p.Cut == 1).ToList();
    }

    /// <summary>
    /// Gets all diggable props (minerals, etc).
    /// </summary>
    /// <returns>List of diggable props.</returns>
    public static List<ActiveProp> GetDiggableProps()
    {
        return GetInstance().Props.Where(p => p.Dig == 1).ToList();
    }

    /// <summary>
    /// Gets all searchable props.
    /// </summary>
    /// <returns>List of searchable props.</returns>
    public static List<ActiveProp> GetSearchableProps()
    {
        return GetInstance().Props.Where(p => p.Search == 1).ToList();
    }
}

/// <summary>
/// Active prop definition.
/// </summary>
public class ActiveProp
{
    /// <summary>Prop type ID.</summary>
    [XmlAttribute(AttributeName = "typeID")]
    public int TypeId { get; set; }

    /// <summary>Prop name.</summary>
    [XmlAttribute(AttributeName = "propName")]
    public string PropName { get; set; } = string.Empty;

    /// <summary>Effect ID when interacting.</summary>
    [XmlAttribute(AttributeName = "effectID")]
    public int EffectId { get; set; }

    /// <summary>Quest effect ID.</summary>
    [XmlAttribute(AttributeName = "questEffectID")]
    public int QuestEffectId { get; set; }

    /// <summary>Can be gathered (1=yes, 0=no).</summary>
    [XmlAttribute(AttributeName = "gather")]
    public int Gather { get; set; }

    /// <summary>Can be cut (1=yes, 0=no).</summary>
    [XmlAttribute(AttributeName = "cut")]
    public int Cut { get; set; }

    /// <summary>Can be dug (1=yes, 0=no).</summary>
    [XmlAttribute(AttributeName = "dig")]
    public int Dig { get; set; }

    /// <summary>Can be searched (1=yes, 0=no).</summary>
    [XmlAttribute(AttributeName = "search")]
    public int Search { get; set; }

    /// <summary>
    /// Returns true if this prop can be gathered.
    /// </summary>
    public bool IsGatherable => Gather == 1;

    /// <summary>
    /// Returns true if this prop can be cut.
    /// </summary>
    public bool IsCuttable => Cut == 1;

    /// <summary>
    /// Returns true if this prop can be dug.
    /// </summary>
    public bool IsDiggable => Dig == 1;

    /// <summary>
    /// Returns true if this prop can be searched.
    /// </summary>
    public bool IsSearchable => Search == 1;
}
