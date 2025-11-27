/// <summary>
/// Service for managing guilds with MongoDB persistence.
/// </summary>
/// <remarks>
/// Uses: MongoDBService for guild persistence
/// Handles: Create, disband, invite, join, leave, kick, leader change, options
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Models.Persistence;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Guild;

/// <summary>
/// Service for managing guilds with MongoDB persistence.
/// </summary>
public class GuildService : IGuildService
{
	private static uint _nextGuildId = 1;
	private readonly ConcurrentDictionary<uint, Guild> _guilds = new();
	private readonly ConcurrentDictionary<long, uint> _playerGuildMap = new(); // Player instance ID -> Guild ID
	private readonly ConcurrentDictionary<long, GuildInvitation> _pendingInvitations = new(); // Target player ID -> Invitation
	private bool _initialized;

	/// <summary>
	/// Initializes the guild service by loading all guilds from the database.
	/// </summary>
	public void Initialize()
	{
		if (_initialized) return;

		var guilds = MongoDBService.Instance.LoadAllGuilds();
		foreach (var guildData in guilds)
		{
			var guild = GuildFromData(guildData);
			_guilds[guild.Id] = guild;

			if (guild.Id >= _nextGuildId)
				_nextGuildId = guild.Id + 1;
		}

		Logger.Log($"[GUILD] Loaded {guilds.Count} guilds from database", LogLevel.Information);
		_initialized = true;
	}

	/// <summary>
	/// Converts GuildData (persistence) to Guild (runtime).
	/// </summary>
	private static Guild GuildFromData(GuildData data)
	{
		var guild = new Guild
		{
			Id = (uint)data.GuildId,
			Name = data.Name,
			LeaderId = 0,
			Fame = data.Fame,
			Point = 0,
			Grade = data.Level,
			Option = GuildOption.None,
			Notice = data.Notice,
			CreatedAt = data.CreatedAt
		};

		uint idx = 0;
		foreach (var memberData in data.Members)
		{
			var member = new GuildMember
			{
				Index = idx,
				Name = memberData.CharacterName,
				Player = null, // Will be set on login
				InstId = 0,
				IsConnected = false,
				CombatJobTypeId = 0,
				LifeJobTypeId = 0,
				CombatJobLevel = 1,
				LifeJobLevel = 1,
				ContributionPoints = memberData.ContributionPoints,
				MemberType = (GuildMemberType)memberData.RankId
			};

			if (memberData.CharacterName == data.LeaderName)
			{
				guild.LeaderId = idx;
				member.MemberType = GuildMemberType.Leader;
			}

			guild.Members.Add(member);
			idx++;
		}

		return guild;
	}

	/// <summary>
	/// Converts Guild (runtime) to GuildData (persistence).
	/// </summary>
	private static GuildData GuildToData(Guild guild)
	{
		var leader = guild.Members.FirstOrDefault(m => m.MemberType == GuildMemberType.Leader);

		var data = new GuildData
		{
			GuildId = (int)guild.Id,
			Name = guild.Name,
			LeaderName = leader?.Name ?? string.Empty,
			Notice = guild.Notice,
			CreatedAt = guild.CreatedAt,
			Level = (byte)guild.Grade,
			Experience = 0,
			Fame = guild.Fame,
			Funds = 0,
			MaxMembers = 50
		};

		foreach (var member in guild.Members)
		{
			data.Members.Add(new GuildMemberData
			{
				CharacterName = member.Name,
				RankId = (byte)member.MemberType,
				JoinedAt = DateTime.UtcNow,
				LastOnline = member.IsConnected ? DateTime.UtcNow : DateTime.MinValue,
				ContributionPoints = member.ContributionPoints
			});
		}

		return data;
	}

	/// <summary>
	/// Saves a guild to the database.
	/// </summary>
	private void SaveGuild(Guild guild)
	{
		var data = GuildToData(guild);
		MongoDBService.Instance.SaveGuild(data);
	}

	/// <summary>
	/// Deletes a guild from the database.
	/// </summary>
	private void DeleteGuildFromDb(uint guildId)
	{
		MongoDBService.Instance.DeleteGuild((int)guildId);
	}

	/// <summary>
	/// Creates a new guild with the given player as leader.
	/// </summary>
	public GuildResult CreateGuild(PlayerClient leader, string guildName)
	{
		var character = leader.GetCurrentCharacter();
		if (character == null)
			return GuildResult.Fail(GuildError.PlayerNotFound, "Character not found");

		long playerId = leader.GetId();

		// Check if already in guild
		if (_playerGuildMap.ContainsKey(playerId))
			return GuildResult.Fail(GuildError.AlreadyInGuild, "Already in a guild");

		// Validate guild name
		if (string.IsNullOrWhiteSpace(guildName))
			return GuildResult.Fail(GuildError.GuildNameTooLong, "Guild name cannot be empty");

		if (guildName.Length > 50)
			return GuildResult.Fail(GuildError.GuildNameTooLong, "Guild name too long (max 50 chars)");

		// Check if guild name is taken
		if (_guilds.Values.Any(g => g.Name.Equals(guildName, StringComparison.OrdinalIgnoreCase)))
			return GuildResult.Fail(GuildError.GuildNameTaken, "Guild name already taken");

		// Create guild
		uint guildId = Interlocked.Increment(ref _nextGuildId);

		var guild = new Guild
		{
			Id = guildId,
			Name = guildName,
			LeaderId = 0, // Will be set to first member's idx
			Fame = 0,
			Point = 0,
			Grade = 1,
			Option = GuildOption.None,
			Notice = string.Empty,
			CreatedAt = DateTime.Now
		};

		var member = new GuildMember
		{
			Index = 0,
			Name = character.appearance.name,
			Player = leader,
			InstId = playerId,
			IsConnected = true,
			CombatJobTypeId = character.status.combatJob.typeID,
			LifeJobTypeId = character.status.lifeJob.typeID,
			CombatJobLevel = character.status.combatJob.lv,
			LifeJobLevel = character.status.lifeJob.lv,
			ContributionPoints = 0,
			MemberType = GuildMemberType.Leader
		};

		guild.LeaderId = member.Index;
		guild.Members.Add(member);

		_guilds[guildId] = guild;
		_playerGuildMap[playerId] = guildId;

		// Save to database
		SaveGuild(guild);

		// Send SC_GUILD_CREATED to leader
		SendGuildCreated(leader);

		// Send SC_GUILD_INFO to leader with initial guild info
		SendGuildInfo(leader, guild, member);

		Logger.Log($"[GUILD] {character.appearance.name} created guild '{guildName}' (ID: {guildId})", LogLevel.Information);

		return GuildResult.Ok(guild);
	}

	/// <summary>
	/// Disbands a guild (leader only).
	/// </summary>
	public bool DisbandGuild(PlayerClient leader)
	{
		var guild = GetGuild(leader);
		if (guild == null)
			return false;

		var member = guild.GetMember(leader);
		if (member == null || member.MemberType != GuildMemberType.Leader)
			return false;

		// Send SC_GUILD_DISBANDED to all members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildDisbanded(m.Player!);
			_playerGuildMap.TryRemove(m.InstId, out _);
		}

		_guilds.TryRemove(guild.Id, out _);

		// Delete from database
		DeleteGuildFromDb(guild.Id);

		var character = leader.GetCurrentCharacter();
		Logger.Log($"[GUILD] {character?.appearance.name} disbanded guild '{guild.Name}'", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Sends an invitation to a player.
	/// </summary>
	public GuildResult InvitePlayer(PlayerClient inviter, string targetName)
	{
		var inviterChar = inviter.GetCurrentCharacter();
		if (inviterChar == null)
			return GuildResult.Fail(GuildError.PlayerNotFound);

		// Cannot invite yourself
		if (inviterChar.appearance.name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
			return GuildResult.Fail(GuildError.CannotInviteSelf, "Cannot invite yourself");

		// Get guild
		var guild = GetGuild(inviter);
		if (guild == null)
			return GuildResult.Fail(GuildError.NotInGuild, "You are not in a guild");

		// Check if inviter has permission
		var inviterMember = guild.GetMember(inviter);
		if (inviterMember == null)
			return GuildResult.Fail(GuildError.NotInGuild);

		if (inviterMember.MemberType < GuildMemberType.Officer)
			return GuildResult.Fail(GuildError.NotEnoughPermission, "Only officers or higher can invite");

		// Check if guild is full
		if (guild.IsFull)
			return GuildResult.Fail(GuildError.GuildFull, "Guild is full");

		// Find target player
		var target = LimeServer.PlayerClients.FirstOrDefault(p =>
		{
			var c = p.GetCurrentCharacter();
			return c != null && c.appearance.name.Equals(targetName, StringComparison.OrdinalIgnoreCase);
		});

		if (target == null)
			return GuildResult.Fail(GuildError.PlayerNotFound, $"Player '{targetName}' not found");

		long targetId = target.GetId();

		// Check if target is already in a guild
		if (_playerGuildMap.ContainsKey(targetId))
			return GuildResult.Fail(GuildError.PlayerAlreadyInGuild, "Player is already in a guild");

		// Create invitation
		var invitation = new GuildInvitation
		{
			GuildId = guild.Id,
			GuildName = guild.Name,
			InviterName = inviterChar.appearance.name,
			SentAt = DateTime.Now
		};

		_pendingInvitations[targetId] = invitation;

		// Send SC_GUILD_INVITED to target
		SendGuildInvited(target, inviterChar.appearance.name, guild.Name);

		Logger.Log($"[GUILD] {inviterChar.appearance.name} invited {targetName} to guild '{guild.Name}'", LogLevel.Debug);

		return GuildResult.Ok(guild);
	}

	/// <summary>
	/// Accepts a pending guild invitation.
	/// </summary>
	public GuildResult AcceptInvitation(PlayerClient player)
	{
		long playerId = player.GetId();

		if (!_pendingInvitations.TryRemove(playerId, out var invitation))
			return GuildResult.Fail(GuildError.NoPendingInvitation, "No pending invitation");

		if (invitation.IsExpired)
			return GuildResult.Fail(GuildError.InvitationExpired, "Invitation expired");

		if (!_guilds.TryGetValue(invitation.GuildId, out var guild))
			return GuildResult.Fail(GuildError.GuildFull, "Guild no longer exists");

		if (guild.IsFull)
			return GuildResult.Fail(GuildError.GuildFull, "Guild is full");

		var character = player.GetCurrentCharacter();
		if (character == null)
			return GuildResult.Fail(GuildError.PlayerNotFound);

		uint newIndex = guild.GetNextIndex();

		var member = new GuildMember
		{
			Index = newIndex,
			Name = character.appearance.name,
			Player = player,
			InstId = playerId,
			IsConnected = true,
			CombatJobTypeId = character.status.combatJob.typeID,
			LifeJobTypeId = character.status.lifeJob.typeID,
			CombatJobLevel = character.status.combatJob.lv,
			LifeJobLevel = character.status.lifeJob.lv,
			ContributionPoints = 0,
			MemberType = GuildMemberType.Member
		};

		guild.Members.Add(member);
		_playerGuildMap[playerId] = guild.Id;

		// Save to database
		SaveGuild(guild);

		// Send SC_GUILD_MEMBER_ADDED to existing members
		foreach (var m in guild.Members.Where(m => m.Index != newIndex && m.IsConnected && m.Player != null))
		{
			SendGuildMemberAdded(m.Player!, member, guild.GetLoginMemberCount(), guild.Members.Count);
		}

		// Send SC_GUILD_INFO to new member with all guild info
		SendGuildInfo(player, guild, member);

		Logger.Log($"[GUILD] {character.appearance.name} joined guild '{guild.Name}'", LogLevel.Information);

		return GuildResult.Ok(guild);
	}

	/// <summary>
	/// Declines a pending guild invitation.
	/// </summary>
	public void DeclineInvitation(PlayerClient player)
	{
		_pendingInvitations.TryRemove(player.GetId(), out _);
	}

	/// <summary>
	/// Requests to join a guild by name.
	/// </summary>
	public GuildResult RequestJoin(PlayerClient requester, string guildName)
	{
		var requesterChar = requester.GetCurrentCharacter();
		if (requesterChar == null)
			return GuildResult.Fail(GuildError.PlayerNotFound);

		// Check if requester is already in a guild
		if (_playerGuildMap.ContainsKey(requester.GetId()))
			return GuildResult.Fail(GuildError.AlreadyInGuild, "You are already in a guild");

		// Find guild by name
		var guild = _guilds.Values.FirstOrDefault(g =>
			g.Name.Equals(guildName, StringComparison.OrdinalIgnoreCase));

		if (guild == null)
			return GuildResult.Fail(GuildError.GuildNotFound, $"Guild '{guildName}' not found");

		// Check if guild accepts join requests
		if (guild.Option == GuildOption.None)
			return GuildResult.Fail(GuildError.NotEnoughPermission, "Guild is not accepting join requests");

		// Check if guild is full
		if (guild.IsFull)
			return GuildResult.Fail(GuildError.GuildFull, "Guild is full");

		// If open recruitment, auto-join
		if (guild.Option == GuildOption.OpenRecruitment)
		{
			return JoinGuildDirectly(requester, guild);
		}

		// If requires approval, notify guild leader
		var leader = guild.Members.FirstOrDefault(m => m.MemberType == GuildMemberType.Leader);
		if (leader != null && leader.IsConnected && leader.Player != null)
		{
			SendJoinRequest(leader.Player, requesterChar.appearance.name, guild.Name);
		}

		Logger.Log($"[GUILD] {requesterChar.appearance.name} requested to join guild '{guild.Name}'", LogLevel.Debug);

		return GuildResult.Ok(guild);
	}

	/// <summary>
	/// Joins a guild directly (for open recruitment).
	/// </summary>
	private GuildResult JoinGuildDirectly(PlayerClient player, Guild guild)
	{
		var character = player.GetCurrentCharacter();
		if (character == null)
			return GuildResult.Fail(GuildError.PlayerNotFound);

		long playerId = player.GetId();
		uint newIndex = guild.GetNextIndex();

		var member = new GuildMember
		{
			Index = newIndex,
			Name = character.appearance.name,
			Player = player,
			InstId = playerId,
			IsConnected = true,
			CombatJobTypeId = character.status.combatJob.typeID,
			LifeJobTypeId = character.status.lifeJob.typeID,
			CombatJobLevel = character.status.combatJob.lv,
			LifeJobLevel = character.status.lifeJob.lv,
			ContributionPoints = 0,
			MemberType = GuildMemberType.Member
		};

		guild.Members.Add(member);
		_playerGuildMap[playerId] = guild.Id;

		// Save to database
		SaveGuild(guild);

		// Send SC_GUILD_MEMBER_ADDED to existing members
		foreach (var m in guild.Members.Where(m => m.Index != newIndex && m.IsConnected && m.Player != null))
		{
			SendGuildMemberAdded(m.Player!, member, guild.GetLoginMemberCount(), guild.Members.Count);
		}

		// Send SC_GUILD_INFO to new member with all guild info
		SendGuildInfo(player, guild, member);

		Logger.Log($"[GUILD] {character.appearance.name} joined guild '{guild.Name}' via open recruitment", LogLevel.Information);

		return GuildResult.Ok(guild);
	}

	/// <summary>
	/// Sends join request notification to guild leader.
	/// </summary>
	private static void SendJoinRequest(PlayerClient leader, string requesterName, string guildName)
	{
		// Use SC_GUILD_INVITED to notify leader of join request
		// The leader can then use invite to accept
		var packet = new SC_GUILD_INVITED
		{
			pcName = new byte[26],
			guildName = new byte[51]
		};

		var nameBytes = System.Text.Encoding.ASCII.GetBytes(requesterName);
		Array.Copy(nameBytes, packet.pcName, Math.Min(nameBytes.Length, 25));

		var guildBytes = System.Text.Encoding.ASCII.GetBytes(guildName);
		Array.Copy(guildBytes, packet.guildName, Math.Min(guildBytes.Length, 50));

		using PacketWriter pw = new();
		pw.Write(packet);
		leader.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Player voluntarily leaves guild.
	/// </summary>
	public bool LeaveGuild(PlayerClient player)
	{
		var guild = GetGuild(player);
		if (guild == null)
			return false;

		var member = guild.GetMember(player);
		if (member == null)
			return false;

		// Leader cannot leave, must transfer leadership or disband
		if (member.MemberType == GuildMemberType.Leader)
			return false;

		long playerId = player.GetId();

		// Remove member
		guild.Members.Remove(member);
		_playerGuildMap.TryRemove(playerId, out _);

		// Save to database
		SaveGuild(guild);

		// Send SC_GUILD_SECEDED to leaving player
		SendGuildSeceded(player, member.Index, guild.GetLoginMemberCount(), guild.Members.Count);

		// Notify remaining members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildSeceded(m.Player!, member.Index, guild.GetLoginMemberCount(), guild.Members.Count);
		}

		var character = player.GetCurrentCharacter();
		Logger.Log($"[GUILD] {character?.appearance.name} left guild '{guild.Name}'", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Kicks a member from the guild (leader/officers only).
	/// </summary>
	public bool KickMember(PlayerClient leader, uint memberIdx)
	{
		var guild = GetGuild(leader);
		if (guild == null)
			return false;

		var leaderMember = guild.GetMember(leader);
		if (leaderMember == null || leaderMember.MemberType < GuildMemberType.Officer)
			return false;

		var targetMember = guild.GetMember(memberIdx);
		if (targetMember == null)
			return false;

		// Cannot kick leader
		if (targetMember.MemberType == GuildMemberType.Leader)
			return false;

		// Officers can only kick members
		if (leaderMember.MemberType == GuildMemberType.Officer && targetMember.MemberType >= GuildMemberType.Officer)
			return false;

		// Vice leaders can only kick officers and members
		if (leaderMember.MemberType == GuildMemberType.ViceLeader && targetMember.MemberType >= GuildMemberType.ViceLeader)
			return false;

		// Remove member
		guild.Members.Remove(targetMember);
		_playerGuildMap.TryRemove(targetMember.InstId, out _);

		// Save to database
		SaveGuild(guild);

		// Notify kicked player
		if (targetMember.IsConnected && targetMember.Player != null)
		{
			SendGuildSeceded(targetMember.Player, memberIdx, guild.GetLoginMemberCount(), guild.Members.Count);
		}

		// Notify remaining members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildSeceded(m.Player!, memberIdx, guild.GetLoginMemberCount(), guild.Members.Count);
		}

		var leaderChar = leader.GetCurrentCharacter();
		Logger.Log($"[GUILD] {leaderChar?.appearance.name} kicked {targetMember.Name} from guild", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Changes the guild leader.
	/// </summary>
	public bool ChangeLeader(PlayerClient currentLeader, uint newLeaderIdx)
	{
		var guild = GetGuild(currentLeader);
		if (guild == null)
			return false;

		var leaderMember = guild.GetMember(currentLeader);
		if (leaderMember == null || leaderMember.MemberType != GuildMemberType.Leader)
			return false;

		var newLeader = guild.GetMember(newLeaderIdx);
		if (newLeader == null)
			return false;

		// Swap roles
		leaderMember.MemberType = GuildMemberType.Member;
		newLeader.MemberType = GuildMemberType.Leader;
		guild.LeaderId = newLeaderIdx;

		// Save to database
		SaveGuild(guild);

		// Notify all members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildChangedLeader(m.Player!, newLeaderIdx);
		}

		Logger.Log($"[GUILD] {newLeader.Name} is now leader of '{guild.Name}'", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Changes guild options.
	/// </summary>
	public bool ChangeOption(PlayerClient leader, GuildOption option)
	{
		var guild = GetGuild(leader);
		if (guild == null)
			return false;

		var leaderMember = guild.GetMember(leader);
		if (leaderMember == null || leaderMember.MemberType < GuildMemberType.ViceLeader)
			return false;

		guild.Option = option;

		// Save to database
		SaveGuild(guild);

		// Notify all members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildChangedOption(m.Player!, option);
		}

		return true;
	}

	/// <summary>
	/// Sets guild notice (officers+ only).
	/// </summary>
	public bool SetNotice(PlayerClient leader, string notice)
	{
		var guild = GetGuild(leader);
		if (guild == null)
			return false;

		var leaderMember = guild.GetMember(leader);
		if (leaderMember == null || leaderMember.MemberType < GuildMemberType.Officer)
			return false;

		if (notice.Length > 100)
			return false;

		guild.Notice = notice;

		// Save to database
		SaveGuild(guild);

		// Notify all members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildNotice(m.Player!, notice);
		}

		Logger.Log($"[GUILD] {leaderMember.Name} updated notice for '{guild.Name}'", LogLevel.Debug);

		return true;
	}

	/// <summary>
	/// Gets the guild a player is in.
	/// </summary>
	public Guild? GetGuild(PlayerClient player)
	{
		long playerId = player.GetId();
		if (_playerGuildMap.TryGetValue(playerId, out uint guildId))
		{
			_guilds.TryGetValue(guildId, out var guild);
			return guild;
		}
		return null;
	}

	/// <summary>
	/// Gets a guild by its ID.
	/// </summary>
	public Guild? GetGuildById(uint guildId)
	{
		_guilds.TryGetValue(guildId, out var guild);
		return guild;
	}

	/// <summary>
	/// Checks if a player is in a guild.
	/// </summary>
	public bool IsInGuild(PlayerClient player)
	{
		return _playerGuildMap.ContainsKey(player.GetId());
	}

	/// <summary>
	/// Checks if a player is a guild leader.
	/// </summary>
	public bool IsGuildLeader(PlayerClient player)
	{
		var guild = GetGuild(player);
		if (guild == null) return false;

		var member = guild.GetMember(player);
		return member != null && member.MemberType == GuildMemberType.Leader;
	}

	/// <summary>
	/// Updates a member's combat job level.
	/// </summary>
	public void UpdateMemberCombatJobLevel(PlayerClient player, int level)
	{
		var guild = GetGuild(player);
		if (guild == null) return;

		var member = guild.GetMember(player);
		if (member == null) return;

		member.CombatJobLevel = level;

		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendGuildUpdateMemberCombatJobLv(m.Player!, member.Index, level);
		}
	}

	/// <summary>
	/// Updates a member's life job level.
	/// </summary>
	public void UpdateMemberLifeJobLevel(PlayerClient player, int level)
	{
		var guild = GetGuild(player);
		if (guild == null) return;

		var member = guild.GetMember(player);
		if (member == null) return;

		member.LifeJobLevel = level;

		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendGuildUpdateMemberLifeJobLv(m.Player!, member.Index, level);
		}
	}

	/// <summary>
	/// Handles player disconnection.
	/// </summary>
	public void OnPlayerDisconnect(PlayerClient player)
	{
		var guild = GetGuild(player);
		if (guild == null) return;

		var member = guild.GetMember(player);
		if (member == null) return;

		member.IsConnected = false;
		member.Player = null;

		// Notify other members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildMemberDisconnected(m.Player!, member.Index, guild.GetLoginMemberCount(), guild.Members.Count);
		}

		Logger.Log($"[GUILD] {member.Name} disconnected from guild '{guild.Name}'", LogLevel.Debug);
	}

	/// <summary>
	/// Handles player reconnection.
	/// </summary>
	public void OnPlayerReconnect(PlayerClient player)
	{
		long playerId = player.GetId();
		if (!_playerGuildMap.TryGetValue(playerId, out uint guildId))
			return;

		if (!_guilds.TryGetValue(guildId, out var guild))
			return;

		var member = guild.Members.FirstOrDefault(m => m.InstId == playerId);
		if (member == null) return;

		member.IsConnected = true;
		member.Player = player;

		// Update character info
		var character = player.GetCurrentCharacter();
		if (character != null)
		{
			member.CombatJobTypeId = character.status.combatJob.typeID;
			member.LifeJobTypeId = character.status.lifeJob.typeID;
			member.CombatJobLevel = character.status.combatJob.lv;
			member.LifeJobLevel = character.status.lifeJob.lv;
		}

		// Send guild info to reconnecting player
		SendGuildInfo(player, guild, member);

		// Notify other members
		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendGuildMemberConnected(m.Player!, member, guild.GetLoginMemberCount(), guild.Members.Count);
		}

		Logger.Log($"[GUILD] {member.Name} reconnected to guild '{guild.Name}'", LogLevel.Debug);
	}

	/// <summary>
	/// Sends a chat message to guild members.
	/// </summary>
	public void SendGuildChat(PlayerClient sender, string message, uint maintainTime, int type)
	{
		var guild = GetGuild(sender);
		if (guild == null) return;

		var member = guild.GetMember(sender);
		if (member == null) return;

		foreach (var m in guild.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendGuildSay(m.Player!, member.Index, message, maintainTime, type);
		}
	}

	#region Packet Sending

	private static void SendGuildCreated(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_CREATED());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildDisbanded(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_DISBANDED());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildInfo(PlayerClient player, Guild guild, GuildMember self)
	{
		using PacketWriter pw = new();

		// Write header with myID and guild info
		var header = new SC_GUILD_INFO
		{
			myID = self.Index,
			guildInfo = guild.ToPacketStruct()
		};
		pw.Write(header);

		// Write member count and members
		pw.Write((byte)guild.Members.Count);
		foreach (var member in guild.Members)
		{
			pw.Write(PacketConverter.AsBytes(member.ToPacketStruct()));
		}

		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	private static void SendGuildInvited(PlayerClient player, string inviterName, string guildName)
	{
		var packet = new SC_GUILD_INVITED
		{
			pcName = new byte[26],
			guildName = new byte[51]
		};

		var inviterBytes = System.Text.Encoding.ASCII.GetBytes(inviterName);
		Array.Copy(inviterBytes, packet.pcName, Math.Min(inviterBytes.Length, 25));

		var guildBytes = System.Text.Encoding.ASCII.GetBytes(guildName);
		Array.Copy(guildBytes, packet.guildName, Math.Min(guildBytes.Length, 50));

		using PacketWriter pw = new();
		pw.Write(packet);
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildMemberAdded(PlayerClient player, GuildMember member, int loginMember, int totalMember)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_MEMBER_ADDED
		{
			member = member.ToPacketStruct(),
			loginMember = loginMember,
			totalMember = totalMember
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildSeceded(PlayerClient player, uint memberIdx, int loginMember, int totalMember)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_SECEDED
		{
			idx = memberIdx,
			loginMember = loginMember,
			totalMember = totalMember
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildChangedLeader(PlayerClient player, uint newLeaderIdx)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_CHANGED_LEADER
		{
			idx = newLeaderIdx
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildChangedOption(PlayerClient player, GuildOption option)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_CHANGED_OPTION
		{
			type = (byte)option
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildNotice(PlayerClient player, string notice)
	{
		var packet = new SC_GUILD_NOTICE
		{
			notice = new byte[101]
		};

		var noticeBytes = System.Text.Encoding.ASCII.GetBytes(notice);
		Array.Copy(noticeBytes, packet.notice, Math.Min(noticeBytes.Length, 100));

		using PacketWriter pw = new();
		pw.Write(packet);
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildMemberDisconnected(PlayerClient player, uint memberIdx, int loginMember, int totalMember)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_MEMBER_DISCONNECTED
		{
			idx = memberIdx,
			loginMember = loginMember,
			totalMember = totalMember
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildMemberConnected(PlayerClient player, GuildMember member, int loginMember, int totalMember)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_MEMBER_CONNECTED
		{
			idx = member.Index,
			loginMember = loginMember,
			totalMember = totalMember
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildSay(PlayerClient player, uint senderIdx, string message, uint maintainTime, int type)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_SAY
		{
			idx = senderIdx,
			maintainTime = maintainTime,
			type = type
		});
		// Write message as variable-length string
		pw.Write(System.Text.Encoding.ASCII.GetBytes(message));
		pw.Write((byte)0); // Null terminator
		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	private static void SendGuildUpdateMemberCombatJobLv(PlayerClient player, uint memberIdx, int level)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_UPDATE_MEMBER_COMBAT_JOB_LV
		{
			idx = memberIdx,
			lv = level
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendGuildUpdateMemberLifeJobLv(PlayerClient player, uint memberIdx, int level)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_GUILD_UPDATE_MEMBER_LIFE_JOB_LV
		{
			idx = memberIdx,
			lv = level
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	#endregion
}
