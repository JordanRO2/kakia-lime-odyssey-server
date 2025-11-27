namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Persisted social data for a character (friends, blocks)
/// </summary>
public class PlayerSocial
{
	/// <summary>Friend list entries</summary>
	public List<FriendEntry> Friends { get; set; } = new();

	/// <summary>Blocked player entries</summary>
	public List<BlockEntry> BlockedPlayers { get; set; } = new();
}

/// <summary>
/// A friend list entry
/// </summary>
public class FriendEntry
{
	/// <summary>Friend's character name</summary>
	public string CharacterName { get; set; } = string.Empty;

	/// <summary>When the friendship was established</summary>
	public DateTime AddedAt { get; set; } = DateTime.UtcNow;

	/// <summary>Custom note/memo for this friend</summary>
	public string Note { get; set; } = string.Empty;

	/// <summary>Friend group/category</summary>
	public string Group { get; set; } = "Default";
}

/// <summary>
/// A blocked player entry
/// </summary>
public class BlockEntry
{
	/// <summary>Blocked character name</summary>
	public string CharacterName { get; set; } = string.Empty;

	/// <summary>When the player was blocked</summary>
	public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

	/// <summary>Reason for blocking (optional)</summary>
	public string Reason { get; set; } = string.Empty;
}
