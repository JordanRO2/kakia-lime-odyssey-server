using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Party;

// DEF is from kakia_lime_odyssey_packets.Packets.Models

/// <summary>
/// Interface for party management service.
/// </summary>
public interface IPartyService
{
	/// <summary>
	/// Creates a new party with the given player as leader.
	/// </summary>
	/// <param name="leader">The player creating the party</param>
	/// <param name="partyName">Name for the party</param>
	/// <returns>Result of party creation</returns>
	PartyResult CreateParty(PlayerClient leader, string partyName);

	/// <summary>
	/// Disbands a party (leader only).
	/// </summary>
	/// <param name="leader">The party leader</param>
	/// <returns>True if successfully disbanded</returns>
	bool DisbandParty(PlayerClient leader);

	/// <summary>
	/// Sends an invitation to a player.
	/// </summary>
	/// <param name="inviter">Player sending the invitation</param>
	/// <param name="targetName">Name of player to invite</param>
	/// <returns>Result of invitation</returns>
	PartyResult InvitePlayer(PlayerClient inviter, string targetName);

	/// <summary>
	/// Accepts a pending party invitation.
	/// </summary>
	/// <param name="player">Player accepting invitation</param>
	/// <returns>Result of joining</returns>
	PartyResult AcceptInvitation(PlayerClient player);

	/// <summary>
	/// Declines a pending party invitation.
	/// </summary>
	/// <param name="player">Player declining invitation</param>
	void DeclineInvitation(PlayerClient player);

	/// <summary>
	/// Accepts a pending join request (leader approves player joining).
	/// </summary>
	/// <param name="leader">Party leader accepting the request</param>
	/// <returns>Result of accepting the join request</returns>
	PartyResult AcceptJoinRequest(PlayerClient leader);

	/// <summary>
	/// Declines a pending join request.
	/// </summary>
	/// <param name="leader">Party leader declining the request</param>
	void DeclineJoinRequest(PlayerClient leader);

	/// <summary>
	/// Player voluntarily leaves party.
	/// </summary>
	/// <param name="player">Player leaving</param>
	/// <returns>True if successfully left</returns>
	bool LeaveParty(PlayerClient player);

	/// <summary>
	/// Kicks a member from the party (leader only).
	/// </summary>
	/// <param name="leader">Party leader</param>
	/// <param name="memberIdx">Index of member to kick</param>
	/// <returns>True if successfully kicked</returns>
	bool KickMember(PlayerClient leader, uint memberIdx);

	/// <summary>
	/// Changes the party leader.
	/// </summary>
	/// <param name="currentLeader">Current party leader</param>
	/// <param name="newLeaderIdx">Index of new leader</param>
	/// <returns>True if successfully changed</returns>
	bool ChangeLeader(PlayerClient currentLeader, uint newLeaderIdx);

	/// <summary>
	/// Changes party options (loot distribution, etc.).
	/// </summary>
	/// <param name="leader">Party leader</param>
	/// <param name="option">New party option</param>
	/// <returns>True if successfully changed</returns>
	bool ChangeOption(PlayerClient leader, PartyOption option);

	/// <summary>
	/// Gets the party a player is in.
	/// </summary>
	/// <param name="player">Player to check</param>
	/// <returns>Party or null if not in party</returns>
	Party? GetParty(PlayerClient player);

	/// <summary>
	/// Gets a party by its ID.
	/// </summary>
	/// <param name="partyId">Party ID</param>
	/// <returns>Party or null</returns>
	Party? GetPartyById(uint partyId);

	/// <summary>
	/// Checks if a player is in a party.
	/// </summary>
	/// <param name="player">Player to check</param>
	/// <returns>True if in party</returns>
	bool IsInParty(PlayerClient player);

	/// <summary>
	/// Checks if a player is a party leader.
	/// </summary>
	/// <param name="player">Player to check</param>
	/// <returns>True if party leader</returns>
	bool IsPartyLeader(PlayerClient player);

	/// <summary>
	/// Updates a party member's HP and broadcasts to party.
	/// </summary>
	void UpdateMemberHP(PlayerClient player, int hp, int maxHp);

	/// <summary>
	/// Updates a party member's MP and broadcasts to party.
	/// </summary>
	void UpdateMemberMP(PlayerClient player, int mp, int maxMp);

	/// <summary>
	/// Updates a party member's position and broadcasts to party.
	/// </summary>
	void UpdateMemberPosition(PlayerClient player, float x, float y, float z, uint zoneId);

	/// <summary>
	/// Handles player disconnection.
	/// </summary>
	/// <param name="player">Disconnecting player</param>
	void OnPlayerDisconnect(PlayerClient player);

	/// <summary>
	/// Handles player reconnection.
	/// </summary>
	/// <param name="player">Reconnecting player</param>
	void OnPlayerReconnect(PlayerClient player);

	/// <summary>
	/// Sends a chat message to party members.
	/// </summary>
	/// <param name="sender">Message sender</param>
	/// <param name="message">Message text</param>
	/// <param name="maintainTime">How long to display message</param>
	/// <param name="type">Chat type</param>
	void SendPartyChat(PlayerClient sender, string message, uint maintainTime, int type);

	/// <summary>
	/// Notifies party when a member loots an item.
	/// </summary>
	void NotifyMemberLootedItem(PlayerClient player, int itemTypeId, long count);

	/// <summary>
	/// Notifies party when a member gains a buff.
	/// </summary>
	void NotifyMemberAddedBuff(PlayerClient player, DEF def);

	/// <summary>
	/// Notifies party when a member loses a buff.
	/// </summary>
	void NotifyMemberRemovedBuff(PlayerClient player, uint defInstId);
}

/// <summary>
/// Represents a party.
/// </summary>
public class Party
{
	/// <summary>Unique party ID</summary>
	public uint Id { get; set; }

	/// <summary>Party name (max 40 chars)</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Index of the party leader</summary>
	public uint LeaderIndex { get; set; }

	/// <summary>Party options (loot distribution, etc.)</summary>
	public PartyOption Option { get; set; } = PartyOption.FreeForAll;

	/// <summary>Party members</summary>
	public List<PartyMember> Members { get; set; } = new();

	/// <summary>Maximum party size</summary>
	public const int MaxMembers = 6;

	/// <summary>When the party was created</summary>
	public DateTime CreatedAt { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets the leader player client.
	/// </summary>
	public PlayerClient? GetLeader()
	{
		var leader = Members.FirstOrDefault(m => m.Index == LeaderIndex);
		return leader?.Player;
	}

	/// <summary>
	/// Gets a member by index.
	/// </summary>
	public PartyMember? GetMember(uint index)
	{
		return Members.FirstOrDefault(m => m.Index == index);
	}

	/// <summary>
	/// Gets a member by player client.
	/// </summary>
	public PartyMember? GetMember(PlayerClient player)
	{
		return Members.FirstOrDefault(m => m.Player == player);
	}

	/// <summary>
	/// Gets the next available member index.
	/// </summary>
	public uint GetNextIndex()
	{
		if (Members.Count == 0) return 0;
		return Members.Max(m => m.Index) + 1;
	}

	/// <summary>
	/// Checks if party is full.
	/// </summary>
	public bool IsFull => Members.Count >= MaxMembers;
}

/// <summary>
/// Represents a party member.
/// </summary>
public class PartyMember
{
	/// <summary>Member index in party</summary>
	public uint Index { get; set; }

	/// <summary>Character name</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Player client reference</summary>
	public PlayerClient? Player { get; set; }

	/// <summary>Character instance ID</summary>
	public long InstId { get; set; }

	/// <summary>Whether the player is connected</summary>
	public bool IsConnected { get; set; } = true;

	/// <summary>Current zone ID</summary>
	public uint ZoneId { get; set; }

	/// <summary>Last known position X</summary>
	public float PosX { get; set; }

	/// <summary>Last known position Y</summary>
	public float PosY { get; set; }

	/// <summary>Last known position Z</summary>
	public float PosZ { get; set; }

	/// <summary>
	/// Converts to PARTY_MEMBER packet structure.
	/// </summary>
	public PARTY_MEMBER ToPacketStruct()
	{
		var character = Player?.GetCurrentCharacter();
		var status = Player?.GetEntityStatus();

		return new PARTY_MEMBER
		{
			idx = Index,
			name = System.Text.Encoding.ASCII.GetBytes(Name),
			isConnected = IsConnected,
			instID = InstId,
			state = new PARTY_MEMBER_STATE
			{
				combatJobTypeID = (sbyte)(character?.status.combatJob.typeID ?? 0),
				lifeJobTypeID = (sbyte)(character?.status.lifeJob.typeID ?? 0),
				playingJob = character?.appearance.playingJobClass ?? 0,
				combatJobLv = character?.status.combatJob.lv ?? 1,
				lifeJobLv = character?.status.lifeJob.lv ?? 1,
				hp = (int)(status?.BasicStatus.Hp ?? 0),
				mhp = (int)(status?.BasicStatus.MaxHp ?? 0),
				mp = (int)(status?.BasicStatus.Mp ?? 0),
				mmp = (int)(status?.BasicStatus.MaxMp ?? 0),
				lp = (int)(status?.Lp ?? 0),
				mlp = (int)(status?.MaxLp ?? 0),
				pos = new POS { x = (int)PosX, y = (int)PosY, z = PosZ },
				zoneID = ZoneId
			}
		};
	}
}

/// <summary>
/// Party loot distribution options.
/// </summary>
public enum PartyOption : byte
{
	/// <summary>Anyone can loot</summary>
	FreeForAll = 0,

	/// <summary>Round robin distribution</summary>
	RoundRobin = 1,

	/// <summary>Leader distributes loot</summary>
	LeaderDistribute = 2,

	/// <summary>Master looter (leader picks up all)</summary>
	MasterLooter = 3
}

/// <summary>
/// Result of a party operation.
/// </summary>
public class PartyResult
{
	/// <summary>Whether the operation succeeded</summary>
	public bool Success { get; set; }

	/// <summary>Error code if failed</summary>
	public PartyError Error { get; set; }

	/// <summary>Error message</summary>
	public string Message { get; set; } = string.Empty;

	/// <summary>The party involved (if applicable)</summary>
	public Party? Party { get; set; }

	public static PartyResult Ok(Party? party = null) => new() { Success = true, Party = party };
	public static PartyResult Fail(PartyError error, string message = "") => new() { Success = false, Error = error, Message = message };
}

/// <summary>
/// Party operation error codes.
/// </summary>
public enum PartyError
{
	None,
	AlreadyInParty,
	NotInParty,
	NotPartyLeader,
	PartyFull,
	PlayerNotFound,
	PlayerOffline,
	PlayerAlreadyInParty,
	InvalidMemberIndex,
	CannotKickSelf,
	CannotInviteSelf,
	NoPendingInvitation,
	InvitationExpired,
	PartyNameTooLong,
	PartyNameInvalid
}

/// <summary>
/// Pending party invitation.
/// </summary>
public class PartyInvitation
{
	/// <summary>Party ID</summary>
	public uint PartyId { get; set; }

	/// <summary>Party name</summary>
	public string PartyName { get; set; } = string.Empty;

	/// <summary>Inviter's character name</summary>
	public string InviterName { get; set; } = string.Empty;

	/// <summary>When the invitation was sent</summary>
	public DateTime SentAt { get; set; } = DateTime.Now;

	/// <summary>Invitation timeout (default 60 seconds)</summary>
	public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

	/// <summary>Whether the invitation has expired</summary>
	public bool IsExpired => DateTime.Now - SentAt > Timeout;
}

/// <summary>
/// Pending party join request (player requests to join an existing party).
/// </summary>
public class PartyJoinRequest
{
	/// <summary>Party ID the player wants to join</summary>
	public uint PartyId { get; set; }

	/// <summary>Party name</summary>
	public string PartyName { get; set; } = string.Empty;

	/// <summary>Requester's character name</summary>
	public string RequesterName { get; set; } = string.Empty;

	/// <summary>Requester's instance ID</summary>
	public long RequesterInstId { get; set; }

	/// <summary>Requester's player client</summary>
	public PlayerClient? Requester { get; set; }

	/// <summary>When the request was sent</summary>
	public DateTime SentAt { get; set; } = DateTime.Now;

	/// <summary>Request timeout (default 60 seconds)</summary>
	public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

	/// <summary>Whether the request has expired</summary>
	public bool IsExpired => DateTime.Now - SentAt > Timeout;
}
