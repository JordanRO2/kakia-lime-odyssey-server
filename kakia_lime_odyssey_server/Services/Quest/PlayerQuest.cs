namespace kakia_lime_odyssey_server.Services.Quest;

public class PlayerQuest
{
	public uint TypeID { get; set; }
	public QuestType QuestType { get; set; }
	public QuestState State { get; set; }
	public DateTime AcceptedAt { get; set; }
	public DateTime? CompletedAt { get; set; }
}

public enum QuestType : byte
{
	Main = 0,
	Sub = 1,
	Normal = 2
}

public enum QuestState : byte
{
	InProgress = 0,
	Completable = 1,
	Completed = 2
}
