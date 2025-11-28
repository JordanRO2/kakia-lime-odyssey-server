/// <summary>
/// Service for tracking item lifecycle and audit logging.
/// </summary>
/// <remarks>
/// Tracks all item operations: creation, transfer, destruction, modification.
/// Each item instance has a unique GUID that persists across all operations.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Network;
using ItemModel = kakia_lime_odyssey_server.Models.Item;

namespace kakia_lime_odyssey_server.Services.Audit;

/// <summary>
/// Types of item lifecycle events.
/// </summary>
public enum ItemEventType
{
	/// <summary>Item created (loot, craft, shop, quest reward, GM command)</summary>
	Created,
	/// <summary>Item transferred between players (trade, exchange, mail)</summary>
	Transferred,
	/// <summary>Item moved within same player (inventory slot change, equip/unequip)</summary>
	Moved,
	/// <summary>Item consumed (potion, scroll, craft material)</summary>
	Consumed,
	/// <summary>Item destroyed (discarded, deleted)</summary>
	Destroyed,
	/// <summary>Item modified (durability, enchant, grade)</summary>
	Modified,
	/// <summary>Item split (stack split)</summary>
	Split,
	/// <summary>Item merged (stack merge)</summary>
	Merged,
	/// <summary>Item sold to NPC</summary>
	SoldToNpc,
	/// <summary>Item bought from NPC</summary>
	BoughtFromNpc
}

/// <summary>
/// Sources of item creation.
/// </summary>
public enum ItemCreationSource
{
	Unknown,
	Loot,
	Craft,
	Shop,
	QuestReward,
	Mail,
	Trade,
	Exchange,
	GMCommand,
	Spawn
}

/// <summary>
/// Record of an item lifecycle event.
/// </summary>
public class ItemAuditRecord
{
	/// <summary>Unique ID of this audit record</summary>
	public Guid AuditId { get; set; } = Guid.NewGuid();

	/// <summary>Unique instance ID of the item</summary>
	public Guid ItemInstanceId { get; set; }

	/// <summary>Item type ID from XML</summary>
	public int ItemTypeId { get; set; }

	/// <summary>Item name for readability</summary>
	public string ItemName { get; set; } = string.Empty;

	/// <summary>Type of event</summary>
	public ItemEventType EventType { get; set; }

	/// <summary>When this event occurred</summary>
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;

	/// <summary>Source character name (null for system events)</summary>
	public string? SourceCharacter { get; set; }

	/// <summary>Source account ID (null for system events)</summary>
	public string? SourceAccountId { get; set; }

	/// <summary>Target character name (for transfers)</summary>
	public string? TargetCharacter { get; set; }

	/// <summary>Target account ID (for transfers)</summary>
	public string? TargetAccountId { get; set; }

	/// <summary>Quantity affected</summary>
	public ulong Quantity { get; set; } = 1;

	/// <summary>Additional details (JSON or description)</summary>
	public string? Details { get; set; }

	/// <summary>Creation source (for Created events)</summary>
	public ItemCreationSource? CreationSource { get; set; }

	public override string ToString()
	{
		return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {EventType}: {ItemName} (x{Quantity}) " +
			   $"Instance={ItemInstanceId:N} Type={ItemTypeId} " +
			   $"From={SourceCharacter ?? "SYSTEM"} To={TargetCharacter ?? "N/A"} " +
			   $"Details={Details ?? "N/A"}";
	}
}

/// <summary>
/// Extended item class with instance tracking.
/// </summary>
public class TrackedItem
{
	/// <summary>Unique instance ID for this specific item</summary>
	public Guid InstanceId { get; set; } = Guid.NewGuid();

	/// <summary>The underlying item data</summary>
	public ItemModel Item { get; set; } = null!;

	/// <summary>When this item instance was created</summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>How this item was created</summary>
	public ItemCreationSource CreationSource { get; set; }

	/// <summary>Character who originally created/obtained this item</summary>
	public string? OriginalOwner { get; set; }

	/// <summary>Current owner character name</summary>
	public string? CurrentOwner { get; set; }
}

/// <summary>
/// Service for tracking item lifecycle events.
/// </summary>
public class ItemAuditService
{
	private readonly List<ItemAuditRecord> _auditLog = new();
	private readonly Dictionary<Guid, TrackedItem> _trackedItems = new();
	private readonly object _lock = new();

	/// <summary>
	/// Creates a new tracked item instance.
	/// </summary>
	public Guid CreateTrackedItem(ItemModel item, ItemCreationSource source, PlayerClient? owner, string? details = null)
	{
		var tracked = new TrackedItem
		{
			InstanceId = Guid.NewGuid(),
			Item = item,
			CreatedAt = DateTime.UtcNow,
			CreationSource = source,
			OriginalOwner = owner?.GetCurrentCharacter()?.appearance.name,
			CurrentOwner = owner?.GetCurrentCharacter()?.appearance.name
		};

		lock (_lock)
		{
			_trackedItems[tracked.InstanceId] = tracked;
		}

		LogEvent(new ItemAuditRecord
		{
			ItemInstanceId = tracked.InstanceId,
			ItemTypeId = item.Id,
			ItemName = item.Name,
			EventType = ItemEventType.Created,
			SourceCharacter = tracked.OriginalOwner,
			SourceAccountId = owner?.GetAccountId(),
			Quantity = item.Count,
			CreationSource = source,
			Details = details
		});

		Logger.Log($"[AUDIT] Item created: {item.Name} (x{item.Count}) Instance={tracked.InstanceId:N} " +
				   $"Source={source} Owner={tracked.OriginalOwner}", LogLevel.Information);

		return tracked.InstanceId;
	}

	/// <summary>
	/// Logs an item transfer between players.
	/// </summary>
	public void LogTransfer(Guid instanceId, PlayerClient from, PlayerClient to, ulong quantity, string transferType)
	{
		var fromName = from.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var toName = to.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		TrackedItem? tracked;
		lock (_lock)
		{
			_trackedItems.TryGetValue(instanceId, out tracked);
			if (tracked != null)
			{
				tracked.CurrentOwner = toName;
			}
		}

		LogEvent(new ItemAuditRecord
		{
			ItemInstanceId = instanceId,
			ItemTypeId = tracked?.Item.Id ?? 0,
			ItemName = tracked?.Item.Name ?? "Unknown",
			EventType = ItemEventType.Transferred,
			SourceCharacter = fromName,
			SourceAccountId = from.GetAccountId(),
			TargetCharacter = toName,
			TargetAccountId = to.GetAccountId(),
			Quantity = quantity,
			Details = $"TransferType={transferType}"
		});

		Logger.Log($"[AUDIT] Item transferred: {tracked?.Item.Name ?? "Unknown"} (x{quantity}) " +
				   $"Instance={instanceId:N} From={fromName} To={toName} Via={transferType}", LogLevel.Information);
	}

	/// <summary>
	/// Logs an item being consumed.
	/// </summary>
	public void LogConsume(Guid instanceId, PlayerClient player, ulong quantity, string reason)
	{
		var playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		TrackedItem? tracked;
		lock (_lock)
		{
			_trackedItems.TryGetValue(instanceId, out tracked);
		}

		LogEvent(new ItemAuditRecord
		{
			ItemInstanceId = instanceId,
			ItemTypeId = tracked?.Item.Id ?? 0,
			ItemName = tracked?.Item.Name ?? "Unknown",
			EventType = ItemEventType.Consumed,
			SourceCharacter = playerName,
			SourceAccountId = player.GetAccountId(),
			Quantity = quantity,
			Details = reason
		});

		Logger.Log($"[AUDIT] Item consumed: {tracked?.Item.Name ?? "Unknown"} (x{quantity}) " +
				   $"Instance={instanceId:N} By={playerName} Reason={reason}", LogLevel.Information);
	}

	/// <summary>
	/// Logs an item being destroyed.
	/// </summary>
	public void LogDestroy(Guid instanceId, PlayerClient? player, ulong quantity, string reason)
	{
		var playerName = player?.GetCurrentCharacter()?.appearance.name ?? "SYSTEM";

		TrackedItem? tracked;
		lock (_lock)
		{
			_trackedItems.TryGetValue(instanceId, out tracked);
			if (tracked != null)
			{
				_trackedItems.Remove(instanceId);
			}
		}

		LogEvent(new ItemAuditRecord
		{
			ItemInstanceId = instanceId,
			ItemTypeId = tracked?.Item.Id ?? 0,
			ItemName = tracked?.Item.Name ?? "Unknown",
			EventType = ItemEventType.Destroyed,
			SourceCharacter = playerName,
			SourceAccountId = player?.GetAccountId(),
			Quantity = quantity,
			Details = reason
		});

		Logger.Log($"[AUDIT] Item destroyed: {tracked?.Item.Name ?? "Unknown"} (x{quantity}) " +
				   $"Instance={instanceId:N} By={playerName} Reason={reason}", LogLevel.Information);
	}

	/// <summary>
	/// Logs an item being modified (durability, enchant, grade).
	/// </summary>
	public void LogModify(Guid instanceId, PlayerClient player, string modification)
	{
		var playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		TrackedItem? tracked;
		lock (_lock)
		{
			_trackedItems.TryGetValue(instanceId, out tracked);
		}

		LogEvent(new ItemAuditRecord
		{
			ItemInstanceId = instanceId,
			ItemTypeId = tracked?.Item.Id ?? 0,
			ItemName = tracked?.Item.Name ?? "Unknown",
			EventType = ItemEventType.Modified,
			SourceCharacter = playerName,
			SourceAccountId = player.GetAccountId(),
			Quantity = 1,
			Details = modification
		});

		Logger.Log($"[AUDIT] Item modified: {tracked?.Item.Name ?? "Unknown"} " +
				   $"Instance={instanceId:N} By={playerName} Change={modification}", LogLevel.Debug);
	}

	/// <summary>
	/// Logs an item sold to NPC.
	/// </summary>
	public void LogSoldToNpc(Guid instanceId, PlayerClient player, ulong quantity, int price)
	{
		var playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		TrackedItem? tracked;
		lock (_lock)
		{
			_trackedItems.TryGetValue(instanceId, out tracked);
		}

		LogEvent(new ItemAuditRecord
		{
			ItemInstanceId = instanceId,
			ItemTypeId = tracked?.Item.Id ?? 0,
			ItemName = tracked?.Item.Name ?? "Unknown",
			EventType = ItemEventType.SoldToNpc,
			SourceCharacter = playerName,
			SourceAccountId = player.GetAccountId(),
			Quantity = quantity,
			Details = $"Price={price}"
		});

		Logger.Log($"[AUDIT] Item sold to NPC: {tracked?.Item.Name ?? "Unknown"} (x{quantity}) " +
				   $"Instance={instanceId:N} By={playerName} Price={price}", LogLevel.Information);
	}

	/// <summary>
	/// Logs an item bought from NPC.
	/// </summary>
	public void LogBoughtFromNpc(Guid instanceId, PlayerClient player, ulong quantity, int price)
	{
		var playerName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		TrackedItem? tracked;
		lock (_lock)
		{
			_trackedItems.TryGetValue(instanceId, out tracked);
		}

		LogEvent(new ItemAuditRecord
		{
			ItemInstanceId = instanceId,
			ItemTypeId = tracked?.Item.Id ?? 0,
			ItemName = tracked?.Item.Name ?? "Unknown",
			EventType = ItemEventType.BoughtFromNpc,
			SourceCharacter = playerName,
			SourceAccountId = player.GetAccountId(),
			Quantity = quantity,
			Details = $"Price={price}"
		});

		Logger.Log($"[AUDIT] Item bought from NPC: {tracked?.Item.Name ?? "Unknown"} (x{quantity}) " +
				   $"Instance={instanceId:N} By={playerName} Price={price}", LogLevel.Information);
	}

	/// <summary>
	/// Gets a tracked item by instance ID.
	/// </summary>
	public TrackedItem? GetTrackedItem(Guid instanceId)
	{
		lock (_lock)
		{
			_trackedItems.TryGetValue(instanceId, out var tracked);
			return tracked;
		}
	}

	/// <summary>
	/// Gets all audit records for a specific item instance.
	/// </summary>
	public List<ItemAuditRecord> GetItemHistory(Guid instanceId)
	{
		lock (_lock)
		{
			return _auditLog.Where(r => r.ItemInstanceId == instanceId).ToList();
		}
	}

	/// <summary>
	/// Gets all audit records for a specific player.
	/// </summary>
	public List<ItemAuditRecord> GetPlayerHistory(string characterName)
	{
		lock (_lock)
		{
			return _auditLog.Where(r =>
				r.SourceCharacter == characterName ||
				r.TargetCharacter == characterName).ToList();
		}
	}

	/// <summary>
	/// Gets recent audit records.
	/// </summary>
	public List<ItemAuditRecord> GetRecentRecords(int count = 100)
	{
		lock (_lock)
		{
			return _auditLog.OrderByDescending(r => r.Timestamp).Take(count).ToList();
		}
	}

	private void LogEvent(ItemAuditRecord record)
	{
		lock (_lock)
		{
			_auditLog.Add(record);

			// Keep audit log from growing too large (keep last 10000 records in memory)
			// In production, these should be persisted to database
			if (_auditLog.Count > 10000)
			{
				_auditLog.RemoveRange(0, _auditLog.Count - 10000);
			}
		}
	}
}
