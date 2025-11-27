/// <summary>
/// Loader and cache for skill definition XML files.
/// </summary>
/// <remarks>
/// Loads skill data from: GameData/Definitions/Skills/SkillInfo.xml
/// Skills are cached on first load for performance.
/// </remarks>
using System.Text;
using System.Xml.Serialization;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models.SkillXML;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Root element for skill XML file.
/// </summary>
[XmlRoot(ElementName = "SkillInfo")]
public class SkillInfoRoot
{
	/// <summary>List of skills in this file.</summary>
	[XmlElement(ElementName = "Skill")]
	public List<XmlSkill> Skills { get; set; } = new();
}

/// <summary>
/// Loads and provides access to skill definition data.
/// </summary>
public static class SkillInfo
{
	private static List<XmlSkill>? _cachedSkills;
	private static readonly object _lock = new();

	/// <summary>
	/// Gets all skill definitions from the GameData/Definitions/Skills folder.
	/// </summary>
	/// <returns>List of all skill definitions.</returns>
	public static List<XmlSkill> GetSkills()
	{
		if (_cachedSkills != null)
			return _cachedSkills;

		lock (_lock)
		{
			if (_cachedSkills != null)
				return _cachedSkills;

			_cachedSkills = LoadAllSkills();
			return _cachedSkills;
		}
	}

	/// <summary>
	/// Gets a skill definition by its ID.
	/// </summary>
	/// <param name="skillId">Skill ID.</param>
	/// <returns>Skill definition or null if not found.</returns>
	public static XmlSkill? GetSkillById(int skillId)
	{
		return GetSkills().FirstOrDefault(s => s.Id == skillId);
	}

	/// <summary>
	/// Gets all skills for a specific category.
	/// </summary>
	/// <param name="category">Skill category.</param>
	/// <returns>List of skills in the category.</returns>
	public static List<XmlSkill> GetSkillsByCategory(SkillCategory category)
	{
		return GetSkills().Where(s => s.GetCategory() == category).ToList();
	}

	/// <summary>
	/// Gets all combat skills (skills that deal damage).
	/// </summary>
	/// <returns>List of combat skills.</returns>
	public static List<XmlSkill> GetCombatSkills()
	{
		return GetSkills().Where(s => s.IsCombatSkill).ToList();
	}

	/// <summary>
	/// Gets all active skills (not passive/training).
	/// </summary>
	/// <returns>List of active skills.</returns>
	public static List<XmlSkill> GetActiveSkills()
	{
		return GetSkills().Where(s => s.IsActive).ToList();
	}

	/// <summary>
	/// Gets all passive/training skills.
	/// </summary>
	/// <returns>List of passive skills.</returns>
	public static List<XmlSkill> GetPassiveSkills()
	{
		return GetSkills().Where(s => s.IsPassive).ToList();
	}

	/// <summary>
	/// Reloads skill data from files (clears cache).
	/// </summary>
	public static void Reload()
	{
		lock (_lock)
		{
			_cachedSkills = null;
			_cachedSkills = LoadAllSkills();
		}
	}

	private static List<XmlSkill> LoadAllSkills()
	{
		var skills = new List<XmlSkill>();
		string skillsPath = GameDataPaths.Definitions.Skills.SkillInfo;

		if (!File.Exists(skillsPath))
		{
			Logger.Log($"[SKILL] Skills file not found: {skillsPath}", LogLevel.Warning);
			return skills;
		}

		try
		{
			// Read file with EUC-KR encoding (Korean)
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var eucKr = Encoding.GetEncoding("EUC-KR");

			string xmlContent;
			using (var reader = new StreamReader(skillsPath, eucKr))
			{
				xmlContent = reader.ReadToEnd();
			}

			var serializer = new XmlSerializer(typeof(SkillInfoRoot));
			using var stringReader = new StringReader(xmlContent);
			var skillRoot = (SkillInfoRoot?)serializer.Deserialize(stringReader);

			if (skillRoot?.Skills != null)
			{
				skills.AddRange(skillRoot.Skills);
			}

			Logger.Log($"[SKILL] Loaded {skills.Count} skill definitions", LogLevel.Information);
		}
		catch (Exception ex)
		{
			Logger.Log($"[SKILL] Error loading skills file: {ex.Message}", LogLevel.Error);
		}

		return skills;
	}
}