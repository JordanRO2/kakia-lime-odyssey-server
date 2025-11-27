/// <summary>
/// Loader and cache for forbidden words (chat filter) XML file.
/// </summary>
/// <remarks>
/// Loads data from: GameData/Definitions/Server/ForbiddenWords.xml
/// Contains words that should be filtered in chat.
/// </remarks>
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for forbidden words XML file.
/// </summary>
[XmlRoot(ElementName = "Forbiden")]
public class ForbiddenWordsInfo
{
    /// <summary>Slang filter section.</summary>
    [XmlElement(ElementName = "Slang")]
    public SlangFilter Slang { get; set; } = new();

    private static ForbiddenWordsInfo? _instance;
    private static HashSet<string>? _englishWords;
    private static List<KoreanForbiddenWord>? _koreanWords;
    private static HashSet<string>? _prefixes;
    private static HashSet<string>? _suffixes;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <returns>ForbiddenWordsInfo instance.</returns>
    public static ForbiddenWordsInfo GetInstance()
    {
        if (_instance != null) return _instance;

        XmlSerializer serializer = new XmlSerializer(typeof(ForbiddenWordsInfo));
        using FileStream fileStream = new FileStream(GameDataPaths.Definitions.Server.ForbiddenWords, FileMode.Open);
        _instance = (ForbiddenWordsInfo)serializer.Deserialize(fileStream)!;

        // Build caches
        _englishWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var word in _instance.Slang.EnglishWords)
            _englishWords.Add(word.Word);

        _koreanWords = _instance.Slang.KoreanWords;

        _prefixes = new HashSet<string>();
        foreach (var prefix in _instance.Slang.Prefixes)
            if (!string.IsNullOrEmpty(prefix.Korea))
                _prefixes.Add(prefix.Korea);

        _suffixes = new HashSet<string>();
        foreach (var suffix in _instance.Slang.Suffixes)
            if (!string.IsNullOrEmpty(suffix.Korea))
                _suffixes.Add(suffix.Korea);

        return _instance;
    }

    /// <summary>
    /// Filters a message, replacing forbidden words with asterisks.
    /// </summary>
    /// <param name="message">Message to filter.</param>
    /// <returns>Filtered message.</returns>
    public static string FilterMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return message;

        GetInstance();
        string filtered = message;

        // Filter English words
        foreach (var word in _englishWords!)
        {
            filtered = Regex.Replace(filtered, $@"\b{Regex.Escape(word)}\b",
                match => new string('*', match.Length),
                RegexOptions.IgnoreCase);
        }

        // Filter Korean words
        foreach (var word in _koreanWords!)
        {
            if (string.IsNullOrEmpty(word.Word)) continue;
            filtered = filtered.Replace(word.Word, word.Convert ?? "***");
        }

        return filtered;
    }

    /// <summary>
    /// Checks if a message contains forbidden words.
    /// </summary>
    /// <param name="message">Message to check.</param>
    /// <returns>True if message contains forbidden words.</returns>
    public static bool ContainsForbiddenWords(string message)
    {
        if (string.IsNullOrEmpty(message)) return false;

        GetInstance();

        // Check English words
        foreach (var word in _englishWords!)
        {
            if (Regex.IsMatch(message, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase))
                return true;
        }

        // Check Korean words
        foreach (var word in _koreanWords!)
        {
            if (!string.IsNullOrEmpty(word.Word) && message.Contains(word.Word))
                return true;
        }

        return false;
    }
}

/// <summary>
/// Slang filter section.
/// </summary>
public class SlangFilter
{
    /// <summary>Forbidden prefix list.</summary>
    [XmlElement(ElementName = "Prefix")]
    public List<ForbiddenPrefix> Prefixes { get; set; } = new();

    /// <summary>Forbidden suffix list.</summary>
    [XmlElement(ElementName = "Suffix")]
    public List<ForbiddenSuffix> Suffixes { get; set; } = new();

    /// <summary>English forbidden words.</summary>
    [XmlElement(ElementName = "English")]
    public List<EnglishForbiddenWord> EnglishWords { get; set; } = new();

    /// <summary>Korean forbidden words.</summary>
    [XmlElement(ElementName = "Korea")]
    public List<KoreanForbiddenWord> KoreanWords { get; set; } = new();
}

/// <summary>
/// Forbidden prefix.
/// </summary>
public class ForbiddenPrefix
{
    /// <summary>Korean prefix.</summary>
    [XmlAttribute(AttributeName = "Korea")]
    public string Korea { get; set; } = string.Empty;

    /// <summary>Replacement text.</summary>
    [XmlAttribute(AttributeName = "Convert")]
    public string Convert { get; set; } = string.Empty;
}

/// <summary>
/// Forbidden suffix.
/// </summary>
public class ForbiddenSuffix
{
    /// <summary>Korean suffix.</summary>
    [XmlAttribute(AttributeName = "Korea")]
    public string Korea { get; set; } = string.Empty;

    /// <summary>Replacement text.</summary>
    [XmlAttribute(AttributeName = "Convert")]
    public string Convert { get; set; } = string.Empty;
}

/// <summary>
/// English forbidden word.
/// </summary>
public class EnglishForbiddenWord
{
    /// <summary>English word.</summary>
    [XmlAttribute(AttributeName = "English")]
    public string Word { get; set; } = string.Empty;

    /// <summary>Replacement text.</summary>
    [XmlAttribute(AttributeName = "Convert")]
    public string Convert { get; set; } = "***";
}

/// <summary>
/// Korean forbidden word.
/// </summary>
public class KoreanForbiddenWord
{
    /// <summary>Permitted prefix.</summary>
    [XmlAttribute(AttributeName = "PermitPre")]
    public string PermitPre { get; set; } = string.Empty;

    /// <summary>Forbidden word.</summary>
    [XmlAttribute(AttributeName = "Word")]
    public string Word { get; set; } = string.Empty;

    /// <summary>Permitted suffix.</summary>
    [XmlAttribute(AttributeName = "PermitSuf")]
    public string PermitSuf { get; set; } = string.Empty;

    /// <summary>Replacement text.</summary>
    [XmlAttribute(AttributeName = "Convert")]
    public string Convert { get; set; } = "***";
}
