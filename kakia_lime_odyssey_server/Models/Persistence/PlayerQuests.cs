namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Persisted quest data for a character
/// </summary>
public class PlayerQuests
{
	/// <summary>Currently active quests</summary>
	public List<QuestProgress> ActiveQuests { get; set; } = new();

	/// <summary>Completed quest IDs (for tracking completion)</summary>
	public HashSet<int> CompletedQuests { get; set; } = new();

	/// <summary>Failed/abandoned quest IDs (for cooldown tracking)</summary>
	public HashSet<int> AbandonedQuests { get; set; } = new();
}

/// <summary>
/// Progress tracking for a single quest
/// </summary>
public class QuestProgress
{
	/// <summary>Quest ID from quest XML files</summary>
	public int QuestId { get; set; }

	/// <summary>Current quest state/step</summary>
	public int State { get; set; }

	/// <summary>Progress on quest objectives (key: objective index, value: current count)</summary>
	public Dictionary<int, int> ObjectiveProgress { get; set; } = new();

	/// <summary>When the quest was accepted</summary>
	public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;

	/// <summary>Quest expiry time (for timed quests, null if no expiry)</summary>
	public DateTime? ExpiresAt { get; set; }
}
