using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Quest;

public class QuestService
{
	private readonly ConcurrentDictionary<long, List<PlayerQuest>> _playerQuests = new();
	private readonly ConcurrentDictionary<long, QuestStats> _playerStats = new();
	private const int MaxActiveQuests = 30;

	public bool AcceptQuest(PlayerClient player, uint questTypeID)
	{
		long playerId = player.GetId();
		var quests = GetOrCreateQuestList(playerId);

		if (quests.Count >= MaxActiveQuests)
		{
			Logger.Log($"[QUEST] Player quest log full", LogLevel.Debug);
			return false;
		}

		if (quests.Any(q => q.TypeID == questTypeID))
		{
			Logger.Log($"[QUEST] Quest {questTypeID} already in log", LogLevel.Debug);
			return false;
		}

		var quest = new PlayerQuest
		{
			TypeID = questTypeID,
			QuestType = QuestType.Normal,
			State = QuestState.InProgress,
			AcceptedAt = DateTime.Now
		};

		quests.Add(quest);

		SendQuestAdd(player, questTypeID);

		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} accepted quest {questTypeID}", LogLevel.Debug);

		return true;
	}

	public bool AbandonQuest(PlayerClient player, uint questTypeID)
	{
		long playerId = player.GetId();
		var quests = GetOrCreateQuestList(playerId);

		var quest = quests.FirstOrDefault(q => q.TypeID == questTypeID);
		if (quest == null)
		{
			Logger.Log($"[QUEST] Quest {questTypeID} not found in log", LogLevel.Debug);
			return false;
		}

		quests.Remove(quest);

		SendQuestDelete(player, questTypeID);

		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} abandoned quest {questTypeID}", LogLevel.Debug);

		return true;
	}

	public bool CompleteQuest(PlayerClient player, uint questTypeID, int[] rewardChoices)
	{
		long playerId = player.GetId();
		var quests = GetOrCreateQuestList(playerId);

		var quest = quests.FirstOrDefault(q => q.TypeID == questTypeID);
		if (quest == null)
		{
			Logger.Log($"[QUEST] Quest {questTypeID} not found for completion", LogLevel.Debug);
			return false;
		}

		quests.Remove(quest);

		var stats = GetOrCreateStats(playerId);
		switch (quest.QuestType)
		{
			case QuestType.Main:
				stats.CompletedMain++;
				break;
			case QuestType.Sub:
				stats.CompletedSub++;
				break;
			case QuestType.Normal:
				stats.CompletedNormal++;
				break;
		}

		SendQuestComplete(player, questTypeID, stats.CompletedMain, stats.CompletedSub, stats.CompletedNormal);

		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} completed quest {questTypeID}", LogLevel.Info);

		return true;
	}

	public void UpdateQuestState(PlayerClient player, uint questTypeID, QuestState newState)
	{
		long playerId = player.GetId();
		var quests = GetOrCreateQuestList(playerId);

		var quest = quests.FirstOrDefault(q => q.TypeID == questTypeID);
		if (quest == null) return;

		quest.State = newState;

		SendUpdateQuestState(player, questTypeID, (byte)newState);

		Logger.Log($"[QUEST] Quest {questTypeID} state updated to {newState}", LogLevel.Debug);
	}

	public List<PlayerQuest> GetPlayerQuests(long playerId)
	{
		return GetOrCreateQuestList(playerId);
	}

	public QuestStats GetPlayerStats(long playerId)
	{
		return GetOrCreateStats(playerId);
	}

	private List<PlayerQuest> GetOrCreateQuestList(long playerId)
	{
		return _playerQuests.GetOrAdd(playerId, _ => new List<PlayerQuest>());
	}

	private QuestStats GetOrCreateStats(long playerId)
	{
		return _playerStats.GetOrAdd(playerId, _ => new QuestStats());
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
}

public class QuestStats
{
	public int CompletedMain { get; set; }
	public int CompletedSub { get; set; }
	public int CompletedNormal { get; set; }
}
