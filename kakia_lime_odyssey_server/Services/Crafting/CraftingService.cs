/// <summary>
/// Service for managing item crafting and material processing.
/// </summary>
/// <remarks>
/// Handles item making (crafting) and stuff making (gathering/processing).
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Crafting;

/// <summary>
/// Tracks active crafting session for a player.
/// </summary>
public class CraftingSession
{
	public uint RecipeTypeID { get; set; }
	public int TargetCount { get; set; }
	public int CompletedCount { get; set; }
	public bool IsContinuous { get; set; }
	public DateTime StartTime { get; set; }
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

		// TODO: Validate recipe exists
		// TODO: Check player has required life job level
		// TODO: Check player has required materials

		var session = new CraftingSession
		{
			RecipeTypeID = typeID,
			TargetCount = 1,
			CompletedCount = 0,
			IsContinuous = false,
			StartTime = DateTime.Now
		};

		_activeSessions[playerId] = session;

		Logger.Log($"[CRAFT] {playerName} ready to craft recipe {typeID}", LogLevel.Debug);

		// TODO: Send SC_ITEM_MAKE_UPDATE_REPORT
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

		session.StartTime = DateTime.Now;

		Logger.Log($"[CRAFT] {playerName} started crafting", LogLevel.Debug);

		// TODO: Consume materials
		// TODO: Start crafting timer
		// TODO: Send SC_ITEM_MAKE_START_CASTING

		return true;
	}

	/// <summary>
	/// Cancels the current crafting session.
	/// </summary>
	public void CancelItemMake(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_activeSessions.TryRemove(playerId, out _))
		{
			Logger.Log($"[CRAFT] {playerName} canceled crafting", LogLevel.Debug);
		}

		// TODO: Send SC_ITEM_MAKE_FINISH
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
}
