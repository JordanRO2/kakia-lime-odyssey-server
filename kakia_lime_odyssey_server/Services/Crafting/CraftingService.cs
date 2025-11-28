/// <summary>
/// Service for managing item crafting and material processing.
/// </summary>
/// <remarks>
/// Handles item making (crafting) and stuff making (gathering/processing).
/// Uses: ItemMakeInfo for recipe definitions, PlayerInventory for materials
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.ItemMakeXML;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Crafting;

/// <summary>
/// Tracks active crafting session for a player.
/// </summary>
public class CraftingSession
{
	public uint RecipeTypeID { get; set; }
	public MakeItemSkill? Recipe { get; set; }
	public int TargetCount { get; set; }
	public int CompletedCount { get; set; }
	public bool IsContinuous { get; set; }
	public DateTime StartTime { get; set; }
	public int SuccessRate { get; set; } = 75;
	public int CriticalRate { get; set; } = 10;
	public uint CraftTime { get; set; } = 3000;
	public ushort LpCost { get; set; } = 5;
}

/// <summary>
/// Tracks an item in the stuff make queue.
/// </summary>
public class StuffMakeItem
{
	public int Slot { get; set; }
	public long Count { get; set; }
}

/// <summary>
/// Tracks active stuff make (material processing) session for a player.
/// </summary>
public class StuffMakeSession
{
	public uint TypeID { get; set; }
	public List<StuffMakeItem> Items { get; set; } = new();
	public DateTime StartTime { get; set; }
}

/// <summary>
/// Service for managing item crafting and material processing.
/// </summary>
public class CraftingService
{
	private readonly ConcurrentDictionary<long, CraftingSession> _activeSessions = new();
	private readonly ConcurrentDictionary<long, StuffMakeSession> _stuffMakeSessions = new();

	/// <summary>
	/// Prepares crafting for a specific recipe.
	/// </summary>
	public bool ReadyItemMake(PlayerClient pc, uint typeID)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Validate recipe exists
		var recipe = ItemMakeInfo.GetMakeItemSkill((int)typeID);
		if (recipe == null)
		{
			Logger.Log($"[CRAFT] {playerName} tried unknown recipe {typeID}", LogLevel.Warning);
			return false;
		}

		// Check player has required materials
		var inventory = pc.GetInventory();
		var requiredItems = recipe.GetRequiredItems();

		foreach (var (itemId, count) in requiredItems)
		{
			int slot = inventory.FindItem(itemId, (ulong)count);
			if (slot < 0)
			{
				Logger.Log($"[CRAFT] {playerName} missing material {itemId} x{count}", LogLevel.Debug);
				return false;
			}
		}

		var session = new CraftingSession
		{
			RecipeTypeID = typeID,
			Recipe = recipe,
			TargetCount = 1,
			CompletedCount = 0,
			IsContinuous = false,
			StartTime = DateTime.Now,
			SuccessRate = 75,
			CriticalRate = 10,
			CraftTime = 3000,
			LpCost = 5
		};

		_activeSessions[playerId] = session;

		Logger.Log($"[CRAFT] {playerName} ready to craft recipe {typeID}", LogLevel.Debug);

		// Send SC_ITEM_MAKE_UPDATE_REPORT
		SendUpdateReport(pc, session);
		return true;
	}

	/// <summary>
	/// Starts the crafting process.
	/// </summary>
	public bool StartItemMake(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_activeSessions.TryGetValue(playerId, out var session))
		{
			Logger.Log($"[CRAFT] {playerName} has no active crafting session", LogLevel.Warning);
			return false;
		}

		if (session.Recipe == null)
		{
			Logger.Log($"[CRAFT] {playerName} session has no recipe", LogLevel.Warning);
			return false;
		}

		// Consume materials
		var inventory = pc.GetInventory();
		var requiredItems = session.Recipe.GetRequiredItems();

		foreach (var (itemId, count) in requiredItems)
		{
			int slot = inventory.FindItem(itemId, (ulong)count);
			if (slot < 0)
			{
				Logger.Log($"[CRAFT] {playerName} missing material {itemId} during start", LogLevel.Warning);
				return false;
			}

			var item = inventory.AtSlot(slot) as Models.Item;
			if (item != null)
			{
				if (item.GetAmount() <= (ulong)count)
				{
					inventory.RemoveItem(slot);
				}
				else
				{
					item.UpdateAmount(item.GetAmount() - (ulong)count);
					inventory.UpdateItemAtSlot(slot, item);
				}
			}
		}

		session.StartTime = DateTime.Now;

		// Determine if critical based on rate
		bool isCritical = Random.Shared.Next(100) < session.CriticalRate;

		Logger.Log($"[CRAFT] {playerName} started crafting recipe {session.RecipeTypeID}", LogLevel.Debug);

		// Send SC_ITEM_MAKE_START_CASTING
		SendStartCasting(pc, session, isCritical);

		return true;
	}

	/// <summary>
	/// Cancels the current crafting session.
	/// </summary>
	public void CancelItemMake(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_activeSessions.TryRemove(playerId, out var session))
		{
			Logger.Log($"[CRAFT] {playerName} canceled crafting", LogLevel.Debug);

			// Send SC_ITEM_MAKE_FINISH with cancelled result
			SendCraftFinish(pc, CraftResult.Cancelled, session?.LpCost ?? 0, 0);
		}
	}

	/// <summary>
	/// Starts continuous crafting for multiple items.
	/// </summary>
	public bool StartContinuousCrafting(PlayerClient pc, int count)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_activeSessions.TryGetValue(playerId, out var session))
		{
			Logger.Log($"[CRAFT] {playerName} has no active crafting session for continuous", LogLevel.Warning);
			return false;
		}

		session.TargetCount = count;
		session.IsContinuous = true;
		session.StartTime = DateTime.Now;

		Logger.Log($"[CRAFT] {playerName} started continuous crafting x{count}", LogLevel.Debug);

		// TODO: Validate can craft requested count
		// TODO: Start continuous crafting loop
		// TODO: Send SC_ITEM_MAKE_START_CASTING

		return true;
	}

	/// <summary>
	/// Stops continuous crafting.
	/// </summary>
	public void StopContinuousCrafting(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_activeSessions.TryGetValue(playerId, out var session))
		{
			session.IsContinuous = false;
			Logger.Log($"[CRAFT] {playerName} stopped continuous crafting at {session.CompletedCount}/{session.TargetCount}", LogLevel.Debug);
		}

		// TODO: Send SC_ITEM_MAKE_FINISH with progress
	}

	/// <summary>
	/// Gets the crafting report/status for a player.
	/// </summary>
	public void RequestReport(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		Logger.Log($"[CRAFT] {playerName} requested crafting report", LogLevel.Debug);

		// TODO: Send SC_ITEM_MAKE_UPDATE_REPORT with current status
	}

	/// <summary>
	/// Checks if a player has an active crafting session.
	/// </summary>
	public bool HasActiveSession(long playerId)
	{
		return _activeSessions.ContainsKey(playerId);
	}

	/// <summary>
	/// Cleans up crafting session for a player (on disconnect).
	/// </summary>
	public void CleanupPlayer(long playerId)
	{
		_activeSessions.TryRemove(playerId, out _);
		_stuffMakeSessions.TryRemove(playerId, out _);
	}

	// ============ STUFF MAKE (Material Processing) ============

	/// <summary>
	/// Prepares stuff make (material processing) for a specific type.
	/// </summary>
	public bool ReadyStuffMake(PlayerClient pc, uint typeID)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		var session = new StuffMakeSession
		{
			TypeID = typeID,
			StartTime = DateTime.Now
		};

		_stuffMakeSessions[playerId] = session;

		Logger.Log($"[CRAFT] {playerName} ready for stuff make type {typeID}", LogLevel.Debug);

		// TODO: Send SC_STUFF_MAKE_READY_SUCCESS
		return true;
	}

	/// <summary>
	/// Adds an item to the stuff make processing queue.
	/// </summary>
	public bool AddToStuffMakeList(PlayerClient pc, int slot, long count)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_stuffMakeSessions.TryGetValue(playerId, out var session))
		{
			Logger.Log($"[CRAFT] {playerName} has no active stuff make session", LogLevel.Warning);
			return false;
		}

		// TODO: Validate item exists in inventory slot
		session.Items.Add(new StuffMakeItem { Slot = slot, Count = count });

		Logger.Log($"[CRAFT] {playerName} added slot {slot} x{count} to stuff make list", LogLevel.Debug);

		// TODO: Send SC_STUFF_MAKE_ADD_LIST_SUCCESS
		return true;
	}

	/// <summary>
	/// Removes an item from the stuff make processing queue.
	/// </summary>
	public bool RemoveFromStuffMakeList(PlayerClient pc, int slot)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_stuffMakeSessions.TryGetValue(playerId, out var session))
		{
			Logger.Log($"[CRAFT] {playerName} has no active stuff make session", LogLevel.Warning);
			return false;
		}

		var itemToRemove = session.Items.FirstOrDefault(i => i.Slot == slot);
		if (itemToRemove != null)
		{
			session.Items.Remove(itemToRemove);
		}

		Logger.Log($"[CRAFT] {playerName} removed slot {slot} from stuff make list", LogLevel.Debug);

		// TODO: Send SC_STUFF_MAKE_DELETE_LIST_SUCCESS
		return true;
	}

	/// <summary>
	/// Starts the stuff make processing.
	/// </summary>
	public bool StartStuffMake(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_stuffMakeSessions.TryGetValue(playerId, out var session))
		{
			Logger.Log($"[CRAFT] {playerName} has no active stuff make session", LogLevel.Warning);
			return false;
		}

		session.StartTime = DateTime.Now;

		Logger.Log($"[CRAFT] {playerName} started stuff make with {session.Items.Count} items", LogLevel.Debug);

		// TODO: Consume materials
		// TODO: Start processing timer
		// TODO: Send SC_STUFF_MAKE_START_CASTING

		return true;
	}

	/// <summary>
	/// Cancels the current stuff make session.
	/// </summary>
	public void CancelStuffMake(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_stuffMakeSessions.TryRemove(playerId, out _))
		{
			Logger.Log($"[CRAFT] {playerName} canceled stuff make", LogLevel.Debug);
		}

		// TODO: Send SC_STUFF_MAKE_FINISH
	}

	// ============ STREAM GAUGE (Fishing/Gathering Minigame) ============

	/// <summary>
	/// Processes a stream gauge interaction result (fishing/gathering minigame).
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="gaugeValue">The gauge position/value from the minigame</param>
	public void UseStreamGauge(PlayerClient pc, int gaugeValue = 0)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Stream gauge is used for fishing and gathering minigames
		// The gauge value determines success/quality of the gathering attempt

		Logger.Log($"[CRAFT] {playerName} used stream gauge with value {gaugeValue}", LogLevel.Debug);

		// Calculate result based on gauge accuracy
		byte result = gaugeValue >= 80 ? (byte)2 : gaugeValue >= 50 ? (byte)1 : (byte)0;

		// Send result to client
		SendStreamGaugeResult(pc, result);
	}

	private static void SendStreamGaugeResult(PlayerClient pc, byte result)
	{
		using PacketWriter pw = new();

		pw.Write(new SC_USE_STREAM_GAUGE_RESULT
		{
			result = result
		});

		pc.Send(pw.ToPacket(), default).Wait();
	}

	// ============ PACKET HELPERS ============

	/// <summary>
	/// Sends SC_ITEM_MAKE_UPDATE_REPORT with recipe details.
	/// </summary>
	private void SendUpdateReport(PlayerClient pc, CraftingSession session)
	{
		// Get result item definition (simplified - assumes first item in recipe materials is the result)
		var resultItem = session.Recipe != null ? LimeServer.ItemDB.FirstOrDefault(i => i.Id == session.Recipe.SkillId) : null;

		var packet = new SC_ITEM_MAKE_UPDATE_REPORT
		{
			typeID = session.RecipeTypeID,
			successPercent = session.SuccessRate,
			criticalSuccessPercent = session.CriticalRate,
			makeTime = session.CraftTime,
			requestLP = session.LpCost,
			itemTypeID = resultItem?.Id ?? 0,
			count = 1,
			durability = 100,
			mdurability = 100,
			inherits = new ITEM_INHERITS(),
			criticalItemTypeID = resultItem?.Id ?? 0,
			criticalCount = 2,
			criticalDurability = 100,
			criticalMdurability = 100,
			criticalInherits = new ITEM_INHERITS()
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_ITEM_MAKE_START_CASTING to begin craft animation.
	/// </summary>
	private void SendStartCasting(PlayerClient pc, CraftingSession session, bool isCritical)
	{
		var packet = new SC_ITEM_MAKE_START_CASTING
		{
			InstID = pc.GetObjInstID(),
			typeID = session.RecipeTypeID,
			castTime = session.CraftTime,
			isCritical = isCritical
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_ITEM_MAKE_FINISH with craft result.
	/// </summary>
	private void SendCraftFinish(PlayerClient pc, CraftResult result, ushort lpUsed, int remainCount)
	{
		var packet = new SC_ITEM_MAKE_FINISH
		{
			finishResult = (byte)result,
			InstID = pc.GetObjInstID(),
			useLP = lpUsed,
			currentLP = 100,
			remainCount = remainCount
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Completes a crafting operation and awards the result item.
	/// </summary>
	public void CompleteCraft(PlayerClient pc, bool success, bool critical)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (!_activeSessions.TryGetValue(playerId, out var session))
		{
			return;
		}

		var result = success ? (critical ? CraftResult.Critical : CraftResult.Success) : CraftResult.Fail;

		if (success && session.Recipe != null)
		{
			// Award the crafted item
			var resultItemDef = LimeServer.ItemDB.FirstOrDefault(i => i.Id == session.Recipe.SkillId);
			if (resultItemDef != null)
			{
				var inventory = pc.GetInventory();
				var newItem = new Models.Item
				{
					Id = resultItemDef.Id,
					ModelId = resultItemDef.ModelId,
					Name = resultItemDef.Name,
					Desc = resultItemDef.Desc,
					Grade = resultItemDef.Grade,
					Type = resultItemDef.Type,
					SecondType = resultItemDef.SecondType,
					Level = resultItemDef.Level,
					Price = resultItemDef.Price,
					Inherits = resultItemDef.Inherits ?? new List<Models.Inherit>()
				};
				newItem.UpdateAmount(critical ? 2ul : 1ul);
				inventory.AddItem(newItem);

				Logger.Log($"[CRAFT] {playerName} crafted {resultItemDef.Name} x{(critical ? 2 : 1)}", LogLevel.Information);
			}

			session.CompletedCount++;
		}

		int remainCount = session.IsContinuous ? (session.TargetCount - session.CompletedCount) : 0;

		SendCraftFinish(pc, result, session.LpCost, remainCount);

		// Clean up if not continuous or completed all
		if (!session.IsContinuous || remainCount <= 0)
		{
			_activeSessions.TryRemove(playerId, out _);
		}

		pc.SendInventory();
	}
}

/// <summary>
/// Result of a crafting attempt.
/// </summary>
public enum CraftResult : byte
{
	Fail = 0,
	Success = 1,
	Critical = 2,
	Cancelled = 3
}
