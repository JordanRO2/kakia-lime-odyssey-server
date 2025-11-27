/// <summary>
/// Loader and cache for server error messages XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Server/ServerErrorInfo.xml
/// Contains localized error messages for skill/item usage failures.
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for server error info XML file.
/// </summary>
[XmlRoot(ElementName = "ExceptionMessage")]
public class ServerErrorInfo
{
    /// <summary>List of error messages.</summary>
    [XmlElement(ElementName = "Message")]
    public List<ServerErrorMessage> Messages { get; set; } = new();

    private static ServerErrorInfo? _instance;
    private static Dictionary<int, ServerErrorMessage>? _idCache;
    private static Dictionary<string, ServerErrorMessage>? _nameCache;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>ServerErrorInfo instance.</returns>
    public static ServerErrorInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(ServerErrorInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Server.ServerErrorInfo, FileMode.Open);
        _instance = (ServerErrorInfo)serializer.Deserialize(fileStream)!;

        _idCache = new Dictionary<int, ServerErrorMessage>();
        _nameCache = new Dictionary<string, ServerErrorMessage>(StringComparer.OrdinalIgnoreCase);

        foreach (var msg in _instance.Messages)
        {
            _idCache[msg.Id] = msg;
            if (!string.IsNullOrEmpty(msg.Name))
                _nameCache[msg.Name] = msg;
        }

        return _instance;
    }

    /// <summary>
    /// Gets an error message by ID.
    /// </summary>
    /// <param name="id">Message ID.</param>
    /// <returns>ServerErrorMessage or null if not found.</returns>
    public static ServerErrorMessage? GetMessageById(int id)
    {
        GetInstance();
        return _idCache!.TryGetValue(id, out var msg) ? msg : null;
    }

    /// <summary>
    /// Gets an error message by name.
    /// </summary>
    /// <param name="name">Message name constant.</param>
    /// <returns>ServerErrorMessage or null if not found.</returns>
    public static ServerErrorMessage? GetMessageByName(string name)
    {
        GetInstance();
        return _nameCache!.TryGetValue(name, out var msg) ? msg : null;
    }

    /// <summary>
    /// Gets the text for an error by ID.
    /// </summary>
    /// <param name="id">Message ID.</param>
    /// <returns>Error text or empty string if not found.</returns>
    public static string GetErrorText(int id)
    {
        var msg = GetMessageById(id);
        return msg?.Text ?? string.Empty;
    }

    /// <summary>
    /// Gets the text for an error by name.
    /// </summary>
    /// <param name="name">Message name constant.</param>
    /// <returns>Error text or empty string if not found.</returns>
    public static string GetErrorText(string name)
    {
        var msg = GetMessageByName(name);
        return msg?.Text ?? string.Empty;
    }
}

/// <summary>
/// Server error message definition.
/// </summary>
public class ServerErrorMessage
{
    /// <summary>Message ID.</summary>
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    /// <summary>Message name constant.</summary>
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Localized message text.</summary>
    [XmlAttribute(AttributeName = "text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>Message type.</summary>
    [XmlAttribute(AttributeName = "type")]
    public int Type { get; set; }
}

/// <summary>
/// Common server error IDs.
/// </summary>
public static class ServerErrorId
{
    /// <summary>Skill cooldown not finished.</summary>
    public const int CantUseSkillCoolTimeRemains = 1;

    /// <summary>Not enough HP to use skill.</summary>
    public const int CantUseSkillNotEnoughHp = 2;

    /// <summary>Not enough SP to use skill.</summary>
    public const int CantUseSkillNotEnoughSp = 3;

    /// <summary>Not enough MP to use skill.</summary>
    public const int CantUseSkillNotEnoughMp = 4;

    /// <summary>Not enough LP to use skill.</summary>
    public const int CantUseSkillNotEnoughLp = 5;

    /// <summary>Cannot use skill while riding.</summary>
    public const int CantUseSkillRiding = 6;

    /// <summary>No target selected.</summary>
    public const int CantUseSkillNoTarget = 7;

    /// <summary>Target too far away.</summary>
    public const int CantUseSkillTooFar = 8;

    /// <summary>Invalid target.</summary>
    public const int CantUseSkillTargetInvalid = 9;

    /// <summary>Target is dead.</summary>
    public const int CantUseSkillTargetDead = 11;

    /// <summary>Item cooldown not finished.</summary>
    public const int CantUseItemCoolTimeRemains = 14;

    /// <summary>No combat job selected.</summary>
    public const int NoCombatJobSelected = 23;

    /// <summary>Cannot combat while in life job mode.</summary>
    public const int CantCombatInLifeJobState = 24;

    /// <summary>Cannot gather in combat job mode.</summary>
    public const int CantGatherInCombatJobState = 25;

    /// <summary>No target available.</summary>
    public const int NoTarget = 28;

    /// <summary>Item not found.</summary>
    public const int NoItem = 29;

    /// <summary>Not enough money.</summary>
    public const int NotEnoughMoney = 30;

    /// <summary>Cannot abandon this quest.</summary>
    public const int CantAbandonQuest = 32;

    /// <summary>Cannot perform action in combat state.</summary>
    public const int CantDoInCombatState = 34;

    /// <summary>Life job level too low.</summary>
    public const int LifeJobLevelTooLow = 35;
}
