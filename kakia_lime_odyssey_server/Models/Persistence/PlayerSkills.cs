namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Persisted learned skill data for a character
/// </summary>
public class PlayerSkills
{
	/// <summary>List of learned skill IDs with their levels</summary>
	public List<LearnedSkill> Skills { get; set; } = new();

	/// <summary>Skill points available for learning new skills</summary>
	public int SkillPoints { get; set; }
}

/// <summary>
/// A single learned skill with its current level
/// </summary>
public class LearnedSkill
{
	/// <summary>Skill ID from SkillInfo.xml</summary>
	public int SkillId { get; set; }

	/// <summary>Current skill level (1-based)</summary>
	public byte Level { get; set; } = 1;

	/// <summary>Current experience towards next level</summary>
	public uint Experience { get; set; }

	/// <summary>Whether the skill is on the hotbar</summary>
	public bool OnHotbar { get; set; }

	/// <summary>Hotbar slot position (if OnHotbar is true)</summary>
	public byte HotbarSlot { get; set; }
}
