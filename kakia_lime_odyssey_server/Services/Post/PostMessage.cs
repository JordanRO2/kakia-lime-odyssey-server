using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_server.Services.Post;

public class PostMessage
{
	public int IndexNumber { get; set; }
	public string FromName { get; set; } = string.Empty;
	public long FromPlayerId { get; set; }
	public string ToName { get; set; } = string.Empty;
	public long ToPlayerId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Body { get; set; } = string.Empty;
	public List<PostAttachment> Attachments { get; set; } = new();
	public DateTime SentAt { get; set; }
	public bool IsRead { get; set; }
	public bool ItemsTaken { get; set; }
}

public class PostAttachment
{
	public int TypeID { get; set; }
	public int Count { get; set; }
	public int RemainExpiryTime { get; set; }
	public int Durability { get; set; }
	public int MaxDurability { get; set; }
	public int Grade { get; set; }
	public ITEM_INHERITS Inherits { get; set; }

	public POST_ATTACHED ToPacketStruct()
	{
		return new POST_ATTACHED
		{
			typeID = TypeID,
			count = Count,
			remainExpiryTime = RemainExpiryTime,
			durability = Durability,
			mdurability = MaxDurability,
			grade = Grade,
			inherits = Inherits
		};
	}
}
