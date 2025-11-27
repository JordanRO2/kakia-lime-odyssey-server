using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Guild;

/// <summary>
/// Guild service interface for managing guilds.
/// </summary>
public interface IGuildService
{
	// Guild CRUD operations
	GuildResult CreateGuild(PlayerClient leader, string guildName);
	bool DisbandGuild(PlayerClient leader);
	Guild? GetGuild(PlayerClient player);
	Guild? GetGuildById(uint guildId);
	bool IsInGuild(PlayerClient player);
	bool IsGuildLeader(PlayerClient player);

	// Invitation system
	GuildResult InvitePlayer(PlayerClient inviter, string targetName);
	GuildResult AcceptInvitation(PlayerClient player);
	void DeclineInvitation(PlayerClient player);

	// Join request system
	GuildResult RequestJoin(PlayerClient requester, string guildName);

	// Member management
	bool LeaveGuild(PlayerClient player);
	bool KickMember(PlayerClient leader, uint memberIdx);
	bool ChangeLeader(PlayerClient currentLeader, uint newLeaderIdx);
	bool ChangeOption(PlayerClient leader, GuildOption option);
	bool SetNotice(PlayerClient leader, string notice);

	// Member updates
	void UpdateMemberCombatJobLevel(PlayerClient player, int level);
	void UpdateMemberLifeJobLevel(PlayerClient player, int level);

	// Disconnect/Reconnect handling
	void OnPlayerDisconnect(PlayerClient player);
	void OnPlayerReconnect(PlayerClient player);

	// Guild chat
	void SendGuildChat(PlayerClient sender, string message, uint maintainTime, int type);
}

/// <summary>
/// In-memory guild representation
/// </summary>
public class Guild
{
	public uint Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public uint LeaderId { get; set; }
	public int Fame { get; set; }
	public int Point { get; set; }
	public int Grade { get; set; }
	public GuildOption Option { get; set; } = GuildOption.None;
	public string Notice { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public List<GuildMember> Members { get; } = new();

	public const int MaxMembers = 50;
	public bool IsFull => Members.Count >= MaxMembers;

	public GuildMember? GetMember(PlayerClient player)
	{
		long playerId = player.GetId();
		return Members.FirstOrDefault(m => m.InstId == playerId);
	}

	public GuildMember? GetMember(uint idx)
	{
		return Members.FirstOrDefault(m => m.Index == idx);
	}

	public uint GetNextIndex()
	{
		uint maxIdx = 0;
		foreach (var m in Members)
		{
			if (m.Index > maxIdx)
				maxIdx = m.Index;
		}
		return maxIdx + 1;
	}

	public int GetLoginMemberCount()
	{
		return Members.Count(m => m.IsConnected);
	}

	/// <summary>
	/// Converts to packet GUILD struct
	/// </summary>
	public GUILD ToPacketStruct()
	{
		var guild = new GUILD
		{
			dbid = Id,
			name = new byte[51],
			leader = LeaderId,
			fame = Fame,
			point = Point,
			grade = Grade,
			option = (byte)Option,
			notice = new byte[101],
			since = new DB_TIME
			{
				year = (short)CreatedAt.Year,
				month = (ushort)CreatedAt.Month,
				day = (ushort)CreatedAt.Day,
				hour = (ushort)CreatedAt.Hour,
				minute = (ushort)CreatedAt.Minute,
				second = (ushort)CreatedAt.Second,
				milliSecond = (uint)CreatedAt.Millisecond
			},
			loginMember = GetLoginMemberCount(),
			totalMember = Members.Count
		};

		// Copy name
		var nameBytes = System.Text.Encoding.ASCII.GetBytes(Name);
		Array.Copy(nameBytes, guild.name, Math.Min(nameBytes.Length, 50));

		// Copy notice
		var noticeBytes = System.Text.Encoding.ASCII.GetBytes(Notice);
		Array.Copy(noticeBytes, guild.notice, Math.Min(noticeBytes.Length, 100));

		return guild;
	}
}

/// <summary>
/// In-memory guild member representation
/// </summary>
public class GuildMember
{
	public uint Index { get; set; }
	public string Name { get; set; } = string.Empty;
	public PlayerClient? Player { get; set; }
	public long InstId { get; set; }
	public bool IsConnected { get; set; }
	public byte CombatJobTypeId { get; set; }
	public byte LifeJobTypeId { get; set; }
	public int CombatJobLevel { get; set; }
	public int LifeJobLevel { get; set; }
	public int ContributionPoints { get; set; }
	public GuildMemberType MemberType { get; set; } = GuildMemberType.Member;

	/// <summary>
	/// Converts to packet GUILD_MEMBER struct
	/// </summary>
	public GUILD_MEMBER ToPacketStruct()
	{
		var member = new GUILD_MEMBER
		{
			idx = Index,
			name = new byte[26],
			isConnected = IsConnected,
			instID = InstId,
			state = new GUILD_MEMBER_STATE
			{
				combatJobTypeID = CombatJobTypeId,
				lifeJobTypeID = LifeJobTypeId,
				combatJobLv = CombatJobLevel,
				lifeJobLv = LifeJobLevel,
				point = ContributionPoints,
				memberType = (int)MemberType
			}
		};

		// Copy name
		var nameBytes = System.Text.Encoding.ASCII.GetBytes(Name);
		Array.Copy(nameBytes, member.name, Math.Min(nameBytes.Length, 25));

		return member;
	}
}

/// <summary>
/// Guild member type/rank
/// </summary>
public enum GuildMemberType
{
	Member = 0,
	Officer = 1,
	ViceLeader = 2,
	Leader = 3
}

/// <summary>
/// Guild options
/// </summary>
public enum GuildOption : byte
{
	None = 0,
	OpenRecruitment = 1,
	RequireApproval = 2
}

/// <summary>
/// Result of guild operations
/// </summary>
public class GuildResult
{
	public bool Success { get; set; }
	public Guild? Guild { get; set; }
	public GuildError Error { get; set; }
	public string Message { get; set; } = string.Empty;

	public static GuildResult Ok(Guild guild) => new() { Success = true, Guild = guild };
	public static GuildResult Fail(GuildError error, string message = "") => new() { Success = false, Error = error, Message = message };
}

/// <summary>
/// Guild operation error types
/// </summary>
public enum GuildError
{
	None,
	AlreadyInGuild,
	GuildFull,
	GuildNotFound,
	NotInGuild,
	NotGuildLeader,
	NotEnoughPermission,
	PlayerNotFound,
	PlayerAlreadyInGuild,
	NoPendingInvitation,
	InvitationExpired,
	GuildNameTooLong,
	GuildNameTaken,
	CannotInviteSelf,
	CannotKickLeader,
	InvalidMemberIndex,
	NoticeTooLong
}

/// <summary>
/// Pending guild invitation
/// </summary>
public class GuildInvitation
{
	public uint GuildId { get; set; }
	public string GuildName { get; set; } = string.Empty;
	public string InviterName { get; set; } = string.Empty;
	public DateTime SentAt { get; set; }

	public const int InvitationTimeoutSeconds = 60;
	public bool IsExpired => (DateTime.Now - SentAt).TotalSeconds > InvitationTimeoutSeconds;
}
