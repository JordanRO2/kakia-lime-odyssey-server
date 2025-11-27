using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Party;

/// <summary>
/// Service for managing parties.
/// </summary>
public class PartyService : IPartyService
{
	private static uint _nextPartyId = 1;
	private readonly ConcurrentDictionary<uint, Party> _parties = new();
	private readonly ConcurrentDictionary<long, uint> _playerPartyMap = new(); // Player instance ID -> Party ID
	private readonly ConcurrentDictionary<long, PartyInvitation> _pendingInvitations = new(); // Target player ID -> Invitation

	/// <summary>
	/// Creates a new party with the given player as leader.
	/// </summary>
	public PartyResult CreateParty(PlayerClient leader, string partyName)
	{
		var character = leader.GetCurrentCharacter();
		if (character == null)
			return PartyResult.Fail(PartyError.PlayerNotFound, "Character not found");

		long playerId = leader.GetId();

		// Check if already in party
		if (_playerPartyMap.ContainsKey(playerId))
			return PartyResult.Fail(PartyError.AlreadyInParty, "Already in a party");

		// Validate party name
		if (string.IsNullOrWhiteSpace(partyName))
			partyName = $"{character.appearance.name}'s Party";

		if (partyName.Length > 40)
			return PartyResult.Fail(PartyError.PartyNameTooLong, "Party name too long");

		// Create party
		uint partyId = Interlocked.Increment(ref _nextPartyId);
		var pos = leader.GetPosition();

		var party = new Party
		{
			Id = partyId,
			Name = partyName,
			LeaderIndex = 0,
			Option = PartyOption.FreeForAll
		};

		var member = new PartyMember
		{
			Index = 0,
			Name = character.appearance.name,
			Player = leader,
			InstId = playerId,
			IsConnected = true,
			ZoneId = leader.GetZone(),
			PosX = pos.x,
			PosY = pos.y,
			PosZ = pos.z
		};

		party.Members.Add(member);

		_parties[partyId] = party;
		_playerPartyMap[playerId] = partyId;

		// Send SC_PARTY_CREATED to leader
		SendPartyCreated(leader);

		// Send SC_PARTY_JOINED to leader with initial party info
		SendPartyJoined(leader, party, member);

		Logger.Log($"[PARTY] {character.appearance.name} created party '{partyName}' (ID: {partyId})", LogLevel.Information);

		return PartyResult.Ok(party);
	}

	/// <summary>
	/// Disbands a party (leader only).
	/// </summary>
	public bool DisbandParty(PlayerClient leader)
	{
		var party = GetParty(leader);
		if (party == null)
			return false;

		var member = party.GetMember(leader);
		if (member == null || member.Index != party.LeaderIndex)
			return false;

		// Send SC_PARTY_DISBANDED to all members
		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendPartyDisbanded(m.Player!);
			_playerPartyMap.TryRemove(m.InstId, out _);
		}

		_parties.TryRemove(party.Id, out _);

		var character = leader.GetCurrentCharacter();
		Logger.Log($"[PARTY] {character?.appearance.name} disbanded party '{party.Name}'", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Sends an invitation to a player.
	/// </summary>
	public PartyResult InvitePlayer(PlayerClient inviter, string targetName)
	{
		var inviterChar = inviter.GetCurrentCharacter();
		if (inviterChar == null)
			return PartyResult.Fail(PartyError.PlayerNotFound);

		// Cannot invite yourself
		if (inviterChar.appearance.name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
			return PartyResult.Fail(PartyError.CannotInviteSelf, "Cannot invite yourself");

		// Find target player
		var target = LimeServer.PlayerClients.FirstOrDefault(p =>
		{
			var c = p.GetCurrentCharacter();
			return c != null && c.appearance.name.Equals(targetName, StringComparison.OrdinalIgnoreCase);
		});

		if (target == null)
			return PartyResult.Fail(PartyError.PlayerNotFound, $"Player '{targetName}' not found");

		long targetId = target.GetId();

		// Check if target is already in a party
		if (_playerPartyMap.ContainsKey(targetId))
			return PartyResult.Fail(PartyError.PlayerAlreadyInParty, "Player is already in a party");

		// Get or create party
		var party = GetParty(inviter);
		if (party == null)
		{
			// Auto-create party when first invite is sent
			var createResult = CreateParty(inviter, $"{inviterChar.appearance.name}'s Party");
			if (!createResult.Success)
				return createResult;
			party = createResult.Party!;
		}

		// Check if party is full
		if (party.IsFull)
			return PartyResult.Fail(PartyError.PartyFull, "Party is full");

		// Check if inviter is leader
		var inviterMember = party.GetMember(inviter);
		if (inviterMember == null || inviterMember.Index != party.LeaderIndex)
			return PartyResult.Fail(PartyError.NotPartyLeader, "Only party leader can invite");

		// Create invitation
		var invitation = new PartyInvitation
		{
			PartyId = party.Id,
			PartyName = party.Name,
			InviterName = inviterChar.appearance.name,
			SentAt = DateTime.Now
		};

		_pendingInvitations[targetId] = invitation;

		// Send SC_PARTY_INVITED to target
		SendPartyInvited(target, inviterChar.appearance.name, party.Name);

		Logger.Log($"[PARTY] {inviterChar.appearance.name} invited {targetName} to party '{party.Name}'", LogLevel.Debug);

		return PartyResult.Ok(party);
	}

	/// <summary>
	/// Accepts a pending party invitation.
	/// </summary>
	public PartyResult AcceptInvitation(PlayerClient player)
	{
		long playerId = player.GetId();

		if (!_pendingInvitations.TryRemove(playerId, out var invitation))
			return PartyResult.Fail(PartyError.NoPendingInvitation, "No pending invitation");

		if (invitation.IsExpired)
			return PartyResult.Fail(PartyError.InvitationExpired, "Invitation expired");

		if (!_parties.TryGetValue(invitation.PartyId, out var party))
			return PartyResult.Fail(PartyError.PartyFull, "Party no longer exists");

		if (party.IsFull)
			return PartyResult.Fail(PartyError.PartyFull, "Party is full");

		var character = player.GetCurrentCharacter();
		if (character == null)
			return PartyResult.Fail(PartyError.PlayerNotFound);

		var pos = player.GetPosition();
		uint newIndex = party.GetNextIndex();

		var member = new PartyMember
		{
			Index = newIndex,
			Name = character.appearance.name,
			Player = player,
			InstId = playerId,
			IsConnected = true,
			ZoneId = player.GetZone(),
			PosX = pos.x,
			PosY = pos.y,
			PosZ = pos.z
		};

		party.Members.Add(member);
		_playerPartyMap[playerId] = party.Id;

		// Send SC_PARTY_MEMBER_ADDED to existing members
		foreach (var m in party.Members.Where(m => m.Index != newIndex && m.IsConnected && m.Player != null))
		{
			SendPartyMemberAdded(m.Player!, member);
		}

		// Send SC_PARTY_JOINED to new member with all party info
		SendPartyJoined(player, party, member);

		Logger.Log($"[PARTY] {character.appearance.name} joined party '{party.Name}'", LogLevel.Information);

		return PartyResult.Ok(party);
	}

	/// <summary>
	/// Declines a pending party invitation.
	/// </summary>
	public void DeclineInvitation(PlayerClient player)
	{
		_pendingInvitations.TryRemove(player.GetId(), out _);
	}

	/// <summary>
	/// Requests to join a player's party.
	/// </summary>
	public PartyResult RequestJoin(PlayerClient requester, string targetName)
	{
		var requesterChar = requester.GetCurrentCharacter();
		if (requesterChar == null)
			return PartyResult.Fail(PartyError.PlayerNotFound);

		// Cannot request to join your own party
		if (requesterChar.appearance.name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
			return PartyResult.Fail(PartyError.CannotInviteSelf, "Cannot request to join yourself");

		// Check if requester is already in a party
		if (_playerPartyMap.ContainsKey(requester.GetId()))
			return PartyResult.Fail(PartyError.PlayerAlreadyInParty, "You are already in a party");

		// Find target player
		var target = LimeServer.PlayerClients.FirstOrDefault(p =>
		{
			var c = p.GetCurrentCharacter();
			return c != null && c.appearance.name.Equals(targetName, StringComparison.OrdinalIgnoreCase);
		});

		if (target == null)
			return PartyResult.Fail(PartyError.PlayerNotFound, $"Player '{targetName}' not found");

		// Check if target has a party
		var party = GetParty(target);
		if (party == null)
			return PartyResult.Fail(PartyError.NotInParty, $"Player '{targetName}' is not in a party");

		// Check if party is full
		if (party.IsFull)
			return PartyResult.Fail(PartyError.PartyFull, "Party is full");

		// Get party leader
		var leader = party.GetLeader();
		if (leader == null)
			return PartyResult.Fail(PartyError.NotPartyLeader, "Party has no leader");

		// TODO: Send join request notification to party leader
		// For now, just log the request
		Logger.Log($"[PARTY] {requesterChar.appearance.name} requested to join party '{party.Name}'", LogLevel.Debug);

		return PartyResult.Ok(party);
	}

	/// <summary>
	/// Player voluntarily leaves party.
	/// </summary>
	public bool LeaveParty(PlayerClient player)
	{
		var party = GetParty(player);
		if (party == null)
			return false;

		var member = party.GetMember(player);
		if (member == null)
			return false;

		long playerId = player.GetId();
		bool wasLeader = member.Index == party.LeaderIndex;

		// Remove member
		party.Members.Remove(member);
		_playerPartyMap.TryRemove(playerId, out _);

		// Send SC_PARTY_SECEDED to leaving player
		SendPartySeceded(player, member.Index);

		// If party is now empty, disband
		if (party.Members.Count == 0)
		{
			_parties.TryRemove(party.Id, out _);
			Logger.Log($"[PARTY] Party '{party.Name}' disbanded (empty)", LogLevel.Debug);
			return true;
		}

		// Notify remaining members
		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendPartySeceded(m.Player!, member.Index);
		}

		// If leader left, assign new leader
		if (wasLeader && party.Members.Count > 0)
		{
			var newLeader = party.Members.First();
			party.LeaderIndex = newLeader.Index;

			foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
			{
				SendPartyChangedLeader(m.Player!, newLeader.Index);
			}

			Logger.Log($"[PARTY] {newLeader.Name} is now leader of '{party.Name}'", LogLevel.Debug);
		}

		var character = player.GetCurrentCharacter();
		Logger.Log($"[PARTY] {character?.appearance.name} left party '{party.Name}'", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Kicks a member from the party (leader only).
	/// </summary>
	public bool KickMember(PlayerClient leader, uint memberIdx)
	{
		var party = GetParty(leader);
		if (party == null)
			return false;

		var leaderMember = party.GetMember(leader);
		if (leaderMember == null || leaderMember.Index != party.LeaderIndex)
			return false;

		// Cannot kick yourself
		if (memberIdx == party.LeaderIndex)
			return false;

		var targetMember = party.GetMember(memberIdx);
		if (targetMember == null)
			return false;

		// Remove member
		party.Members.Remove(targetMember);
		_playerPartyMap.TryRemove(targetMember.InstId, out _);

		// Notify kicked player
		if (targetMember.IsConnected && targetMember.Player != null)
		{
			SendPartyMemberBanned(targetMember.Player, memberIdx);
		}

		// Notify remaining members
		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendPartyMemberBanned(m.Player!, memberIdx);
		}

		var leaderChar = leader.GetCurrentCharacter();
		Logger.Log($"[PARTY] {leaderChar?.appearance.name} kicked {targetMember.Name} from party", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Changes the party leader.
	/// </summary>
	public bool ChangeLeader(PlayerClient currentLeader, uint newLeaderIdx)
	{
		var party = GetParty(currentLeader);
		if (party == null)
			return false;

		var leaderMember = party.GetMember(currentLeader);
		if (leaderMember == null || leaderMember.Index != party.LeaderIndex)
			return false;

		var newLeader = party.GetMember(newLeaderIdx);
		if (newLeader == null)
			return false;

		party.LeaderIndex = newLeaderIdx;

		// Notify all members
		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendPartyChangedLeader(m.Player!, newLeaderIdx);
		}

		Logger.Log($"[PARTY] {newLeader.Name} is now leader of '{party.Name}'", LogLevel.Information);

		return true;
	}

	/// <summary>
	/// Changes party options (loot distribution, etc.).
	/// </summary>
	public bool ChangeOption(PlayerClient leader, PartyOption option)
	{
		var party = GetParty(leader);
		if (party == null)
			return false;

		var leaderMember = party.GetMember(leader);
		if (leaderMember == null || leaderMember.Index != party.LeaderIndex)
			return false;

		party.Option = option;

		// Notify all members
		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendPartyChangedOption(m.Player!, option);
		}

		return true;
	}

	/// <summary>
	/// Gets the party a player is in.
	/// </summary>
	public Party? GetParty(PlayerClient player)
	{
		long playerId = player.GetId();
		if (_playerPartyMap.TryGetValue(playerId, out uint partyId))
		{
			_parties.TryGetValue(partyId, out var party);
			return party;
		}
		return null;
	}

	/// <summary>
	/// Gets a party by its ID.
	/// </summary>
	public Party? GetPartyById(uint partyId)
	{
		_parties.TryGetValue(partyId, out var party);
		return party;
	}

	/// <summary>
	/// Checks if a player is in a party.
	/// </summary>
	public bool IsInParty(PlayerClient player)
	{
		return _playerPartyMap.ContainsKey(player.GetId());
	}

	/// <summary>
	/// Checks if a player is a party leader.
	/// </summary>
	public bool IsPartyLeader(PlayerClient player)
	{
		var party = GetParty(player);
		if (party == null) return false;

		var member = party.GetMember(player);
		return member != null && member.Index == party.LeaderIndex;
	}

	/// <summary>
	/// Updates a party member's HP and broadcasts to party.
	/// </summary>
	public void UpdateMemberHP(PlayerClient player, int hp, int maxHp)
	{
		var party = GetParty(player);
		if (party == null) return;

		var member = party.GetMember(player);
		if (member == null) return;

		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendPartyUpdateMemberHP(m.Player!, member.Index, hp, maxHp);
		}
	}

	/// <summary>
	/// Updates a party member's MP and broadcasts to party.
	/// </summary>
	public void UpdateMemberMP(PlayerClient player, int mp, int maxMp)
	{
		var party = GetParty(player);
		if (party == null) return;

		var member = party.GetMember(player);
		if (member == null) return;

		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendPartyUpdateMemberMP(m.Player!, member.Index, mp, maxMp);
		}
	}

	/// <summary>
	/// Updates a party member's position and broadcasts to party.
	/// </summary>
	public void UpdateMemberPosition(PlayerClient player, float x, float y, float z, uint zoneId)
	{
		var party = GetParty(player);
		if (party == null) return;

		var member = party.GetMember(player);
		if (member == null) return;

		member.PosX = x;
		member.PosY = y;
		member.PosZ = z;
		member.ZoneId = zoneId;

		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendPartyUpdateMemberPos(m.Player!, member.Index, x, y, z, zoneId);
		}
	}

	/// <summary>
	/// Handles player disconnection.
	/// </summary>
	public void OnPlayerDisconnect(PlayerClient player)
	{
		var party = GetParty(player);
		if (party == null) return;

		var member = party.GetMember(player);
		if (member == null) return;

		member.IsConnected = false;
		member.Player = null;

		// Notify other members
		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendPartyMemberDisconnected(m.Player!, member.Index);
		}

		Logger.Log($"[PARTY] {member.Name} disconnected from party '{party.Name}'", LogLevel.Debug);
	}

	/// <summary>
	/// Handles player reconnection.
	/// </summary>
	public void OnPlayerReconnect(PlayerClient player)
	{
		long playerId = player.GetId();
		if (!_playerPartyMap.TryGetValue(playerId, out uint partyId))
			return;

		if (!_parties.TryGetValue(partyId, out var party))
			return;

		var member = party.Members.FirstOrDefault(m => m.InstId == playerId);
		if (member == null) return;

		member.IsConnected = true;
		member.Player = player;

		var pos = player.GetPosition();
		member.PosX = pos.x;
		member.PosY = pos.y;
		member.PosZ = pos.z;
		member.ZoneId = player.GetZone();

		// Send party info to reconnecting player
		SendPartyJoined(player, party, member);

		// Notify other members
		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendPartyMemberConnected(m.Player!, member);
		}

		Logger.Log($"[PARTY] {member.Name} reconnected to party '{party.Name}'", LogLevel.Debug);
	}

	/// <summary>
	/// Sends a chat message to party members.
	/// </summary>
	public void SendPartyChat(PlayerClient sender, string message, uint maintainTime, int type)
	{
		var party = GetParty(sender);
		if (party == null) return;

		var member = party.GetMember(sender);
		if (member == null) return;

		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null))
		{
			SendPartySay(m.Player!, member.Index, message, maintainTime, type);
		}
	}

	/// <summary>
	/// Notifies party when a member loots an item.
	/// </summary>
	public void NotifyMemberLootedItem(PlayerClient player, int itemTypeId, long count)
	{
		var party = GetParty(player);
		if (party == null) return;

		var member = party.GetMember(player);
		if (member == null) return;

		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendPartyMemberLootedItem(m.Player!, member.Index, itemTypeId, count);
		}
	}

	/// <summary>
	/// Notifies party when a member gains a buff.
	/// </summary>
	public void NotifyMemberAddedBuff(PlayerClient player, DEF def)
	{
		var party = GetParty(player);
		if (party == null) return;

		var member = party.GetMember(player);
		if (member == null) return;

		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendPartyMemberAddDef(m.Player!, member.Index, def);
		}
	}

	/// <summary>
	/// Notifies party when a member loses a buff.
	/// </summary>
	public void NotifyMemberRemovedBuff(PlayerClient player, uint defInstId)
	{
		var party = GetParty(player);
		if (party == null) return;

		var member = party.GetMember(player);
		if (member == null) return;

		foreach (var m in party.Members.Where(m => m.IsConnected && m.Player != null && m.Player != player))
		{
			SendPartyMemberDelDef(m.Player!, member.Index, defInstId);
		}
	}

	#region Packet Sending

	private static void SendPartyCreated(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_CREATED());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyDisbanded(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_DISBANDED());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyJoined(PlayerClient player, Party party, PartyMember self)
	{
		using PacketWriter pw = new();

		// Write header
		var header = new SC_PARTY_JOINED
		{
			name = System.Text.Encoding.ASCII.GetBytes(party.Name),
			myIdx = self.Index,
			leaderIndex = party.LeaderIndex,
			option = (byte)party.Option
		};
		pw.Write(header);

		// Write member count and members
		pw.Write((byte)party.Members.Count);
		foreach (var member in party.Members)
		{
			pw.Write(PacketConverter.AsBytes(member.ToPacketStruct()));
		}

		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	private static void SendPartyInvited(PlayerClient player, string inviterName, string partyName)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_INVITED
		{
			pcName = System.Text.Encoding.ASCII.GetBytes(inviterName),
			partyName = System.Text.Encoding.ASCII.GetBytes(partyName)
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyMemberAdded(PlayerClient player, PartyMember member)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_MEMBER_ADDED
		{
			member = member.ToPacketStruct()
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartySeceded(PlayerClient player, uint memberIdx)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_SECEDED
		{
			idx = memberIdx
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyMemberBanned(PlayerClient player, uint memberIdx)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_MEMBER_BANNED
		{
			idx = memberIdx
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyChangedLeader(PlayerClient player, uint newLeaderIdx)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_CHANGED_LEADER
		{
			idx = newLeaderIdx
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyChangedOption(PlayerClient player, PartyOption option)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_CHANGED_OPTION
		{
			type = (byte)option
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyUpdateMemberHP(PlayerClient player, uint memberIdx, int hp, int maxHp)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_UPDATE_MEMBER_HP
		{
			idx = memberIdx,
			hp = hp,
			mhp = maxHp
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyUpdateMemberMP(PlayerClient player, uint memberIdx, int mp, int maxMp)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_UPDATE_MEMBER_MP
		{
			idx = memberIdx,
			mp = mp,
			mmp = maxMp
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyUpdateMemberPos(PlayerClient player, uint memberIdx, float x, float y, float z, uint zoneId)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_UPDATE_MEMBER_POS
		{
			idx = memberIdx,
			zoneID = zoneId,
			pos = new FPOS { x = x, y = y, z = z }
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyMemberDisconnected(PlayerClient player, uint memberIdx)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_MEMBER_DISCONNECTED
		{
			idx = memberIdx
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyMemberConnected(PlayerClient player, PartyMember member)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_MEMBER_CONNECTED
		{
			idx = member.Index,
			instID = member.InstId,
			name = System.Text.Encoding.ASCII.GetBytes(member.Name)
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartySay(PlayerClient player, uint senderIdx, string message, uint maintainTime, int type)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_SAY
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

	private static void SendPartyMemberLootedItem(PlayerClient player, uint memberIdx, int itemTypeId, long count)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_MEMBER_LOOTED_ITEM
		{
			idx = memberIdx,
			itemTypeID = itemTypeId,
			count = count
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyMemberAddDef(PlayerClient player, uint memberIdx, DEF def)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_MEMBER_ADD_DEF
		{
			idx = memberIdx,
			def = def
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendPartyMemberDelDef(PlayerClient player, uint memberIdx, uint defInstId)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PARTY_MEMBER_DEL_DEF
		{
			idx = memberIdx,
			instID = defInstId
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	#endregion
}
