/// <summary>
/// Loader and cache for model type categories XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Entities/ModelsTypeInfo.xml
/// Contains model type category definitions (monsters, plants, objects, etc).
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for models type info XML file.
/// </summary>
[XmlRoot(ElementName = "ModelsTypeInfo")]
public class ModelsTypeInfo
{
    /// <summary>List of model type definitions.</summary>
    [XmlElement(ElementName = "ModelsType")]
    public List<ModelTypeEntry> Types { get; set; } = new();

    private static ModelsTypeInfo? _instance;
    private static Dictionary<int, ModelTypeEntry>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>ModelsTypeInfo instance.</returns>
    public static ModelsTypeInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(ModelsTypeInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Entities.ModelsTypeInfo, FileMode.Open);
        _instance = (ModelsTypeInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, ModelTypeEntry>();
        foreach (var type in _instance.Types)
            _cache[type.TypeId] = type;

        return _instance;
    }

    /// <summary>
    /// Gets a model type by ID.
    /// </summary>
    /// <param name="typeId">Model type ID.</param>
    /// <returns>ModelTypeEntry or null if not found.</returns>
    public static ModelTypeEntry? GetType(int typeId)
    {
        GetInstance();
        return _cache!.TryGetValue(typeId, out var type) ? type : null;
    }

    /// <summary>
    /// Gets all model types.
    /// </summary>
    /// <returns>List of all model type entries.</returns>
    public static List<ModelTypeEntry> GetAllTypes()
    {
        return GetInstance().Types;
    }

    /// <summary>
    /// Gets the name for a model type.
    /// </summary>
    /// <param name="typeId">Model type ID.</param>
    /// <returns>Type name or empty string if not found.</returns>
    public static string GetTypeName(int typeId)
    {
        var type = GetType(typeId);
        return type?.TypeName ?? string.Empty;
    }
}

/// <summary>
/// Model type entry.
/// </summary>
public class ModelTypeEntry
{
    /// <summary>Type ID.</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Type name.</summary>
    [XmlAttribute(AttributeName = "typeName")]
    public string TypeName { get; set; } = string.Empty;
}

/// <summary>
/// Common model type IDs.
/// </summary>
public static class ModelTypeId
{
    /// <summary>Humanoid monsters.</summary>
    public const int Humanoid = 0;

    /// <summary>Animal monsters.</summary>
    public const int Animal = 1;

    /// <summary>Plant monsters.</summary>
    public const int Plant = 2;

    /// <summary>Spirit monsters.</summary>
    public const int Spirit = 3;

    /// <summary>Elemental monsters.</summary>
    public const int Elemental = 4;

    /// <summary>Undead monsters.</summary>
    public const int Undead = 5;

    /// <summary>Demon monsters.</summary>
    public const int Demon = 6;

    /// <summary>Dragon monsters.</summary>
    public const int Dragon = 7;

    /// <summary>Giant monsters.</summary>
    public const int Giant = 8;

    /// <summary>Machine monsters.</summary>
    public const int Machine = 9;

    /// <summary>Fantasy monsters.</summary>
    public const int Fantasy = 10;

    /// <summary>Hybrid monsters.</summary>
    public const int Hybrid = 11;

    /// <summary>Insect monsters.</summary>
    public const int Insect = 12;

    /// <summary>Aquatic monsters.</summary>
    public const int Aquatic = 13;

    /// <summary>Bird monsters.</summary>
    public const int Bird = 14;

    /// <summary>NPCs.</summary>
    public const int Npc = 1000;

    /// <summary>NPC guards.</summary>
    public const int NpcGuard = 1001;

    /// <summary>Grass props.</summary>
    public const int Grass = 1002;

    /// <summary>Tree props.</summary>
    public const int Tree = 1003;

    /// <summary>Rock props.</summary>
    public const int Rock = 1004;

    /// <summary>Mineral props.</summary>
    public const int Mineral = 1005;

    /// <summary>Water objects.</summary>
    public const int Water = 1006;

    /// <summary>Farming/gathering objects.</summary>
    public const int Farming = 1007;

    /// <summary>Building objects.</summary>
    public const int Building = 1008;

    /// <summary>Signpost objects.</summary>
    public const int Signpost = 1009;

    /// <summary>Furniture objects.</summary>
    public const int Furniture = 1010;

    /// <summary>Decoration objects.</summary>
    public const int Decoration = 1011;

    /// <summary>Sea creatures.</summary>
    public const int SeaCreature = 1012;

    /// <summary>Fence objects.</summary>
    public const int Fence = 1013;

    /// <summary>Path objects.</summary>
    public const int Path = 1014;

    /// <summary>Corpse objects.</summary>
    public const int Corpse = 1015;

    /// <summary>Ship objects.</summary>
    public const int Ship = 1016;

    /// <summary>Underground objects.</summary>
    public const int Underground = 1017;

    /// <summary>Interaction objects.</summary>
    public const int Interaction = 1018;

    /// <summary>Special objects.</summary>
    public const int Special = 2000;

    /// <summary>Terrain objects.</summary>
    public const int Terrain = 3000;

    /// <summary>Special NPC.</summary>
    public const int SpecialNpc = 4000;

    /// <summary>Transport NPC.</summary>
    public const int TransportNpc = 5000;
}
