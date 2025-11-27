/// <summary>
/// Loader and cache for job class definition XML files.
/// </summary>
/// <remarks>
/// Loads job class data from: GameData/Definitions/Characters/PcJobClassInfo.xml
/// Contains LifeJob, CombatJob, and TribeClass definitions.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for job class XML file.
/// </summary>
[XmlRoot(ElementName = "Job")]
public class PcJobClassInfo
{
    /// <summary>List of life job definitions.</summary>
    [XmlElement(ElementName = "LifeJob")]
    public List<LifeJobInfo> LifeJobs { get; set; } = new();

    /// <summary>List of combat job definitions.</summary>
    [XmlElement(ElementName = "ComBatJob")]
    public List<CombatJobInfo> CombatJobs { get; set; } = new();

    /// <summary>List of tribe/race class definitions.</summary>
    [XmlElement(ElementName = "TribeClass")]
    public List<TribeClassInfo> TribeClasses { get; set; } = new();

    private static PcJobClassInfo? _instance;
    private static Dictionary<int, LifeJobInfo>? _lifeJobCache;
    private static Dictionary<int, CombatJobInfo>? _combatJobCache;
    private static Dictionary<int, TribeClassInfo>? _tribeClassCache;

    /// <summary>
    /// Gets the singleton instance of job class info.
    /// </summary>
    /// <returns>PcJobClassInfo instance.</returns>
    public static PcJobClassInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(PcJobClassInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Characters.PcJobClassInfo, FileMode.Open);
        _instance = (PcJobClassInfo)serializer.Deserialize(fileStream)!;

        _lifeJobCache = new Dictionary<int, LifeJobInfo>();
        foreach (var job in _instance.LifeJobs)
            _lifeJobCache[job.TypeId] = job;

        _combatJobCache = new Dictionary<int, CombatJobInfo>();
        foreach (var job in _instance.CombatJobs)
            _combatJobCache[job.TypeId] = job;

        _tribeClassCache = new Dictionary<int, TribeClassInfo>();
        foreach (var tribe in _instance.TribeClasses)
            _tribeClassCache[tribe.TypeId] = tribe;

        return _instance;
    }

    /// <summary>
    /// Gets a life job by its type ID.
    /// </summary>
    /// <param name="typeId">Life job type ID.</param>
    /// <returns>Life job info or null if not found.</returns>
    public static LifeJobInfo? GetLifeJob(int typeId)
    {
        GetInstance();
        return _lifeJobCache!.TryGetValue(typeId, out var job) ? job : null;
    }

    /// <summary>
    /// Gets a combat job by its type ID.
    /// </summary>
    /// <param name="typeId">Combat job type ID.</param>
    /// <returns>Combat job info or null if not found.</returns>
    public static CombatJobInfo? GetCombatJob(int typeId)
    {
        GetInstance();
        return _combatJobCache!.TryGetValue(typeId, out var job) ? job : null;
    }

    /// <summary>
    /// Gets a tribe class by its type ID.
    /// </summary>
    /// <param name="typeId">Tribe class type ID.</param>
    /// <returns>Tribe class info or null if not found.</returns>
    public static TribeClassInfo? GetTribeClass(int typeId)
    {
        GetInstance();
        return _tribeClassCache!.TryGetValue(typeId, out var tribe) ? tribe : null;
    }

    /// <summary>
    /// Gets all life jobs.
    /// </summary>
    /// <returns>List of all life jobs.</returns>
    public static List<LifeJobInfo> GetAllLifeJobs()
    {
        return GetInstance().LifeJobs;
    }

    /// <summary>
    /// Gets all combat jobs.
    /// </summary>
    /// <returns>List of all combat jobs.</returns>
    public static List<CombatJobInfo> GetAllCombatJobs()
    {
        return GetInstance().CombatJobs;
    }

    /// <summary>
    /// Gets all tribe classes.
    /// </summary>
    /// <returns>List of all tribe classes.</returns>
    public static List<TribeClassInfo> GetAllTribeClasses()
    {
        return GetInstance().TribeClasses;
    }
}

/// <summary>
/// Life job definition.
/// </summary>
public class LifeJobInfo
{
    /// <summary>Job type ID.</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Job name.</summary>
    [XmlAttribute(AttributeName = "typeName")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>Job description.</summary>
    [XmlAttribute(AttributeName = "exp")]
    public string Description { get; set; } = string.Empty;

    /// <summary>PC info image path.</summary>
    [XmlAttribute(AttributeName = "pcInfoImageName")]
    public string PcInfoImageName { get; set; } = string.Empty;

    /// <summary>Status image path.</summary>
    [XmlAttribute(AttributeName = "statusImageName")]
    public string StatusImageName { get; set; } = string.Empty;

    /// <summary>Gauge type.</summary>
    [XmlAttribute(AttributeName = "gageType")]
    public int GageType { get; set; }

    /// <summary>Job cooldown time.</summary>
    [XmlAttribute(AttributeName = "jobCoolTime")]
    public int JobCoolTime { get; set; }

    /// <summary>Base skill 1 ID.</summary>
    [XmlAttribute(AttributeName = "baseSkill1")]
    public int BaseSkill1 { get; set; }

    /// <summary>Base skill 2 ID.</summary>
    [XmlAttribute(AttributeName = "baseSkill2")]
    public int BaseSkill2 { get; set; }
}

/// <summary>
/// Combat job definition.
/// </summary>
public class CombatJobInfo
{
    /// <summary>Job type ID.</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Job name.</summary>
    [XmlAttribute(AttributeName = "typeName")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>Job description.</summary>
    [XmlAttribute(AttributeName = "exp")]
    public string Description { get; set; } = string.Empty;

    /// <summary>PC info image path.</summary>
    [XmlAttribute(AttributeName = "pcInfoImageName")]
    public string PcInfoImageName { get; set; } = string.Empty;

    /// <summary>Status image path.</summary>
    [XmlAttribute(AttributeName = "statusImageName")]
    public string StatusImageName { get; set; } = string.Empty;

    /// <summary>Gauge type (1=MP, 2=SP).</summary>
    [XmlAttribute(AttributeName = "gageType")]
    public int GageType { get; set; }

    /// <summary>Job cooldown time in milliseconds.</summary>
    [XmlAttribute(AttributeName = "jobCoolTime")]
    public int JobCoolTime { get; set; }

    /// <summary>Base skill 1 ID.</summary>
    [XmlAttribute(AttributeName = "baseSkill1")]
    public int BaseSkill1 { get; set; }

    /// <summary>Base skill 2 ID.</summary>
    [XmlAttribute(AttributeName = "baseSkill2")]
    public int BaseSkill2 { get; set; }
}

/// <summary>
/// Tribe/Race class definition.
/// </summary>
public class TribeClassInfo
{
    /// <summary>Tribe type ID.</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Tribe name.</summary>
    [XmlAttribute(AttributeName = "typeName")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>Tribe description.</summary>
    [XmlAttribute(AttributeName = "exp")]
    public string Description { get; set; } = string.Empty;

    /// <summary>PC info image path.</summary>
    [XmlAttribute(AttributeName = "pcInfoImageName")]
    public string PcInfoImageName { get; set; } = string.Empty;

    /// <summary>Status image path.</summary>
    [XmlAttribute(AttributeName = "statusImageName")]
    public string StatusImageName { get; set; } = string.Empty;

    /// <summary>Gauge type.</summary>
    [XmlAttribute(AttributeName = "gageType")]
    public int GageType { get; set; }

    /// <summary>Job cooldown time.</summary>
    [XmlAttribute(AttributeName = "jobCoolTime")]
    public int JobCoolTime { get; set; }

    /// <summary>Base skill 1 ID.</summary>
    [XmlAttribute(AttributeName = "baseSkill1")]
    public int BaseSkill1 { get; set; }

    /// <summary>Base skill 2 ID.</summary>
    [XmlAttribute(AttributeName = "baseSkill2")]
    public int BaseSkill2 { get; set; }
}
