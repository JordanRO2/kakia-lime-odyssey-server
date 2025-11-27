/// <summary>
/// Service for managing player quests with database persistence.
/// </summary>
/// <remarks>
/// Uses: MongoDBService for quest persistence
/// Handles: Accept, abandon, complete quests, state updates
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Models.Persistence;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Quest;

/// <summary>
/// Service for managing player quests with MongoDB persistence.
/// </summary>
public class QuestService
{
	/// <summary>In-memory cache for active quests (synced with DB)</summary>
	private readonly ConcurrentDictionary<string, PlayerQuests> _questCache = new();
	private const int MaxActiveQuests = 30;

	/// <summary>
	/// Accepts a quest for a player.
	/// </summary>
	public bool AcceptQuest(PlayerClient player, uint questTypeID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		if (quests.ActiveQuests.Count >= MaxActiveQuests)
		{
			Logger.Log($"[QUEST] {charName} quest log full", LogLevel.Debug);
			return false;
		}

		if (quests.ActiveQuests.Any(q => q.QuestId == (int)questTypeID))
		{
			Logger.Log($"[QUEST] {charName} already has quest {questTypeID}", LogLevel.Debug);
			return false;
		}

		// Check if already completed (for non-repeatable quests)
		if (quests.CompletedQuests.Contains((int)questTypeID))
		{
			Logger.Log($"[QUEST] {charName} already completed quest {questTypeID}", LogLevel.Debug);
			return false;
		}

		var quest = new QuestProgress
		{
			QuestId = (int)questTypeID,
			State = 0,
			AcceptedAt = DateTime.UtcNow
		};

		quests.ActiveQuests.Add(quest);
		SaveQuests(accountId, charName, quests);

		SendQuestAdd(player, questTypeID);

		Logger.Log($"[QUEST] {charName} accepted quest {questTypeID}", LogLevel.Debug);
		return true;
	}

	/// <summary>
	/// Abandons a quest for a player.
	/// </summary>
	public bool AbandonQuest(PlayerClient player, uint questTypeID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		var quest = quests.ActiveQuests.FirstOrDefault(q => q.QuestId == (int)questTypeID);
		if (quest == null)
		{
			Logger.Log($"[QUEST] Quest {questTypeID} not found in {charName}'s log", LogLevel.Debug);
			return false;
		}

		quests.ActiveQuests.Remove(quest);
		quests.AbandonedQuests.Add((int)questTypeID);
		SaveQuests(accountId, charName, quests);

		SendQuestDelete(player, questTypeID);

		Logger.Log($"[QUEST] {charName} abandoned quest {questTypeID}", LogLevel.Debug);
		return true;
	}

	/// <summary>
	/// Completes a quest for a player.
	/// </summary>
	public bool CompleteQuest(PlayerClient player, uint questTypeID, int[] rewardChoices)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		var quest = quests.ActiveQuests.FirstOrDefault(q => q.QuestId == (int)questTypeID);
		if (quest == null)
		{
			Logger.Log($"[QUEST] Quest {questTypeID} not found for completion", LogLevel.Debug);
			return false;
		}

		quests.ActiveQuests.Remove(quest);
		quests.CompletedQuests.Add((int)questTypeID);
		SaveQuests(accountId, charName, quests);

		// Count completed quests by type (simplified - would need quest XML data for proper categorization)
		int completedMain = quests.CompletedQuests.Count(id => id < 1000);
		int completedSub = quests.CompletedQuests.Count(id => id >= 1000 && id < 10000);
		int completedNormal = quests.CompletedQuests.Count(id => id >= 10000);

		SendQuestComplete(player, questTypeID, completedMain, completedSub, completedNormal);

		Logger.Log($"[QUEST] {charName} completed quest {questTypeID}", LogLevel.Info);
		return true;
	}

	/// <summary>
	/// Updates quest objective progress.
	/// </summary>
	public void UpdateQuestProgress(PlayerClient player, uint questTypeID, int objectiveIndex, int progress)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		var quest = quests.ActiveQuests.FirstOrDefault(q => q.QuestId == (int)questTypeID);
		if (quest == null) return;

		quest.ObjectiveProgress[objectiveIndex] = progress;
		SaveQuests(accountId, charName, quests);

		Logger.Log($"[QUEST] {charName} quest {questTypeID} objective {objectiveIndex} = {progress}", LogLevel.Debug);
	}

	/// <summary>
	/// Updates quest state.
	/// </summary>
	public void UpdateQuestState(PlayerClient player, uint questTypeID, int newState)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		var quest = quests.ActiveQuests.FirstOrDefault(q => q.QuestId == (int)questTypeID);
		if (quest == null) return;

		quest.State = newState;
		SaveQuests(accountId, charName, quests);

		SendUpdateQuestState(player, questTypeID, (byte)newState);

		Logger.Log($"[QUEST] Quest {questTypeID} state updated to {newState}", LogLevel.Debug);
	}

	/// <summary>
	/// Gets active quests for a player.
	/// </summary>
	public List<QuestProgress> GetPlayerQuests(PlayerClient player)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return new List<QuestProgress>();

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		return quests.ActiveQuests;
	}

	/// <summary>
	/// Checks if a player has completed a quest.
	/// </summary>
	public bool HasCompletedQuest(PlayerClient player, uint questTypeID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		return quests.CompletedQuests.Contains((int)questTypeID);
	}

	/// <summary>
	/// Checks if a player has an active quest.
	/// </summary>
	public bool HasActiveQuest(PlayerClient player, uint questTypeID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		return quests.ActiveQuests.Any(q => q.QuestId == (int)questTypeID);
	}

	/// <summary>
	/// Loads quests for a player on login.
	/// </summary>
	public void LoadPlayerQuests(PlayerClient player)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;

		// Force load from DB
		string cacheKey = $"{accountId}:{charName}";
		var quests = MongoDBService.Instance.GetPlayerQuests(accountId, charName);
		_questCache[cacheKey] = quests;

		Logger.Log($"[QUEST] Loaded {quests.ActiveQuests.Count} active quests for {charName}", LogLevel.Debug);
	}

	/// <summary>
	/// Clears cached quests for a player on logout.
	/// </summary>
	public void UnloadPlayerQuests(PlayerClient player)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		string cacheKey = $"{accountId}:{charName}";

		_questCache.TryRemove(cacheKey, out _);
	}

	/// <summary>
	/// Gets the available quests from a quest board.
	/// </summary>
	public void GetBoardQuests(PlayerClient player, long boardId)
	{
		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} requesting quests from board {boardId}", LogLevel.Debug);

		// Send empty board quests for now - will be populated from quest XML data
		using PacketWriter pw = new();
		pw.Write(new SC_BOARD_QUESTS());
		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Processes a dialog choice selection from NPC interaction.
	/// </summary>
	public void ProcessDialogChoice(PlayerClient player, long npcId, int choiceNum)
	{
		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} chose option {choiceNum} with NPC {npcId}", LogLevel.Debug);

		// Send default response for now - will process choices from NPC dialog data
		using PacketWriter pw = new();
		pw.Write(new SC_TALKING
		{
			objInstID = npcId,
			dialog = $"Option {choiceNum} selected."
		});
		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	#region Private Helpers

	private PlayerQuests GetOrLoadQuests(string accountId, string charName)
	{
		string cacheKey = $"{accountId}:{charName}";
		return _questCache.GetOrAdd(cacheKey, _ => MongoDBService.Instance.GetPlayerQuests(accountId, charName));
	}

	private void SaveQuests(string accountId, string charName, PlayerQuests quests)
	{
		string cacheKey = $"{accountId}:{charName}";
		_questCache[cacheKey] = quests;
		MongoDBService.Instance.SavePlayerQuests(accountId, charName, quests);
	}

	#endregion

	#region Packet Sending

	private static void SendQuestAdd(PlayerClient player, uint questTypeID)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_QUEST_ADD { typeID = questTypeID });
		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	private static void SendQuestDelete(PlayerClient player, uint questTypeID)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_QUEST_DELETE { typeID = questTypeID });
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendQuestComplete(PlayerClient player, uint questTypeID, int completedMain, int completedSub, int completedNormal)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_QUEST_COMPLETE
		{
			typeID = questTypeID,
			completedMain = completedMain,
			completedSub = completedSub,
			completedNormal = completedNormal
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendUpdateQuestState(PlayerClient player, uint questTypeID, byte state)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_UPDATE_QUEST_STATE { typeID = questTypeID, state = state });
		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	#endregion
}
