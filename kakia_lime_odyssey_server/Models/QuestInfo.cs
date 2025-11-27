/// <summary>
/// Loader for quest definition XML files.
/// </summary>
/// <remarks>
/// Loads quest data from: GameData/Definitions/Quests/ folder
/// Quest files follow the client format: quest*.xml with Quest root elements
/// </remarks>
using System.Text;
using System.Xml.Serialization;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models.QuestXML;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for quest XML files containing multiple quests.
/// </summary>
[XmlRoot(ElementName = "Quest")]
public class QuestFileRoot
{
	/// <summary>List of quests in this file.</summary>
	[XmlElement(ElementName = "Quest")]
	public List<XmlQuest> Quests { get; set; } = new();
}

/// <summary>
/// Root element for quest list XML file.
/// </summary>
[XmlRoot(ElementName = "Info")]
public class QuestListRoot
{
	/// <summary>List of quest file references.</summary>
	[XmlElement(ElementName = "quest")]
	public List<QuestFileRef> QuestFiles { get; set; } = new();
}

/// <summary>
/// Reference to a quest file in questList.xml.
/// </summary>
public class QuestFileRef
{
	/// <summary>Quest file name.</summary>
	[XmlAttribute(AttributeName = "fileName")]
	public string FileName { get; set; } = string.Empty;
}

/// <summary>
/// Loads and provides access to quest definition data.
/// </summary>
public static class QuestInfo
{
	private static List<XmlQuest>? _cachedQuests;
	private static readonly object _lock = new();

	/// <summary>
	/// Gets all quest definitions from the GameData/Definitions/Quests folder.
	/// </summary>
	/// <returns>List of all quest definitions.</returns>
	public static List<XmlQuest> GetQuests()
	{
		if (_cachedQuests != null)
			return _cachedQuests;

		lock (_lock)
		{
			if (_cachedQuests != null)
				return _cachedQuests;

			_cachedQuests = LoadAllQuests();
			return _cachedQuests;
		}
	}

	/// <summary>
	/// Gets a quest definition by its type ID.
	/// </summary>
	/// <param name="typeId">Quest type ID.</param>
	/// <returns>Quest definition or null if not found.</returns>
	public static XmlQuest? GetQuestById(uint typeId)
	{
		return GetQuests().FirstOrDefault(q => q.TypeID == typeId);
	}

	/// <summary>
	/// Gets all quests for a specific NPC.
	/// </summary>
	/// <param name="npcName">NPC name.</param>
	/// <returns>List of quests the NPC can give.</returns>
	public static List<XmlQuest> GetQuestsByNPC(string npcName)
	{
		return GetQuests().Where(q => q.NPC == npcName).ToList();
	}

	/// <summary>
	/// Gets all quests for a specific level range.
	/// </summary>
	/// <param name="minLevel">Minimum level.</param>
	/// <param name="maxLevel">Maximum level.</param>
	/// <returns>List of quests in level range.</returns>
	public static List<XmlQuest> GetQuestsByLevelRange(int minLevel, int maxLevel)
	{
		return GetQuests().Where(q => q.Level >= minLevel && q.Level <= maxLevel).ToList();
	}

	/// <summary>
	/// Gets all quests by group type.
	/// </summary>
	/// <param name="groupType">Quest group type.</param>
	/// <returns>List of quests in the group.</returns>
	public static List<XmlQuest> GetQuestsByGroup(QuestGroupType groupType)
	{
		return GetQuests().Where(q => q.GetGroupType() == groupType).ToList();
	}

	/// <summary>
	/// Reloads quest data from files (clears cache).
	/// </summary>
	public static void Reload()
	{
		lock (_lock)
		{
			_cachedQuests = null;
			_cachedQuests = LoadAllQuests();
		}
	}

	private static List<XmlQuest> LoadAllQuests()
	{
		var quests = new List<XmlQuest>();
		string questsFolder = GameDataPaths.Definitions.Quests.Folder;

		if (!Directory.Exists(questsFolder))
		{
			Logger.Log($"[QUEST] Quest folder not found: {questsFolder}", LogLevel.Warning);
			return quests;
		}

		// Try to load from questList.xml first
		string questListPath = Path.Combine(questsFolder, "questList.xml");
		if (File.Exists(questListPath))
		{
			quests = LoadFromQuestList(questListPath, questsFolder);
		}
		else
		{
			// Fall back to loading all quest*.xml files directly
			quests = LoadAllQuestFiles(questsFolder);
		}

		Logger.Log($"[QUEST] Loaded {quests.Count} quest definitions", LogLevel.Information);
		return quests;
	}

	private static List<XmlQuest> LoadFromQuestList(string questListPath, string questsFolder)
	{
		var quests = new List<XmlQuest>();

		try
		{
			var serializer = new XmlSerializer(typeof(QuestListRoot));
			using var fileStream = new FileStream(questListPath, FileMode.Open, FileAccess.Read);
			var questList = (QuestListRoot?)serializer.Deserialize(fileStream);

			if (questList?.QuestFiles == null)
				return quests;

			foreach (var questFileRef in questList.QuestFiles)
			{
				string questFilePath = Path.Combine(questsFolder, questFileRef.FileName);
				var fileQuests = LoadQuestFile(questFilePath);
				quests.AddRange(fileQuests);
			}
		}
		catch (Exception ex)
		{
			Logger.Log($"[QUEST] Error loading quest list: {ex.Message}", LogLevel.Error);
		}

		return quests;
	}

	private static List<XmlQuest> LoadAllQuestFiles(string questsFolder)
	{
		var quests = new List<XmlQuest>();

		foreach (var file in Directory.GetFiles(questsFolder, "quest*.xml"))
		{
			var fileQuests = LoadQuestFile(file);
			quests.AddRange(fileQuests);
		}

		return quests;
	}

	private static List<XmlQuest> LoadQuestFile(string filePath)
	{
		var quests = new List<XmlQuest>();

		if (!File.Exists(filePath))
		{
			Logger.Log($"[QUEST] Quest file not found: {filePath}", LogLevel.Warning);
			return quests;
		}

		try
		{
			// Read file with EUC-KR encoding (Korean)
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var eucKr = Encoding.GetEncoding("EUC-KR");

			string xmlContent;
			using (var reader = new StreamReader(filePath, eucKr))
			{
				xmlContent = reader.ReadToEnd();
			}

			var serializer = new XmlSerializer(typeof(QuestFileRoot));
			using var stringReader = new StringReader(xmlContent);
			var questRoot = (QuestFileRoot?)serializer.Deserialize(stringReader);

			if (questRoot?.Quests != null)
			{
				quests.AddRange(questRoot.Quests);
				Logger.Log($"[QUEST] Loaded {questRoot.Quests.Count} quests from {Path.GetFileName(filePath)}", LogLevel.Debug);
			}
		}
		catch (Exception ex)
		{
			Logger.Log($"[QUEST] Error loading quest file {filePath}: {ex.Message}", LogLevel.Error);
		}

		return quests;
	}
}
