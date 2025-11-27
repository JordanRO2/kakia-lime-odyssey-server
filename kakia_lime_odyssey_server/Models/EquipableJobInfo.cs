/// <summary>
/// Loader and cache for equipable job restrictions XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Items/equipableJob.xml
/// Defines which jobs can equip which items.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for equipable job XML file.
/// </summary>
[XmlRoot(ElementName = "Root")]
public class EquipableJobInfo
{
    /// <summary>List of job equipment restrictions.</summary>
    [XmlElement(ElementName = "Type")]
    public List<EquipableJobType> Types { get; set; } = new();

    private static EquipableJobInfo? _instance;
    private static Dictionary<int, EquipableJobType>? _cache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>EquipableJobInfo instance.</returns>
    public static EquipableJobInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(EquipableJobInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Items.EquipableJob, FileMode.Open);
        _instance = (EquipableJobInfo)serializer.Deserialize(fileStream)!;

        _cache = new Dictionary<int, EquipableJobType>();
        foreach (var type in _instance.Types)
            _cache[type.TypeId] = type;

        return _instance;
    }

    /// <summary>
    /// Gets the equipable job type by ID.
    /// </summary>
    /// <param name="typeId">Type ID.</param>
    /// <returns>EquipableJobType or null if not found.</returns>
    public static EquipableJobType? GetType(int typeId)
    {
        GetInstance();
        return _cache!.TryGetValue(typeId, out var type) ? type : null;
    }

    /// <summary>
    /// Checks if a job can equip an item with the given job restriction type.
    /// </summary>
    /// <param name="itemJobTypeId">Item's job type restriction ID.</param>
    /// <param name="jobId">Job ID to check.</param>
    /// <returns>True if the job can equip the item.</returns>
    public static bool CanJobEquip(int itemJobTypeId, int jobId)
    {
        var type = GetType(itemJobTypeId);
        if (type == null) return false;

        // Type 0 means all jobs can equip
        if (itemJobTypeId == 0) return true;

        return type.AllowedJobs.Any(j => j.TypeId == jobId);
    }
}

/// <summary>
/// Equipment job restriction type.
/// </summary>
public class EquipableJobType
{
    /// <summary>Type ID.</summary>
    [XmlAttribute(AttributeName = "typeID")]
    public int TypeId { get; set; }

    /// <summary>Type name/description.</summary>
    [XmlAttribute(AttributeName = "typeName")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>List of allowed jobs.</summary>
    [XmlElement(ElementName = "Job")]
    public List<AllowedJob> AllowedJobs { get; set; } = new();
}

/// <summary>
/// Allowed job reference.
/// </summary>
public class AllowedJob
{
    /// <summary>Job type ID.</summary>
    [XmlAttribute(AttributeName = "typeID")]
    public int TypeId { get; set; }
}
