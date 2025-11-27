using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_server.Models.Persistence;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Quest type categories based on typeId ranges from QuestInfo.xml
/// </summary>
public enum QuestCategory
{
	/// <summary>Main story quests (typeId 000-002: History, Race, Camp)</summary>
	Main,
	/// <summary>Sub quests (typeId 100-102: Zone, Job, Legend)</summary>
	Sub,
	/// <summary>Normal quests (typeId 200-207: Hunting, Gather, Delivery, etc.)</summary>
	Normal
}

/// <summary>
/// Tracks player quest progress and provides quest data for packets
/// </summary>
public class PlayerQuestsTracker : IPlayerQuests
{
	private PlayerQuests _questData;

	// Completed quest counts by category (persisted separately for performance)
	private int _completedMain;
	private int _completedSub;
	private int _completedNormal;

	public PlayerQuestsTracker()
	{
		_questData = new PlayerQuests();
		_completedMain = 0;
		_completedSub = 0;
		_completedNormal = 0;
	}

	public PlayerQuestsTracker(PlayerQuests questData)
	{
		_questData = questData;
		RecalculateCompletedCounts();
	}

	public int ActiveQuestCount => _questData.ActiveQuests.Count;
	public int CompletedMainQuests => _completedMain;
	public int CompletedSubQuests => _completedSub;
	public int CompletedNormalQuests => _completedNormal;

	public IEnumerable<int> GetActiveQuestIds()
	{
		return _questData.ActiveQuests.Select(q => q.QuestId);
	}

	public bool HasQuest(int questId)
	{
		return _questData.ActiveQuests.Any(q => q.QuestId == questId);
	}

	public bool IsQuestCompleted(int questId)
	{
		return _questData.CompletedQuests.Contains(questId);
	}

	/// <summary>
	/// Gets the underlying persistence data for saving
	/// </summary>
	public PlayerQuests GetPersistenceData()
	{
		return _questData;
	}

	/// <summary>
	/// Adds a new quest to the active quest list
	/// </summary>
	public bool AcceptQuest(int questId)
	{
		if (HasQuest(questId) || IsQuestCompleted(questId))
			return false;

		_questData.ActiveQuests.Add(new QuestProgress
		{
			QuestId = questId,
			State = 0,
			AcceptedAt = DateTime.UtcNow
		});
		return true;
	}

	/// <summary>
	/// Completes a quest and moves it to the completed list
	/// </summary>
	public bool CompleteQuest(int questId, QuestCategory category)
	{
		var quest = _questData.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
		if (quest == null)
			return false;

		_questData.ActiveQuests.Remove(quest);
		_questData.CompletedQuests.Add(questId);

		// Update category counts
		switch (category)
		{
			case QuestCategory.Main:
				_completedMain++;
				break;
			case QuestCategory.Sub:
				_completedSub++;
				break;
			case QuestCategory.Normal:
				_completedNormal++;
				break;
		}

		return true;
	}

	/// <summary>
	/// Abandons a quest, moving it to the abandoned list
	/// </summary>
	public bool AbandonQuest(int questId)
	{
		var quest = _questData.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
		if (quest == null)
			return false;

		_questData.ActiveQuests.Remove(quest);
		_questData.AbandonedQuests.Add(questId);
		return true;
	}

	/// <summary>
	/// Updates progress on a quest objective
	/// </summary>
	public bool UpdateQuestProgress(int questId, int objectiveIndex, int progress)
	{
		var quest = _questData.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
		if (quest == null)
			return false;

		quest.ObjectiveProgress[objectiveIndex] = progress;
		return true;
	}

	/// <summary>
	/// Gets quest progress for a specific quest
	/// </summary>
	public QuestProgress? GetQuestProgress(int questId)
	{
		return _questData.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
	}

	/// <summary>
	/// Determines the category of a quest based on its typeId
	/// </summary>
	public static QuestCategory GetQuestCategory(int questTypeId)
	{
		// Based on QuestInfo.xml typeId ranges:
		// 000-002: Main (History, Race, Camp)
		// 100-102: Sub (Zone, Job, Legend)
		// 200-207: Normal (Hunting, Gather, Delivery, etc.)
		return questTypeId switch
		{
			< 100 => QuestCategory.Main,
			< 200 => QuestCategory.Sub,
			_ => QuestCategory.Normal
		};
	}

	/// <summary>
	/// Recalculates completed quest counts from the completed quests set
	/// This is called when loading from persistence to restore counts
	/// </summary>
	private void RecalculateCompletedCounts()
	{
		// For now, we cannot determine category from just the quest ID
		// We would need the quest database to look up each quest's type
		// For simplicity, we'll track counts separately in persistence
		// This is a placeholder - actual implementation would query quest DB
		_completedMain = 0;
		_completedSub = 0;
		_completedNormal = _questData.CompletedQuests.Count;
	}
}
