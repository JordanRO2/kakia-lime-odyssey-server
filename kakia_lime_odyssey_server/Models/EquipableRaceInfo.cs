/// <summary>
/// Loader and cache for equipable race restrictions XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Items/equipableRace.xml
/// Defines which races can equip which items.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for equipable race XML file.
/// </summary>
[XmlRoot(ElementName = "Root")]
public class EquipableRaceInfo
{
    /// <summary>List of race equipment restrictions.</summary>
    [XmlElement(ElementName = "Type")]
    public List<EquipableRaceType> Types { get; set; } = new();

    private static EquipableRaceInfo? _instance;
    private static Dictionary<int, EquipableRaceType>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>EquipableRaceInfo instance.</returns>
    public static EquipableRaceInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(EquipableRaceInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Items.EquipableRace, FileMode.Open);
        _instance = (EquipableRaceInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, EquipableRaceType>();
        foreach (var type in _instance.Types)
            _cache[type.TypeId] = type;

        return _instance;
    }

    /// <summary>
    /// Gets the equipable race type by ID.
    /// </summary>
    /// <param name="typeId">Type ID.</param>
    /// <returns>EquipableRaceType or null if not found.</returns>
    public static EquipableRaceType? GetType(int typeId)
    {
        GetInstance();
        return _cache!.TryGetValue(typeId, out var type) ? type : null;
    }

    /// <summary>
    /// Checks if a race can equip an item with the given race restriction type.
    /// </summary>
    /// <param name="itemRaceTypeId">Item's race type restriction ID.</param>
    /// <param name="raceId">Race ID to check.</param>
    /// <returns>True if the race can equip the item.</returns>
    public static bool CanRaceEquip(int itemRaceTypeId, int raceId)
    {
        var type = GetType(itemRaceTypeId);
        if (type == null) return false;

        // Type 0 means all races can equip
        if (itemRaceTypeId == 0) return true;

        return type.AllowedRaces.Any(r => r.TypeId == raceId);
    }
}

/// <summary>
/// Equipment race restriction type.
/// </summary>
public class EquipableRaceType
{
    /// <summary>Type ID.</summary>
    [XmlAttribute(AttributeName = "typeID")]
    public int TypeId { get; set; }

    /// <summary>Type name/description.</summary>
    [XmlAttribute(AttributeName = "typeName")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>List of allowed races.</summary>
    [XmlElement(ElementName = "Race")]
    public List<AllowedRace> AllowedRaces { get; set; } = new();
}

/// <summary>
/// Allowed race reference.
/// </summary>
public class AllowedRace
{
    /// <summary>Race type ID.</summary>
    [XmlAttribute(AttributeName = "typeID")]
    public int TypeId { get; set; }
}
