/// <summary>
/// Service for managing player-to-player item and currency exchanges.
/// </summary>
/// <remarks>
/// Handles: Exchange requests, item/currency offers, trade completion
/// Uses: PlayerClient inventory, CurrencyService for Peder
/// All item transfers and currency exchanges are tracked via audit services.
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Audit;

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

	/// <summary>
	/// Adds an item from player's inventory to the exchange.
	/// </summary>
	/// <param name="player">Player adding the item</param>
	/// <param name="inventorySlot">Inventory slot of the item</param>
	/// <param name="count">Amount to add (for stackable items)</param>
	public void AddItem(PlayerClient player, int inventorySlot, long count)
	{
		var session = GetSession(player);
		if (session == null) return;

		if (session.IsPlayerReady(player))
		{
			Logger.Log($"[EXCHANGE] Cannot add item - player already ready", LogLevel.Debug);
			return;
		}

		// Get item from player's inventory
		var inventory = player.GetInventory();
		var inventoryItem = inventory.AtSlot(inventorySlot) as Models.Item;

		if (inventoryItem == null)
		{
			Logger.Log($"[EXCHANGE] Cannot add item - slot {inventorySlot} is empty", LogLevel.Debug);
			return;
		}

		// Check if enough quantity
		if ((long)inventoryItem.GetAmount() < count)
		{
			Logger.Log($"[EXCHANGE] Cannot add item - not enough quantity", LogLevel.Debug);
			return;
		}

		// Check if already added this slot
		var items = session.GetPlayerItems(player);
		if (items.Any(i => i.OriginalInventorySlot == inventorySlot))
		{
			Logger.Log($"[EXCHANGE] Cannot add item - slot already in exchange", LogLevel.Debug);
			return;
		}

		int exchangeSlot = items.Count;

		var exchangeItem = new ExchangeItem
		{
			Slot = exchangeSlot,
			OriginalInventorySlot = inventorySlot,
			ItemTypeID = inventoryItem.Id,
			Count = count,
			Durability = inventoryItem.GetDurability(),
			MaxDurability = inventoryItem.GetMaxDurability(),
			Grade = inventoryItem.Grade
		};

		items.Add(exchangeItem);

		// Notify the player that item was added successfully
		SendSuccessAddItem(player, exchangeItem.ItemTypeID, exchangeSlot, count);

		// Notify the other player about the added item
		var other = session.GetOtherPlayer(player);
		if (other != null)
		{
			SendAddedItem(other, exchangeItem);
		}

		// Reset ready state since items changed
		session.ResetReady();

		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXCHANGE] {playerName} added item {inventoryItem.Name} x{count} from slot {inventorySlot}", LogLevel.Debug);
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

	/// <summary>
	/// Sets the amount of Peder (gold) to offer in the exchange.
	/// </summary>
	/// <param name="player">Player offering the currency</param>
	/// <param name="amount">Amount of Peder to offer</param>
	public void SetPeder(PlayerClient player, long amount)
	{
		var session = GetSession(player);
		if (session == null) return;

		if (session.IsPlayerReady(player))
		{
			Logger.Log($"[EXCHANGE] Cannot set currency - player already ready", LogLevel.Debug);
			return;
		}

		// Validate player has enough Peder
		if (!LimeServer.CurrencyService.HasEnoughPeder(player, amount))
		{
			Logger.Log($"[EXCHANGE] Cannot set currency - insufficient Peder", LogLevel.Debug);
			return;
		}

		session.SetPlayerPeder(player, amount);

		// Reset ready state since offer changed
		session.ResetReady();

		string playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXCHANGE] {playerName} set Peder offer to {amount}", LogLevel.Debug);
	}

	/// <summary>
	/// Marks the player as ready to confirm the exchange.
	/// </summary>
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

	/// <summary>
	/// Completes the exchange by swapping items and currency between players.
	/// </summary>
	private void CompleteExchange(ExchangeSession session)
	{
		var player1Name = session.Player1.GetCurrentCharacter()?.appearance.name ?? "Player1";
		var player2Name = session.Player2.GetCurrentCharacter()?.appearance.name ?? "Player2";

		// Validate both players can complete the exchange
		if (!ValidateExchange(session))
		{
			Logger.Log($"[EXCHANGE] Session {session.SessionId} failed validation", LogLevel.Warning);
			SendExchangeFail(session.Player1);
			SendExchangeFail(session.Player2);
			EndSession(session);
			return;
		}

		// Transfer items from Player1 to Player2
		foreach (var item in session.Player1Items)
		{
			TransferItem(session.Player1, session.Player2, item);
		}

		// Transfer items from Player2 to Player1
		foreach (var item in session.Player2Items)
		{
			TransferItem(session.Player2, session.Player1, item);
		}

		// Transfer currency with audit logging
		if (session.Player1Peder > 0)
		{
			LimeServer.CurrencyService.TransferPederAudited(session.Player1, session.Player2, session.Player1Peder, "Exchange");
		}
		if (session.Player2Peder > 0)
		{
			LimeServer.CurrencyService.TransferPederAudited(session.Player2, session.Player1, session.Player2Peder, "Exchange");
		}

		// Send success notifications (client will refresh inventory)
		SendExchangeSuccess(session.Player1);
		SendExchangeSuccess(session.Player2);

		EndSession(session);

		Logger.Log($"[EXCHANGE] Session {session.SessionId} completed: {player1Name} <-> {player2Name}", LogLevel.Information);
	}

	/// <summary>
	/// Validates that the exchange can be completed (items still exist, currency available).
	/// </summary>
	private static bool ValidateExchange(ExchangeSession session)
	{
		// Validate Player1's items
		var inv1 = session.Player1.GetInventory();
		foreach (var item in session.Player1Items)
		{
			var invItem = inv1.AtSlot(item.OriginalInventorySlot) as Models.Item;
			if (invItem == null || invItem.Id != item.ItemTypeID || (long)invItem.GetAmount() < item.Count)
			{
				Logger.Log($"[EXCHANGE] Validation failed: Player1 item at slot {item.OriginalInventorySlot} missing or changed", LogLevel.Warning);
				return false;
			}
		}

		// Validate Player2's items
		var inv2 = session.Player2.GetInventory();
		foreach (var item in session.Player2Items)
		{
			var invItem = inv2.AtSlot(item.OriginalInventorySlot) as Models.Item;
			if (invItem == null || invItem.Id != item.ItemTypeID || (long)invItem.GetAmount() < item.Count)
			{
				Logger.Log($"[EXCHANGE] Validation failed: Player2 item at slot {item.OriginalInventorySlot} missing or changed", LogLevel.Warning);
				return false;
			}
		}

		// Validate currency
		if (session.Player1Peder > 0 && !LimeServer.CurrencyService.HasEnoughPeder(session.Player1, session.Player1Peder))
		{
			Logger.Log($"[EXCHANGE] Validation failed: Player1 insufficient Peder", LogLevel.Warning);
			return false;
		}
		if (session.Player2Peder > 0 && !LimeServer.CurrencyService.HasEnoughPeder(session.Player2, session.Player2Peder))
		{
			Logger.Log($"[EXCHANGE] Validation failed: Player2 insufficient Peder", LogLevel.Warning);
			return false;
		}

		return true;
	}

	/// <summary>
	/// Transfers an item from source player to target player.
	/// </summary>
	private static void TransferItem(PlayerClient source, PlayerClient target, ExchangeItem exchangeItem)
	{
		var sourceInv = source.GetInventory();
		var targetInv = target.GetInventory();

		var sourceItem = sourceInv.AtSlot(exchangeItem.OriginalInventorySlot) as Models.Item;
		if (sourceItem == null) return;

		// Create tracked item instance for audit trail
		var instanceId = LimeServer.ItemAuditService.CreateTrackedItem(sourceItem, ItemCreationSource.Exchange, source, "FromExchange");

		// Remove from source
		if ((long)sourceItem.GetAmount() <= exchangeItem.Count)
		{
			// Remove entire stack
			sourceInv.RemoveItem(exchangeItem.OriginalInventorySlot);
		}
		else
		{
			// Reduce stack count
			sourceItem.UpdateAmount(sourceItem.GetAmount() - (ulong)exchangeItem.Count);
		}

		// Create copy for target
		var newItem = new Models.Item
		{
			Id = sourceItem.Id,
			ModelId = sourceItem.ModelId,
			Name = sourceItem.Name,
			Desc = sourceItem.Desc,
			Grade = sourceItem.Grade,
			MaxEnchantCount = sourceItem.MaxEnchantCount,
			Type = sourceItem.Type,
			SecondType = sourceItem.SecondType,
			Level = sourceItem.Level,
			TribeClass = sourceItem.TribeClass,
			JobClassType = sourceItem.JobClassType,
			JobClassTypeId = sourceItem.JobClassTypeId,
			WeaponType = sourceItem.WeaponType,
			UserType = sourceItem.UserType,
			StuffType = sourceItem.StuffType,
			SkillId = sourceItem.SkillId,
			ImageName = sourceItem.ImageName,
			SmallImageName = sourceItem.SmallImageName,
			SortingType = sourceItem.SortingType,
			Series = sourceItem.Series,
			IsSell = sourceItem.IsSell,
			IsExchange = sourceItem.IsExchange,
			IsDiscard = sourceItem.IsDiscard,
			Material = sourceItem.Material,
			Durable = sourceItem.Durable,
			Price = sourceItem.Price,
			Inherits = sourceItem.Inherits,
			Count = (ulong)exchangeItem.Count,
			CurrentDurability = exchangeItem.Durability
		};

		// Add to target inventory
		targetInv.AddItem(newItem);

		// Log item transfer in audit trail
		LimeServer.ItemAuditService.LogTransfer(instanceId, source, target, (ulong)exchangeItem.Count, "Exchange");

		var sourceName = source.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var targetName = target.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[EXCHANGE] Transferred {newItem.Name} x{exchangeItem.Count} from {sourceName} to {targetName}", LogLevel.Debug);
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
