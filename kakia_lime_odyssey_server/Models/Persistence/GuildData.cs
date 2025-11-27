namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Persisted guild data (stored globally, not per-character)
/// </summary>
public class GuildData
{
	/// <summary>Unique guild ID</summary>
	public int GuildId { get; set; }

	/// <summary>Guild name (unique)</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Guild leader character name</summary>
	public string LeaderName { get; set; } = string.Empty;

	/// <summary>Guild notice/message of the day</summary>
	public string Notice { get; set; } = string.Empty;

	/// <summary>Guild creation date</summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>Guild level</summary>
	public byte Level { get; set; } = 1;

	/// <summary>Guild experience points</summary>
	public uint Experience { get; set; }

	/// <summary>Guild fame/reputation points</summary>
	public int Fame { get; set; }

	/// <summary>Guild funds (Peder)</summary>
	public long Funds { get; set; }

	/// <summary>Maximum member capacity</summary>
	public int MaxMembers { get; set; } = 50;

	/// <summary>Guild members</summary>
	public List<GuildMemberData> Members { get; set; } = new();

	/// <summary>Guild rank definitions</summary>
	public List<GuildRank> Ranks { get; set; } = new()
	{
		new GuildRank { RankId = 0, Name = "Member", Permissions = GuildPermissions.None },
		new GuildRank { RankId = 1, Name = "Officer", Permissions = GuildPermissions.Invite },
		new GuildRank { RankId = 2, Name = "Vice Leader", Permissions = GuildPermissions.Invite | GuildPermissions.Kick | GuildPermissions.EditNotice },
		new GuildRank { RankId = 3, Name = "Leader", Permissions = GuildPermissions.All }
	};
}

/// <summary>
/// Guild member data
/// </summary>
public class GuildMemberData
{
	/// <summary>Character name</summary>
	public string CharacterName { get; set; } = string.Empty;

	/// <summary>Rank ID within the guild</summary>
	public byte RankId { get; set; }

	/// <summary>When the member joined</summary>
	public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

	/// <summary>Last online timestamp</summary>
	public DateTime LastOnline { get; set; } = DateTime.UtcNow;

	/// <summary>Contribution points to the guild</summary>
	public int ContributionPoints { get; set; }

	/// <summary>Member's public note</summary>
	public string Note { get; set; } = string.Empty;

	/// <summary>Officer note (only visible to officers+)</summary>
	public string OfficerNote { get; set; } = string.Empty;
}

/// <summary>
/// Guild rank definition
/// </summary>
public class GuildRank
{
	/// <summary>Rank ID (0-3, 3 = leader)</summary>
	public byte RankId { get; set; }

	/// <summary>Rank display name</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Permissions for this rank</summary>
	public GuildPermissions Permissions { get; set; }
}

/// <summary>
/// Guild permission flags
/// </summary>
[Flags]
public enum GuildPermissions
{
	None = 0,
	Invite = 1,
	Kick = 2,
	EditNotice = 4,
	WithdrawFunds = 8,
	PromoteDemote = 16,
	Disband = 32,
	All = Invite | Kick | EditNotice | WithdrawFunds | PromoteDemote | Disband
}
