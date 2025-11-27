using System.Collections.Concurrent;
using System.Text;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Chatroom;

public class ChatroomService
{
	private static long _nextRoomId = 1;
	private readonly ConcurrentDictionary<long, PrivateChatroom> _chatrooms = new();
	private readonly ConcurrentDictionary<long, long> _playerRoomMap = new();

	public PrivateChatroom? CreateChatroom(PlayerClient creator, string name, string password, byte maxPersons, byte type)
	{
		long creatorId = creator.GetId();

		if (_playerRoomMap.ContainsKey(creatorId))
		{
			Logger.Log($"[CHATROOM] Creator already in a chatroom", LogLevel.Debug);
			return null;
		}

		long roomId = Interlocked.Increment(ref _nextRoomId);

		var character = creator.GetCurrentCharacter();
		var room = new PrivateChatroom
		{
			RoomId = roomId,
			Name = name,
			Password = password,
			MaxPersons = maxPersons,
			Type = type,
			MasterId = creatorId,
			Master = creator,
			CreatedAt = DateTime.Now
		};

		var member = new ChatroomMember
		{
			InstId = creatorId,
			Name = character?.appearance.name ?? "Unknown",
			Player = creator
		};

		room.Members.Add(member);
		_chatrooms[roomId] = room;
		_playerRoomMap[creatorId] = roomId;

		SendEntered(creator, room);

		Logger.Log($"[CHATROOM] {member.Name} created chatroom '{name}' (ID: {roomId})", LogLevel.Information);

		return room;
	}

	public bool DestroyChatroom(PlayerClient player)
	{
		var room = GetPlayerRoom(player);
		if (room == null) return false;

		if (room.MasterId != player.GetId())
		{
			Logger.Log($"[CHATROOM] Only master can destroy chatroom", LogLevel.Debug);
			return false;
		}

		foreach (var member in room.Members)
		{
			_playerRoomMap.TryRemove(member.InstId, out _);
			SendDestroyed(member.Player);
		}

		_chatrooms.TryRemove(room.RoomId, out _);

		Logger.Log($"[CHATROOM] Chatroom '{room.Name}' destroyed", LogLevel.Information);

		return true;
	}

	public bool EnterChatroom(PlayerClient player, long roomId, string password)
	{
		long playerId = player.GetId();

		if (_playerRoomMap.ContainsKey(playerId))
		{
			Logger.Log($"[CHATROOM] Player already in a chatroom", LogLevel.Debug);
			return false;
		}

		if (!_chatrooms.TryGetValue(roomId, out var room))
		{
			Logger.Log($"[CHATROOM] Room {roomId} not found", LogLevel.Debug);
			return false;
		}

		if (room.IsFull)
		{
			Logger.Log($"[CHATROOM] Room is full", LogLevel.Debug);
			return false;
		}

		if (!room.CheckPassword(password))
		{
			Logger.Log($"[CHATROOM] Wrong password", LogLevel.Debug);
			return false;
		}

		var character = player.GetCurrentCharacter();
		var member = new ChatroomMember
		{
			InstId = playerId,
			Name = character?.appearance.name ?? "Unknown",
			Player = player
		};

		foreach (var m in room.Members)
		{
			SendMemberAdded(m.Player, playerId, member.Name);
		}

		room.Members.Add(member);
		_playerRoomMap[playerId] = roomId;

		SendEntered(player, room);

		Logger.Log($"[CHATROOM] {member.Name} entered chatroom '{room.Name}'", LogLevel.Debug);

		return true;
	}

	public bool LeaveChatroom(PlayerClient player)
	{
		long playerId = player.GetId();
		var room = GetPlayerRoom(player);
		if (room == null) return false;

		var member = room.Members.FirstOrDefault(m => m.InstId == playerId);
		if (member == null) return false;

		room.Members.Remove(member);
		_playerRoomMap.TryRemove(playerId, out _);

		SendLeft(player, playerId);

		foreach (var m in room.Members)
		{
			SendLeft(m.Player, playerId);
		}

		if (room.Members.Count == 0 || playerId == room.MasterId)
		{
			foreach (var m in room.Members)
			{
				_playerRoomMap.TryRemove(m.InstId, out _);
				SendDestroyed(m.Player);
			}
			_chatrooms.TryRemove(room.RoomId, out _);
			Logger.Log($"[CHATROOM] Room '{room.Name}' closed", LogLevel.Debug);
		}

		Logger.Log($"[CHATROOM] {member.Name} left chatroom", LogLevel.Debug);

		return true;
	}

	public bool BanMember(PlayerClient master, long targetId)
	{
		var room = GetPlayerRoom(master);
		if (room == null) return false;

		if (room.MasterId != master.GetId())
		{
			Logger.Log($"[CHATROOM] Only master can ban members", LogLevel.Debug);
			return false;
		}

		var target = room.Members.FirstOrDefault(m => m.InstId == targetId);
		if (target == null) return false;

		room.Members.Remove(target);
		_playerRoomMap.TryRemove(targetId, out _);

		SendLeft(target.Player, targetId);

		foreach (var m in room.Members)
		{
			SendLeft(m.Player, targetId);
		}

		Logger.Log($"[CHATROOM] {target.Name} was banned from chatroom", LogLevel.Debug);

		return true;
	}

	public void SendMessage(PlayerClient sender, string message)
	{
		var room = GetPlayerRoom(sender);
		if (room == null) return;

		long senderId = sender.GetId();

		foreach (var member in room.Members)
		{
			SendSay(member.Player, senderId, message);
		}

		var character = sender.GetCurrentCharacter();
		Logger.Log($"[CHATROOM] {character?.appearance.name}: {message}", LogLevel.Debug);
	}

	public PrivateChatroom? GetPlayerRoom(PlayerClient player)
	{
		long playerId = player.GetId();
		if (_playerRoomMap.TryGetValue(playerId, out long roomId))
		{
			_chatrooms.TryGetValue(roomId, out var room);
			return room;
		}
		return null;
	}

	public bool IsInChatroom(PlayerClient player)
	{
		return _playerRoomMap.ContainsKey(player.GetId());
	}

	public void OnPlayerDisconnect(PlayerClient player)
	{
		LeaveChatroom(player);
	}

	private static void SendEntered(PlayerClient player, PrivateChatroom room)
	{
		using PacketWriter pw = new();

		pw.WriteHeader(PacketType.SC_PRIVATE_CHATROOM_ENTERED);
		pw.Write(room.MasterId);
		pw.Write(room.Type);

		pw.Write((byte)room.Members.Count);
		foreach (var member in room.Members)
		{
			pw.Write(member.InstId);
			var nameBytes = Encoding.ASCII.GetBytes(member.Name);
			pw.Write((byte)nameBytes.Length);
			pw.Write(nameBytes);
		}

		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	private static void SendDestroyed(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PRIVATE_CHATROOM_DESTROYED());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendMemberAdded(PlayerClient player, long instId, string name)
	{
		using PacketWriter pw = new();

		pw.WriteHeader(PacketType.SC_PRIVATE_CHATROOM_MEMBER_ADDED);
		pw.Write(instId);
		var nameBytes = Encoding.ASCII.GetBytes(name);
		pw.Write((byte)nameBytes.Length);
		pw.Write(nameBytes);

		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	private static void SendLeft(PlayerClient player, long instId)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_PRIVATE_CHATROOM_LEFT { instID = instId });
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendSay(PlayerClient player, long senderId, string message)
	{
		using PacketWriter pw = new();

		pw.WriteHeader(PacketType.SC_PRIVATE_CHATROOM_SAY);
		pw.Write(senderId);
		var msgBytes = Encoding.ASCII.GetBytes(message);
		pw.Write(msgBytes);
		pw.Write((byte)0);

		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	// ============ REALM-WIDE CHAT ============

	/// <summary>
	/// Sends a realm-wide chat message to all connected players.
	/// </summary>
	/// <param name="sender">The player sending the message</param>
	/// <param name="message">The chat message text</param>
	/// <param name="maintainTime">Display duration in milliseconds</param>
	/// <param name="type">Message type flags</param>
	public void SendRealmChat(PlayerClient sender, string message, uint maintainTime, int type)
	{
		long senderId = sender.GetId();
		string senderName = sender.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		foreach (var player in LimeServer.PlayerClients)
		{
			SendRealmSay(player, senderId, message, maintainTime, type);
		}

		Logger.Log($"[CHAT:REALM] {senderName}: {message}", LogLevel.Debug);
	}

	private static void SendRealmSay(PlayerClient player, long senderId, string message, uint maintainTime, int type)
	{
		using PacketWriter pw = new();

		pw.WriteHeader(PacketType.SC_REALM_SAY);
		pw.Write(senderId);
		pw.Write(maintainTime);
		pw.Write(type);
		var msgBytes = Encoding.ASCII.GetBytes(message);
		pw.Write(msgBytes);
		pw.Write((byte)0);

		player.Send(pw.ToSizedPacket(), default).Wait();
	}

	// ============ SERVER NOTICES ============

	/// <summary>
	/// Sends a server notice to all connected players.
	/// </summary>
	/// <param name="sender">The player/GM sending the notice (null for system)</param>
	/// <param name="message">The notice message text</param>
	/// <returns>True if the notice was sent (GM permission check passed)</returns>
	public bool SendServerNotice(PlayerClient? sender, string message)
	{
		string from = "System";

		if (sender != null)
		{
			// Check GM permission (account level > 0)
			var account = sender.GetAccount();
			if (account == null || account.AccessLevel <= 0)
			{
				Logger.Log($"[NOTICE] Non-GM player attempted to send notice", LogLevel.Warning);
				return false;
			}
			from = sender.GetCurrentCharacter()?.appearance.name ?? "GM";
		}

		foreach (var player in LimeServer.PlayerClients)
		{
			SendNotice(player, from, message);
		}

		Logger.Log($"[NOTICE] {from}: {message}", LogLevel.Information);
		return true;
	}

	private static void SendNotice(PlayerClient player, string from, string message)
	{
		using PacketWriter pw = new();

		pw.WriteHeader(PacketType.SC_NOTICE);

		// Write fixed 26-byte 'from' field
		var fromBytes = Encoding.ASCII.GetBytes(from);
		var fromBuffer = new byte[26];
		Array.Copy(fromBytes, fromBuffer, Math.Min(fromBytes.Length, 25));
		pw.Write(fromBuffer);

		// Write variable-length message
		var msgBytes = Encoding.ASCII.GetBytes(message);
		pw.Write(msgBytes);
		pw.Write((byte)0);

		player.Send(pw.ToSizedPacket(), default).Wait();
	}
}
