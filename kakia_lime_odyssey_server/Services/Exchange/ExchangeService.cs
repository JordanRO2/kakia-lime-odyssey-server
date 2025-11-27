using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Exchange;

public class ExchangeService
{
	private static long _nextSessionId = 1;
	private readonly ConcurrentDictionary<long, ExchangeSession> _sessions = new();
	private readonly ConcurrentDictionary<long, ExchangeRequest> _pendingRequests = new();
	private readonly ConcurrentDictionary<long, long> _playerSessionMap = new();

	public void RequestExchange(PlayerClient requester, PlayerClient target)
	{
		long requesterId = requester.GetId();
		long targetId = target.GetId();

		if (_playerSessionMap.ContainsKey(requesterId) || _playerSessionMap.ContainsKey(targetId))
		{
			Logger.Log($"[EXCHANGE] Request failed - player already in exchange", LogLevel.Debug);
			return;
		}

		var request = new ExchangeRequest
		{
			Requester = requester,
			Target = target,
			SentAt = DateTime.Now
		};

		_pendingRequests[targetId] = request;

		SendExchangeRequested(target);

		var requesterChar = requester.GetCurrentCharacter();
		var targetChar = target.GetCurrentCharacter();
		Logger.Log($"[EXCHANGE] {requesterChar?.appearance.name} requested exchange with {targetChar?.appearance.name}", LogLevel.Debug);
	}

	public void AcceptExchange(PlayerClient target)
	{
		long targetId = target.GetId();

		if (!_pendingRequests.TryRemove(targetId, out var request))
		{
			Logger.Log($"[EXCHANGE] Accept failed - no pending request", LogLevel.Debug);
			return;
		}

		if (request.IsExpired)
		{
			Logger.Log($"[EXCHANGE] Accept failed - request expired", LogLevel.Debug);
			return;
		}

		long sessionId = Interlocked.Increment(ref _nextSessionId);
		var session = new ExchangeSession
		{
			SessionId = sessionId,
			Player1 = request.Requester,
			Player2 = target,
			CreatedAt = DateTime.Now
		};

		_sessions[sessionId] = session;
		_playerSessionMap[request.Requester.GetId()] = sessionId;
		_playerSessionMap[targetId] = sessionId;

		SendStartExchange(request.Requester, targetId);
		SendStartExchange(target, request.Requester.GetId());

		Logger.Log($"[EXCHANGE] Session {sessionId} started", LogLevel.Debug);
	}

	public void RejectExchange(PlayerClient target)
	{
		long targetId = target.GetId();
		_pendingRequests.TryRemove(targetId, out _);
		Logger.Log($"[EXCHANGE] Request rejected", LogLevel.Debug);
	}

	public void AddItem(PlayerClient player, int slot, long count)
	{
		var session = GetSession(player);
		if (session == null) return;

		if (session.IsPlayerReady(player))
		{
			Logger.Log($"[EXCHANGE] Cannot add item - player already ready", LogLevel.Debug);
			return;
		}

		var items = session.GetPlayerItems(player);
		int exchangeSlot = items.Count;

		var item = new ExchangeItem
		{
			Slot = exchangeSlot,
			ItemTypeID = 1,
			Count = count,
			Durability = 100,
			MaxDurability = 100,
			Grade = 0
		};

		items.Add(item);

		SendSuccessAddItem(player, item.ItemTypeID, exchangeSlot, count);

		var other = session.GetOtherPlayer(player);
		if (other != null)
		{
			SendAddedItem(other, item);
		}

		session.ResetReady();

		Logger.Log($"[EXCHANGE] Player added item from slot {slot} (count: {count})", LogLevel.Debug);
	}

	public void SubtractItem(PlayerClient player, int slot, long count)
	{
		var session = GetSession(player);
		if (session == null) return;

		if (session.IsPlayerReady(player))
		{
			Logger.Log($"[EXCHANGE] Cannot remove item - player already ready", LogLevel.Debug);
			return;
		}

		var items = session.GetPlayerItems(player);
		var item = items.FirstOrDefault(i => i.Slot == slot);
		if (item == null) return;

		items.Remove(item);

		SendSuccessSubtractItem(player, item.ItemTypeID, slot, count);

		var other = session.GetOtherPlayer(player);
		if (other != null)
		{
			SendSubtractedItem(other, item.ItemTypeID, slot, count);
		}

		session.ResetReady();

		Logger.Log($"[EXCHANGE] Player removed item from slot {slot}", LogLevel.Debug);
	}

	public void SetReady(PlayerClient player)
	{
		var session = GetSession(player);
		if (session == null) return;

		session.SetPlayerReady(player, true);

		var other = session.GetOtherPlayer(player);
		if (other != null)
		{
			SendExchangeReady(other);
		}

		if (session.BothReady)
		{
			SendExchangeReadyAll(session.Player1);
			SendExchangeReadyAll(session.Player2);
		}

		Logger.Log($"[EXCHANGE] Player marked ready", LogLevel.Debug);
	}

	public void SetAgain(PlayerClient player)
	{
		var session = GetSession(player);
		if (session == null) return;

		session.ResetReady();

		var other = session.GetOtherPlayer(player);
		if (other != null)
		{
			SendExchangeAgain(other);
		}

		Logger.Log($"[EXCHANGE] Player requested to modify exchange", LogLevel.Debug);
	}

	public void SetOk(PlayerClient player)
	{
		var session = GetSession(player);
		if (session == null) return;

		if (!session.BothReady)
		{
			Logger.Log($"[EXCHANGE] Cannot confirm - both players not ready", LogLevel.Debug);
			return;
		}

		session.SetPlayerOk(player, true);

		var other = session.GetOtherPlayer(player);
		if (other != null)
		{
			SendExchangeOk(other);
		}

		if (session.BothOk)
		{
			CompleteExchange(session);
		}

		Logger.Log($"[EXCHANGE] Player confirmed exchange", LogLevel.Debug);
	}

	public void CancelExchange(PlayerClient player)
	{
		var session = GetSession(player);
		if (session == null) return;

		var other = session.GetOtherPlayer(player);
		if (other != null)
		{
			SendExchangeFail(other);
		}

		EndSession(session);

		Logger.Log($"[EXCHANGE] Exchange cancelled", LogLevel.Debug);
	}

	private void CompleteExchange(ExchangeSession session)
	{
		SendExchangeSuccess(session.Player1);
		SendExchangeSuccess(session.Player2);

		EndSession(session);

		Logger.Log($"[EXCHANGE] Session {session.SessionId} completed successfully", LogLevel.Information);
	}

	private void EndSession(ExchangeSession session)
	{
		_sessions.TryRemove(session.SessionId, out _);
		_playerSessionMap.TryRemove(session.Player1.GetId(), out _);
		_playerSessionMap.TryRemove(session.Player2.GetId(), out _);
	}

	public ExchangeSession? GetSession(PlayerClient player)
	{
		long playerId = player.GetId();
		if (_playerSessionMap.TryGetValue(playerId, out long sessionId))
		{
			_sessions.TryGetValue(sessionId, out var session);
			return session;
		}
		return null;
	}

	public bool IsInExchange(PlayerClient player)
	{
		return _playerSessionMap.ContainsKey(player.GetId());
	}

	public void OnPlayerDisconnect(PlayerClient player)
	{
		var session = GetSession(player);
		if (session != null)
		{
			CancelExchange(player);
		}

		_pendingRequests.TryRemove(player.GetId(), out _);
	}

	private static void SendExchangeRequested(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_EXCHANGE_REQUESTED());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendStartExchange(PlayerClient player, long targetId)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_START_EXCHANGE { target = targetId });
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendSuccessAddItem(PlayerClient player, int itemTypeID, int slot, long count)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_SUCCESS_ADD_ITEM_TO_EXCHANGE_LIST { itemTypeID = itemTypeID, slot = slot, count = count });
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendAddedItem(PlayerClient player, ExchangeItem item)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_ADDED_ITEM_TO_EXCHANGE_LIST
		{
			itemTypeID = item.ItemTypeID,
			slot = item.Slot,
			count = item.Count,
			durability = item.Durability,
			mdurability = item.MaxDurability,
			grade = item.Grade
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendSuccessSubtractItem(PlayerClient player, int itemTypeID, int slot, long count)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_SUCCESS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST { itemTypeID = itemTypeID, slot = slot, count = count });
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendSubtractedItem(PlayerClient player, int itemTypeID, int slot, long count)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_SUBTRACTED_ITEM_FROM_EXCHANGE_LIST { itemTypeID = itemTypeID, slot = slot, count = count });
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendExchangeReady(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_EXCHANGE_READY());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendExchangeReadyAll(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_EXCHANGE_READY_ALL());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendExchangeAgain(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_EXCHANGE_AGAIN());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendExchangeOk(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_EXCHANGE_OK());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendExchangeSuccess(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_EXCHANGE_SUCCESS());
		player.Send(pw.ToPacket(), default).Wait();
	}

	private static void SendExchangeFail(PlayerClient player)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_EXCHANGE_FAIL());
		player.Send(pw.ToPacket(), default).Wait();
	}
}
