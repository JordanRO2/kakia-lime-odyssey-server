/// <summary>
/// Service for managing player quests with database persistence and XML quest definitions.
/// </summary>
/// <remarks>
/// Uses: MongoDBService for quest persistence, LimeServer.QuestDB for quest definitions
/// Handles: Accept, abandon, complete quests, state updates, quest validation
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Models.Persistence;
using kakia_lime_odyssey_server.Models.QuestXML;
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
	/// Gets a quest definition from the QuestDB by type ID.
	/// </summary>
	/// <param name="questTypeID">Quest type ID.</param>
	/// <returns>Quest definition or null if not found.</returns>
	public static XmlQuest? GetQuestDefinition(uint questTypeID)
	{
		return LimeServer.QuestDB.FirstOrDefault(q => q.TypeID == questTypeID);
	}

	/// <summary>
	/// Checks if a quest exists in the database.
	/// </summary>
	/// <param name="questTypeID">Quest type ID.</param>
	/// <returns>True if quest exists.</returns>
	public static bool QuestExists(uint questTypeID)
	{
		return LimeServer.QuestDB.Any(q => q.TypeID == questTypeID);
	}

	/// <summary>
	/// Gets all quests available from a specific NPC.
	/// </summary>
	/// <param name="npcName">NPC name.</param>
	/// <returns>List of quests the NPC can give.</returns>
	public static List<XmlQuest> GetQuestsForNPC(string npcName)
	{
		return LimeServer.QuestDB.Where(q => q.NPC == npcName).ToList();
	}

	/// <summary>
	/// Gets quests available for a player's level.
	/// </summary>
	/// <param name="playerLevel">Player's current level.</param>
	/// <returns>List of available quests.</returns>
	public static List<XmlQuest> GetQuestsForLevel(int playerLevel)
	{
		return LimeServer.QuestDB.Where(q => q.Level <= playerLevel).ToList();
	}

	/// <summary>
	/// Accepts a quest for a player.
	/// </summary>
	/// <param name="player">The player accepting the quest.</param>
	/// <param name="questTypeID">Quest type ID to accept.</param>
	/// <returns>True if quest was accepted successfully.</returns>
	public bool AcceptQuest(PlayerClient player, uint questTypeID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;

		// Validate quest exists in QuestDB
		var questDef = GetQuestDefinition(questTypeID);
		if (questDef == null)
		{
			Logger.Log($"[QUEST] Quest {questTypeID} not found in QuestDB", LogLevel.Warning);
			return false;
		}

		// Check level requirement
		var entityStatus = (player as Interfaces.IEntity)?.GetEntityStatus();
		if (entityStatus != null && entityStatus.Lv < questDef.Level)
		{
			Logger.Log($"[QUEST] {charName} level {entityStatus.Lv} too low for quest {questTypeID} (requires {questDef.Level})", LogLevel.Debug);
			return false;
		}

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
		if (quests.CompletedQuests.Contains((int)questTypeID) && questDef.Repeatable != 1)
		{
			Logger.Log($"[QUEST] {charName} already completed non-repeatable quest {questTypeID}", LogLevel.Debug);
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

		Logger.Log($"[QUEST] {charName} accepted quest {questTypeID} ({questDef.TypeName})", LogLevel.Debug);
		return true;
	}

	/// <summary>
	/// Abandons a quest for a player.
	/// </summary>
	/// <param name="player">The player abandoning the quest.</param>
	/// <param name="questTypeID">Quest type ID to abandon.</param>
	/// <returns>True if quest was abandoned successfully.</returns>
	public bool AbandonQuest(PlayerClient player, uint questTypeID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;

		// Check if quest is cancelable
		var questDef = GetQuestDefinition(questTypeID);
		if (questDef != null && questDef.Cancelable != 1)
		{
			Logger.Log($"[QUEST] Quest {questTypeID} cannot be abandoned (not cancelable)", LogLevel.Debug);
			return false;
		}

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
	/// <param name="player">The player completing the quest.</param>
	/// <param name="questTypeID">Quest type ID to complete.</param>
	/// <param name="rewardChoices">Indices of selected choice rewards.</param>
	/// <returns>True if quest was completed successfully.</returns>
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

		// Get quest definition for rewards and categorization
		var questDef = GetQuestDefinition(questTypeID);

		quests.ActiveQuests.Remove(quest);
		quests.CompletedQuests.Add((int)questTypeID);
		SaveQuests(accountId, charName, quests);

		// Count completed quests by type using QuestDB for proper categorization
		int completedMain = CountCompletedByCategory(quests.CompletedQuests.ToList(), q => q?.IsMainQuest == true);
		int completedSub = CountCompletedByCategory(quests.CompletedQuests.ToList(), q => q?.IsSideQuest == true);
		int completedNormal = CountCompletedByCategory(quests.CompletedQuests.ToList(), q => q?.IsNormalQuest == true);

		SendQuestComplete(player, questTypeID, completedMain, completedSub, completedNormal);

		// Grant experience rewards if available
		if (questDef != null && player is Interfaces.IEntity entity)
		{
			GrantQuestRewards(entity, player, questDef, rewardChoices);
		}

		string questName = questDef?.TypeName ?? "Unknown";
		Logger.Log($"[QUEST] {charName} completed quest {questTypeID} ({questName})", LogLevel.Information);
		return true;
	}

	/// <summary>
	/// Counts completed quests matching a category predicate.
	/// </summary>
	private static int CountCompletedByCategory(List<int> completedIds, Func<XmlQuest?, bool> predicate)
	{
		return completedIds.Count(id => predicate(GetQuestDefinition((uint)id)));
	}

	/// <summary>
	/// Grants rewards from a completed quest.
	/// </summary>
	private static void GrantQuestRewards(Interfaces.IEntity entity, PlayerClient player, XmlQuest questDef, int[] rewardChoices)
	{
		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		bool inventoryChanged = false;

		// Grant combat experience rewards
		if (questDef.RewardEXP > 0 || questDef.RewardCombatJobEXP > 0)
		{
			ulong totalExp = questDef.RewardEXP + questDef.RewardCombatJobEXP;
			if (totalExp > 0)
			{
				bool levelUp = entity.AddExp(totalExp);
				var status = entity.GetEntityStatus();

				using PacketWriter pw = new();
				pw.Write(new SC_GOT_COMBAT_JOB_EXP
				{
					exp = (uint)status.Exp,
					addExp = (uint)totalExp
				});
				player.Send(pw.ToPacket(), default).Wait();

				if (levelUp)
				{
					using PacketWriter lvPw = new();
					lvPw.Write(new SC_PC_COMBAT_JOB_LEVEL_UP
					{
						objInstID = entity.GetId(),
						lv = status.Lv,
						exp = (uint)status.Exp,
						newStr = 5,
						newInl = 5,
						newAgi = 5,
						newDex = 5,
						newSpi = 5,
						newVit = 5
					});
					player.Send(lvPw.ToPacket(), default).Wait();
					player.SendGlobalPacket(lvPw.ToPacket(), default).Wait();
				}

				Logger.Log($"[QUEST] Granted {totalExp} combat exp to {playerName} from quest {questDef.TypeID}", LogLevel.Debug);
			}
		}

		// Grant life job experience rewards
		if (questDef.RewardLifeJobEXP > 0)
		{
			// Life job exp would need separate tracking - for now log it
			Logger.Log($"[QUEST] Life job EXP reward: {questDef.RewardLifeJobEXP} (tracking not yet implemented)", LogLevel.Debug);
		}

		// Grant basic reward items
		if (questDef.BasicReward != null && questDef.BasicReward.TypeID > 0)
		{
			inventoryChanged |= GrantItemReward(player, questDef.BasicReward.TypeID, questDef.BasicReward.Count);
		}

		// Grant selected choice rewards (player selected from UI)
		if (rewardChoices != null && questDef.ChoiceRewards.Count > 0)
		{
			foreach (int choice in rewardChoices)
			{
				if (choice >= 0 && choice < questDef.ChoiceRewards.Count)
				{
					var reward = questDef.ChoiceRewards[choice];
					inventoryChanged |= GrantItemReward(player, reward.TypeID, reward.Count);
				}
			}
		}

		// Send inventory update if items were added
		if (inventoryChanged)
		{
			player.SendInventory();
		}
	}

	/// <summary>
	/// Grants a single item reward to the player.
	/// </summary>
	/// <param name="player">The player receiving the reward.</param>
	/// <param name="itemTypeId">Item type ID to grant.</param>
	/// <param name="count">Number of items to grant.</param>
	/// <returns>True if item was added to inventory.</returns>
	private static bool GrantItemReward(PlayerClient player, uint itemTypeId, int count)
	{
		if (itemTypeId == 0 || count <= 0) return false;

		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Check for Peder (gold) reward - type ID 1 is typically gold
		if (itemTypeId == 1)
		{
			LimeServer.CurrencyService.AddPeder(player, count);
			Logger.Log($"[QUEST] Granted {count} Peder to {playerName}", LogLevel.Debug);
			return false; // No inventory change for currency
		}

		// Get item definition from ItemDB
		var itemDef = LimeServer.ItemDB.FirstOrDefault(i => i.Id == (int)itemTypeId);
		if (itemDef == null)
		{
			Logger.Log($"[QUEST] Item {itemTypeId} not found in ItemDB for reward", LogLevel.Warning);
			return false;
		}

		// Clone item definition and set count
		var rewardItem = new Models.Item
		{
			Id = itemDef.Id,
			ModelId = itemDef.ModelId,
			Name = itemDef.Name,
			Desc = itemDef.Desc,
			Grade = itemDef.Grade,
			MaxEnchantCount = itemDef.MaxEnchantCount,
			Type = itemDef.Type,
			SecondType = itemDef.SecondType,
			Level = itemDef.Level,
			TribeClass = itemDef.TribeClass,
			JobClassType = itemDef.JobClassType,
			JobClassTypeId = itemDef.JobClassTypeId,
			WeaponType = itemDef.WeaponType,
			UserType = itemDef.UserType,
			StuffType = itemDef.StuffType,
			SkillId = itemDef.SkillId,
			ImageName = itemDef.ImageName,
			SmallImageName = itemDef.SmallImageName,
			SortingType = itemDef.SortingType,
			Series = itemDef.Series,
			IsSell = itemDef.IsSell,
			IsExchange = itemDef.IsExchange,
			IsDiscard = itemDef.IsDiscard,
			Material = itemDef.Material,
			Durable = itemDef.Durable,
			Price = itemDef.Price,
			Inherits = itemDef.Inherits != null ? new List<Models.Inherit>(itemDef.Inherits) : new List<Models.Inherit>(),
			Count = (ulong)count
		};

		// Add to inventory
		var inventory = player.GetInventory();
		int slot = inventory.AddItem(rewardItem);

		if (slot >= 0)
		{
			Logger.Log($"[QUEST] Granted {count}x {itemDef.Name} (ID: {itemTypeId}) to {playerName} at slot {slot}", LogLevel.Debug);
			return true;
		}
		else
		{
			Logger.Log($"[QUEST] Failed to add {itemDef.Name} to {playerName}'s inventory (full?)", LogLevel.Warning);
			return false;
		}
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

	#region Objective Tracking

	/// <summary>
	/// Called when a monster is killed. Updates Hunt objectives for all relevant quests.
	/// </summary>
	/// <param name="player">The player who killed the monster.</param>
	/// <param name="monsterTypeId">The type ID of the killed monster.</param>
	public void OnMonsterKilled(PlayerClient player, int monsterTypeId)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		bool anyUpdated = false;

		foreach (var quest in quests.ActiveQuests)
		{
			var questDef = GetQuestDefinition((uint)quest.QuestId);
			if (questDef?.Storage == null) continue;

			var huntObjectives = questDef.Storage.GetHuntObjectives();
			foreach (var objective in huntObjectives)
			{
				if (objective.TargetTypeID != monsterTypeId) continue;

				// Get current progress
				int currentProgress = quest.ObjectiveProgress.GetValueOrDefault(objective.Index, 0);
				if (currentProgress >= objective.Goal) continue; // Already completed

				// Increment progress
				int newProgress = currentProgress + 1;
				quest.ObjectiveProgress[objective.Index] = newProgress;
				anyUpdated = true;

				// Check if objective completed
				bool objectiveCompleted = newProgress >= objective.Goal;

				// Notify client
				SendQuestSubjectChange(player, (uint)quest.QuestId, (byte)objective.Index, objectiveCompleted);

				Logger.Log($"[QUEST] {charName} hunt progress: quest {quest.QuestId} obj {objective.Index} = {newProgress}/{objective.Goal}", LogLevel.Debug);
			}
		}

		if (anyUpdated)
		{
			SaveQuests(accountId, charName, quests);
		}
	}

	/// <summary>
	/// Called when an item is collected/picked up. Updates Collect objectives for all relevant quests.
	/// </summary>
	/// <param name="player">The player who collected the item.</param>
	/// <param name="itemTypeId">The type ID of the collected item.</param>
	/// <param name="count">Number of items collected.</param>
	public void OnItemCollected(PlayerClient player, int itemTypeId, int count = 1)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		bool anyUpdated = false;

		foreach (var quest in quests.ActiveQuests)
		{
			var questDef = GetQuestDefinition((uint)quest.QuestId);
			if (questDef?.Storage == null) continue;

			var collectObjectives = questDef.Storage.GetCollectObjectives();
			foreach (var objective in collectObjectives)
			{
				if (objective.TargetTypeID != itemTypeId) continue;

				// Get current progress
				int currentProgress = quest.ObjectiveProgress.GetValueOrDefault(objective.Index, 0);
				if (currentProgress >= objective.Goal) continue; // Already completed

				// Increment progress (capped at goal)
				int newProgress = Math.Min(currentProgress + count, objective.Goal);
				quest.ObjectiveProgress[objective.Index] = newProgress;
				anyUpdated = true;

				// Check if objective completed
				bool objectiveCompleted = newProgress >= objective.Goal;

				// Notify client
				SendQuestSubjectChange(player, (uint)quest.QuestId, (byte)objective.Index, objectiveCompleted);

				Logger.Log($"[QUEST] {charName} collect progress: quest {quest.QuestId} obj {objective.Index} = {newProgress}/{objective.Goal}", LogLevel.Debug);
			}
		}

		if (anyUpdated)
		{
			SaveQuests(accountId, charName, quests);
		}
	}

	/// <summary>
	/// Checks if all objectives for a quest are completed.
	/// </summary>
	/// <param name="player">The player to check.</param>
	/// <param name="questTypeID">Quest type ID to check.</param>
	/// <returns>True if all objectives are met.</returns>
	public bool AreAllObjectivesComplete(PlayerClient player, uint questTypeID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		var quest = quests.ActiveQuests.FirstOrDefault(q => q.QuestId == (int)questTypeID);
		if (quest == null) return false;

		var questDef = GetQuestDefinition(questTypeID);
		if (questDef?.Storage == null) return true; // No objectives = complete

		var allObjectives = questDef.Storage.GetAllObjectives();
		foreach (var objective in allObjectives)
		{
			int currentProgress = quest.ObjectiveProgress.GetValueOrDefault(objective.Index, 0);
			if (currentProgress < objective.Goal)
				return false;
		}

		return true;
	}

	/// <summary>
	/// Gets the current progress for a specific quest objective.
	/// </summary>
	/// <param name="player">The player.</param>
	/// <param name="questTypeID">Quest type ID.</param>
	/// <param name="objectiveIndex">Objective index (0-2).</param>
	/// <returns>Current progress count.</returns>
	public int GetObjectiveProgress(PlayerClient player, uint questTypeID, int objectiveIndex)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return 0;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		var quest = quests.ActiveQuests.FirstOrDefault(q => q.QuestId == (int)questTypeID);
		if (quest == null) return 0;

		return quest.ObjectiveProgress.GetValueOrDefault(objectiveIndex, 0);
	}

	#endregion

	#region Packet Sending

	private static void SendQuestSubjectChange(PlayerClient player, uint questTypeID, byte subjectNum, bool completed)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_CHANGE_QUEST_SUBJECT
		{
			typeID = questTypeID,
			subjectNum = subjectNum,
			isSuccessed = completed
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

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

	/// <summary>
	/// Sends the quest turn-in dialog to the player and sets up pending turn-in tracking.
	/// </summary>
	/// <param name="player">The player turning in the quest.</param>
	/// <param name="questTypeID">Quest type ID being turned in.</param>
	/// <param name="npcObjInstID">NPC instance ID handling the turn-in.</param>
	/// <returns>True if quest report dialog was sent.</returns>
	public bool StartQuestTurnIn(PlayerClient player, uint questTypeID, long npcObjInstID)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var quests = GetOrLoadQuests(accountId, charName);

		// Verify quest is active
		var quest = quests.ActiveQuests.FirstOrDefault(q => q.QuestId == (int)questTypeID);
		if (quest == null)
		{
			Logger.Log($"[QUEST] Cannot turn in quest {questTypeID} - not active", LogLevel.Debug);
			return false;
		}

		// Set pending turn-in state so CS_QUEST_COMPLETE knows which quest
		player.SetPendingTurnInQuest(questTypeID);

		// Send quest report talk dialog
		using PacketWriter pw = new();
		pw.Write(new SC_QUEST_REPORT_TALK { typeID = questTypeID, objInstID = npcObjInstID });
		player.Send(pw.ToSizedPacket(), default).Wait();

		Logger.Log($"[QUEST] {charName} started turn-in dialog for quest {questTypeID}", LogLevel.Debug);
		return true;
	}

	#endregion
}
