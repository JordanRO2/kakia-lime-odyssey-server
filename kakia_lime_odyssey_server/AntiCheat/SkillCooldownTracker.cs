using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models.SkillXML;

namespace kakia_lime_odyssey_server.AntiCheat;

/// <summary>
/// Server-side skill cooldown tracking system
/// Tracks skill usage and validates cooldowns haven't been violated
/// Based on IDA Pro client analysis requirements (docs/IDA_CLIENT_ANALYSIS.md)
/// </summary>
public class SkillCooldownTracker
{
	/// <summary>
	/// Skill cooldown entry
	/// </summary>
	private class SkillCooldownEntry
	{
		public int SkillId { get; set; }
		public DateTime LastUseTime { get; set; }
		public double CooldownSeconds { get; set; }
		public DateTime NextAvailableTime { get; set; }
	}

	private readonly Dictionary<int, SkillCooldownEntry> _skillCooldowns = new();
	private readonly uint _playerId;
	private readonly string _playerName;

	public SkillCooldownTracker(uint playerId, string playerName)
	{
		_playerId = playerId;
		_playerName = playerName;
	}

	/// <summary>
	/// Check if a skill is ready to use (cooldown has expired)
	/// </summary>
	/// <param name="skillId">Skill ID to check</param>
	/// <returns>True if skill is ready, false if on cooldown</returns>
	public bool IsSkillReady(int skillId)
	{
		if (!_skillCooldowns.TryGetValue(skillId, out var entry))
		{
			// Skill has never been used, it's ready
			return true;
		}

		return DateTime.Now >= entry.NextAvailableTime;
	}

	/// <summary>
	/// Get remaining cooldown time in milliseconds
	/// </summary>
	/// <param name="skillId">Skill ID to check</param>
	/// <returns>Remaining cooldown in milliseconds, or 0 if ready</returns>
	public double GetRemainingCooldown(int skillId)
	{
		if (!_skillCooldowns.TryGetValue(skillId, out var entry))
		{
			return 0;
		}

		var remaining = entry.NextAvailableTime - DateTime.Now;
		return remaining.TotalMilliseconds > 0 ? remaining.TotalMilliseconds : 0;
	}

	/// <summary>
	/// Validate skill cooldown and log violation if detected
	/// </summary>
	/// <param name="skillId">Skill ID being used</param>
	/// <param name="skill">Skill data from SkillXML</param>
	/// <returns>True if skill can be used, false if cooldown violated</returns>
	public bool ValidateAndTrackSkillUse(int skillId, XmlSkill skill)
	{
		// Get cooldown time - prefer SubjectList level 1 cooldown if available
		double cooldownMs = skill.CoolTime;
		var skillLv1 = skill.Subject?.SubjectLists?.FirstOrDefault();
		if (skillLv1 != null && skillLv1.CoolTime > 0)
		{
			cooldownMs = skillLv1.CoolTime;
		}

		// Check if skill is on cooldown
		if (!IsSkillReady(skillId))
		{
			var remaining = GetRemainingCooldown(skillId);

			// Log the violation
			CheatDetection.LogCheat(
				_playerId,
				_playerName,
				CheatDetection.CheatType.InvalidSkillCast,
				$"Skill {skillId} ({skill.NameEng}) used while on cooldown. Remaining: {remaining:F0}ms"
			);

			Logger.Log(
				$"[COOLDOWN VIOLATION] Player {_playerName} ({_playerId}) used skill {skillId} " +
				$"while on cooldown (remaining: {remaining:F0}ms)",
				LogLevel.Warning
			);

			return false;
		}

		// Skill is ready, track this usage
		SetCooldown(skillId, cooldownMs / 1000.0, skill.NameEng);
		return true;
	}

	/// <summary>
	/// Set a cooldown for a skill (called after successful skill use)
	/// </summary>
	/// <param name="skillId">Skill ID</param>
	/// <param name="cooldownSeconds">Cooldown duration in seconds</param>
	/// <param name="skillName">Skill name for logging (optional)</param>
	public void SetCooldown(int skillId, double cooldownSeconds, string skillName = "")
	{
		var now = DateTime.Now;
		var entry = new SkillCooldownEntry
		{
			SkillId = skillId,
			LastUseTime = now,
			CooldownSeconds = cooldownSeconds,
			NextAvailableTime = now.AddSeconds(cooldownSeconds)
		};

		_skillCooldowns[skillId] = entry;

		Logger.Log(
			$"[COOLDOWN] Player {_playerName} used skill {skillId} " +
			$"{(string.IsNullOrEmpty(skillName) ? "" : $"({skillName})")}, " +
			$"cooldown: {cooldownSeconds:F2}s",
			LogLevel.Debug
		);
	}

	/// <summary>
	/// Clear all skill cooldowns (use for testing or special events)
	/// </summary>
	public void ClearAllCooldowns()
	{
		_skillCooldowns.Clear();
		Logger.Log($"[COOLDOWN] Cleared all skill cooldowns for player {_playerName}", LogLevel.Debug);
	}

	/// <summary>
	/// Clear a specific skill cooldown
	/// </summary>
	/// <param name="skillId">Skill ID to clear</param>
	public void ClearCooldown(int skillId)
	{
		if (_skillCooldowns.Remove(skillId))
		{
			Logger.Log($"[COOLDOWN] Cleared cooldown for skill {skillId} for player {_playerName}", LogLevel.Debug);
		}
	}

	/// <summary>
	/// Get all active cooldowns (for debugging)
	/// </summary>
	/// <returns>Dictionary of skill ID to remaining cooldown in milliseconds</returns>
	public Dictionary<int, double> GetActiveCooldowns()
	{
		var activeCooldowns = new Dictionary<int, double>();
		var now = DateTime.Now;

		foreach (var entry in _skillCooldowns.Values)
		{
			var remaining = (entry.NextAvailableTime - now).TotalMilliseconds;
			if (remaining > 0)
			{
				activeCooldowns[entry.SkillId] = remaining;
			}
		}

		return activeCooldowns;
	}

	/// <summary>
	/// Get cooldown info for a specific skill (for debugging)
	/// </summary>
	/// <param name="skillId">Skill ID</param>
	/// <returns>Cooldown info string, or null if not found</returns>
	public string? GetCooldownInfo(int skillId)
	{
		if (!_skillCooldowns.TryGetValue(skillId, out var entry))
		{
			return null;
		}

		var remaining = GetRemainingCooldown(skillId);
		return $"Skill {skillId}: Last used {(DateTime.Now - entry.LastUseTime).TotalSeconds:F1}s ago, " +
		       $"Cooldown: {entry.CooldownSeconds:F2}s, Remaining: {remaining:F0}ms";
	}
}
