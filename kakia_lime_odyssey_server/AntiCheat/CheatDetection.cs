using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Services.Ban;

namespace kakia_lime_odyssey_server.AntiCheat;

/// <summary>
/// Cheat detection and logging system
/// Logs suspicious activity for review and auto-ban
/// </summary>
public static class CheatDetection
{
	/// <summary>
	/// Set to false to disable auto-banning (logging still works)
	/// </summary>
	public static bool AutoBanEnabled { get; set; } = false;

	/// <summary>
	/// Set to false to disable movement validation enforcement (rubber-banding)
	/// When false, suspicious movements are logged but not blocked
	/// </summary>
	public static bool MovementValidationEnabled { get; set; } = false;

	/// <summary>
	/// Set to true to log all detections to a file for tuning
	/// </summary>
	public static bool FileLoggingEnabled { get; set; } = true;

	private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cheat_detections.log");
	private static readonly object _fileLock = new();

	public enum CheatType
	{
		SpeedHack,
		Teleport,
		FlyHack,
		InvalidDirection,
		TickManipulation,
		ItemDuplication,
		InvalidSkillCast,
		OutOfBounds
	}

	private static readonly Dictionary<uint, CheatLog> _playerLogs = new();

	private class CheatLog
	{
		public uint PlayerId { get; set; }
		public string PlayerName { get; set; } = string.Empty;
		public string AccountId { get; set; } = string.Empty;
		public string IpAddress { get; set; } = string.Empty;
		public Dictionary<CheatType, int> Violations { get; set; } = new();
		public DateTime FirstViolation { get; set; }
		public DateTime LastViolation { get; set; }
		public int TotalBanCount { get; set; }
	}

	/// <summary>
	/// Log a cheat detection event
	/// </summary>
	public static void LogCheat(uint playerId, string playerName, CheatType cheatType, string details)
	{
		LogCheat(playerId, playerName, string.Empty, string.Empty, cheatType, details);
	}

	/// <summary>
	/// Log a cheat detection event with full account information
	/// </summary>
	public static void LogCheat(uint playerId, string playerName, string accountId, string ipAddress, CheatType cheatType, string details)
	{
		lock (_playerLogs)
		{
			if (!_playerLogs.ContainsKey(playerId))
			{
				_playerLogs[playerId] = new CheatLog
				{
					PlayerId = playerId,
					PlayerName = playerName,
					AccountId = accountId,
					IpAddress = ipAddress,
					FirstViolation = DateTime.Now
				};
			}

			var log = _playerLogs[playerId];
			log.LastViolation = DateTime.Now;

			// Update account info if provided
			if (!string.IsNullOrEmpty(accountId))
				log.AccountId = accountId;
			if (!string.IsNullOrEmpty(ipAddress))
				log.IpAddress = ipAddress;

			if (!log.Violations.ContainsKey(cheatType))
				log.Violations[cheatType] = 0;

			log.Violations[cheatType]++;

			Logger.Log($"[CHEAT] Player {playerName} ({playerId}) - {cheatType}: {details} (Count: {log.Violations[cheatType]})");

			// Log to file for tuning
			if (FileLoggingEnabled)
			{
				WriteToLogFile(log, cheatType, details);
			}

			// Auto-ban threshold
			if (ShouldAutoBan(log, cheatType))
			{
				ExecuteBan(log, cheatType, details);
			}
		}
	}

	/// <summary>
	/// Execute the actual ban for a player
	/// </summary>
	private static void ExecuteBan(CheatLog log, CheatType cheatType, string details)
	{
		log.TotalBanCount++;

		// Determine ban duration based on offense history
		var duration = BanService.GetBanDuration(cheatType.ToString(), log.TotalBanCount);

		string durationText = duration.HasValue ? duration.Value.ToString() : "PERMANENT";
		Logger.Log($"[AUTO-BAN] Player {log.PlayerName} ({log.PlayerId}) banned for {durationText}. Reason: {cheatType}", LogLevel.Warning);

		// Register the ban if we have account info
		if (!string.IsNullOrEmpty(log.AccountId))
		{
			BanService.BanAccount(
				log.AccountId,
				$"Auto-ban: {cheatType}",
				cheatType.ToString(),
				log.IpAddress,
				duration,
				"AntiCheat",
				details
			);
		}

		// Kick the player immediately
		BanService.KickPlayer(log.PlayerId, $"Banned for {cheatType}");
	}

	/// <summary>
	/// Log speed hack detection
	/// </summary>
	public static void LogSpeedHack(uint playerId, string playerName, float actualSpeed, float maxSpeed, float distance, float deltaTime)
	{
		string details = $"Speed: {actualSpeed:F2} (max: {maxSpeed:F2}), Distance: {distance:F2}, DeltaTime: {deltaTime:F3}s";
		LogCheat(playerId, playerName, CheatType.SpeedHack, details);
	}

	/// <summary>
	/// Log teleport detection
	/// </summary>
	public static void LogTeleport(uint playerId, string playerName, FPOS oldPos, FPOS newPos, float distance)
	{
		string details = $"From ({oldPos.x:F2}, {oldPos.y:F2}, {oldPos.z:F2}) to ({newPos.x:F2}, {newPos.y:F2}, {newPos.z:F2}), Distance: {distance:F2}";
		LogCheat(playerId, playerName, CheatType.Teleport, details);
	}

	/// <summary>
	/// Log invalid direction vector
	/// </summary>
	public static void LogInvalidDirection(uint playerId, string playerName, FPOS direction, float length)
	{
		string details = $"Direction ({direction.x:F3}, {direction.y:F3}, {direction.z:F3}), Length: {length:F4} (expected: 1.0)";
		LogCheat(playerId, playerName, CheatType.InvalidDirection, details);
	}

	/// <summary>
	/// Log tick manipulation
	/// </summary>
	public static void LogTickManipulation(uint playerId, string playerName, uint clientTick, uint expectedTick)
	{
		string details = $"Client tick: {clientTick}, Expected: {expectedTick}, Diff: {(int)(clientTick - expectedTick)}";
		LogCheat(playerId, playerName, CheatType.TickManipulation, details);
	}

	/// <summary>
	/// Log fly hack detection (Z is height in Lime Odyssey)
	/// </summary>
	public static void LogFlyHack(uint playerId, string playerName, float oldHeight, float newHeight, float heightDelta, float maxJumpHeight)
	{
		string details = $"Height change: {heightDelta:F2} (max: {maxJumpHeight:F2}), From Z={oldHeight:F2} to Z={newHeight:F2}";
		LogCheat(playerId, playerName, CheatType.FlyHack, details);
	}

	/// <summary>
	/// Log out of bounds detection
	/// </summary>
	public static void LogOutOfBounds(uint playerId, string playerName, FPOS position, string boundaryType)
	{
		string details = $"Position ({position.x:F2}, {position.y:F2}, {position.z:F2}) outside {boundaryType}";
		LogCheat(playerId, playerName, CheatType.OutOfBounds, details);
	}

	/// <summary>
	/// Determine if player should be auto-banned
	/// </summary>
	private static bool ShouldAutoBan(CheatLog log, CheatType cheatType)
	{
		// Check if auto-ban is enabled
		if (!AutoBanEnabled)
			return false;

		// Configurable thresholds
		var thresholds = new Dictionary<CheatType, int>
		{
			{ CheatType.SpeedHack, 5 },
			{ CheatType.Teleport, 3 },
			{ CheatType.FlyHack, 3 },
			{ CheatType.InvalidDirection, 10 },
			{ CheatType.TickManipulation, 10 },
			{ CheatType.ItemDuplication, 1 }, // Zero tolerance
			{ CheatType.InvalidSkillCast, 5 },
			{ CheatType.OutOfBounds, 5 }
		};

		if (!thresholds.ContainsKey(cheatType))
			return false;

		return log.Violations[cheatType] >= thresholds[cheatType];
	}

	/// <summary>
	/// Get cheat statistics for a player
	/// </summary>
	public static Dictionary<CheatType, int>? GetPlayerViolations(uint playerId)
	{
		lock (_playerLogs)
		{
			return _playerLogs.TryGetValue(playerId, out var log) ? log.Violations : null;
		}
	}

	/// <summary>
	/// Clear cheat log for a player (use carefully)
	/// </summary>
	public static void ClearPlayerLog(uint playerId)
	{
		lock (_playerLogs)
		{
			_playerLogs.Remove(playerId);
		}
	}

	/// <summary>
	/// Write cheat detection to log file for tuning
	/// </summary>
	private static void WriteToLogFile(CheatLog log, CheatType cheatType, string details)
	{
		try
		{
			lock (_fileLock)
			{
				var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}|{log.PlayerId}|{log.PlayerName}|{log.AccountId}|{log.IpAddress}|{cheatType}|{log.Violations[cheatType]}|{details}";
				File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
			}
		}
		catch (Exception ex)
		{
			Logger.Log($"[CHEAT] Failed to write to log file: {ex.Message}", LogLevel.Warning);
		}
	}
}
