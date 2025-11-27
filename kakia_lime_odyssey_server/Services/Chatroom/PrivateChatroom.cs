using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Chatroom;

public class PrivateChatroom
{
	public long RoomId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public byte MaxPersons { get; set; }
	public byte Type { get; set; }
	public long MasterId { get; set; }
	public PlayerClient Master { get; set; } = null!;
	public List<ChatroomMember> Members { get; set; } = new();
	public DateTime CreatedAt { get; set; }

	public bool IsFull => Members.Count >= MaxPersons;
	public bool HasPassword => !string.IsNullOrEmpty(Password);

	public bool CheckPassword(string password)
	{
		if (!HasPassword) return true;
		return Password == password;
	}
}

public class ChatroomMember
{
	public long InstId { get; set; }
	public string Name { get; set; } = string.Empty;
	public PlayerClient Player { get; set; } = null!;
}
