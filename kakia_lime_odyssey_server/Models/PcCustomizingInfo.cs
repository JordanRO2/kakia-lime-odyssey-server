/// <summary>
/// Loader and cache for character customization options XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Characters/PcCustomizingInfo.xml
/// Contains character creation customization options (hair, face, etc).
/// </remarks>
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for character customizing info XML file.
/// </summary>
[XmlRoot(ElementName = "Customizing")]
public class PcCustomizingInfo
{
    /// <summary>Race selection options.</summary>
    [XmlElement(ElementName = "Race")]
    public CustomizingSection Race { get; set; } = new();

    /// <summary>Life job selection options.</summary>
    [XmlElement(ElementName = "LifeJob")]
    public CustomizingSection LifeJob { get; set; } = new();

    /// <summary>Character part customization options.</summary>
    [XmlElement(ElementName = "CustomizingPart")]
    public CustomizingPartSection CustomizingPart { get; set; } = new();

    private static PcCustomizingInfo? _instance;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>PcCustomizingInfo instance.</returns>
    public static PcCustomizingInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(PcCustomizingInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Characters.PcCustomizingInfo, FileMode.Open);
        _instance = (PcCustomizingInfo)serializer.Deserialize(fileStream)!;

        return _instance;
    }

    /// <summary>
    /// Gets race selection options.
    /// </summary>
    /// <returns>List of race selection parts.</returns>
    public static List<CustomizingPart> GetRaceOptions()
    {
        return GetInstance().Race.Parts;
    }

    /// <summary>
    /// Gets life job selection options.
    /// </summary>
    /// <returns>List of life job selection parts.</returns>
    public static List<CustomizingPart> GetLifeJobOptions()
    {
        return GetInstance().LifeJob.Parts;
    }

    /// <summary>
    /// Gets customization options for a specific slot and race.
    /// </summary>
    /// <param name="slotId">Customization slot ID.</param>
    /// <param name="raceId">Race ID.</param>
    /// <returns>List of available customization options.</returns>
    public static List<CustomOption> GetCustomizationOptions(int slotId, int raceId)
    {
        var instance = GetInstance();
        var part = instance.CustomizingPart.Parts.FirstOrDefault(p => p.SlotId == slotId);
        if (part == null) return new List<CustomOption>();

        var raceSelect = part.Selections.FirstOrDefault(s => s.Race == raceId);
        return raceSelect?.Customs ?? new List<CustomOption>();
    }

    /// <summary>
    /// Gets the slot name for a customization slot.
    /// </summary>
    /// <param name="slotId">Slot ID.</param>
    /// <returns>Slot name or empty string if not found.</returns>
    public static string GetSlotName(int slotId)
    {
        var instance = GetInstance();
        var part = instance.CustomizingPart.Parts.FirstOrDefault(p => p.SlotId == slotId);
        return part?.SlotName ?? string.Empty;
    }

    /// <summary>
    /// Validates if a customization option is valid for a race.
    /// </summary>
    /// <param name="slotId">Slot ID.</param>
    /// <param name="raceId">Race ID.</param>
    /// <param name="typeId">Option type ID.</param>
    /// <returns>True if the option is valid.</returns>
    public static bool IsValidCustomization(int slotId, int raceId, int typeId)
    {
        var options = GetCustomizationOptions(slotId, raceId);
        return options.Any(o => o.TypeId == typeId);
    }
}

/// <summary>
/// Customizing section (Race or LifeJob).
/// </summary>
public class CustomizingSection
{
    /// <summary>List of customization parts.</summary>
    [XmlElement(ElementName = "Part")]
    public List<CustomizingPart> Parts { get; set; } = new();
}

/// <summary>
/// Customizing part section for character appearance.
/// </summary>
public class CustomizingPartSection
{
    /// <summary>List of customization parts.</summary>
    [XmlElement(ElementName = "Part")]
    public List<CharacterCustomPart> Parts { get; set; } = new();
}

/// <summary>
/// Basic customization part (race/job selection).
/// </summary>
public class CustomizingPart
{
    /// <summary>Slot ID.</summary>
    [XmlAttribute(AttributeName = "slotId")]
    public int SlotId { get; set; }

    /// <summary>Selection options.</summary>
    [XmlElement(ElementName = "Select")]
    public List<SelectOption> Options { get; set; } = new();
}

/// <summary>
/// Selection option.
/// </summary>
public class SelectOption
{
    /// <summary>Type ID.</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Option name.</summary>
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Image path.</summary>
    [XmlAttribute(AttributeName = "imageName")]
    public string ImageName { get; set; } = string.Empty;
}

/// <summary>
/// Character appearance customization part.
/// </summary>
public class CharacterCustomPart
{
    /// <summary>Slot ID.</summary>
    [XmlAttribute(AttributeName = "slotId")]
    public int SlotId { get; set; }

    /// <summary>Slot name.</summary>
    [XmlAttribute(AttributeName = "slotName")]
    public string SlotName { get; set; } = string.Empty;

    /// <summary>Race-specific selections.</summary>
    [XmlElement(ElementName = "Select")]
    public List<RaceSelection> Selections { get; set; } = new();
}

/// <summary>
/// Race-specific customization selection.
/// </summary>
public class RaceSelection
{
    /// <summary>Race ID.</summary>
    [XmlAttribute(AttributeName = "race")]
    public int Race { get; set; }

    /// <summary>Custom options.</summary>
    [XmlElement(ElementName = "custom")]
    public List<CustomOption> Customs { get; set; } = new();
}

/// <summary>
/// Custom appearance option.
/// </summary>
public class CustomOption
{
    /// <summary>Type ID.</summary>
    [XmlAttribute(AttributeName = "typeId")]
    public int TypeId { get; set; }

    /// <summary>Option name.</summary>
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Image path.</summary>
    [XmlAttribute(AttributeName = "imageName")]
    public string ImageName { get; set; } = string.Empty;
}

/// <summary>
/// Character customization slot IDs.
/// </summary>
public static class CustomSlotId
{
    /// <summary>Hair style.</summary>
    public const int HairStyle = 0;

    /// <summary>Hair decoration.</summary>
    public const int HairDecoration = 1;

    /// <summary>Hair color.</summary>
    public const int HairColor = 2;

    /// <summary>Ears/Horns.</summary>
    public const int Ears = 3;

    /// <summary>Skin color.</summary>
    public const int SkinColor = 4;

    /// <summary>Eye color.</summary>
    public const int EyeColor = 5;

    /// <summary>Face type.</summary>
    public const int FaceType = 6;
}
