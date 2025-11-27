using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models.Persistence;

namespace kakia_lime_odyssey_server.Services.Ban;

/// <summary>
/// Service for managing player bans
/// </summary>
public static class BanService
{
	private static readonly Dictionary<string, BanRecord> _activeBans = new();
	private static readonly object _lock = new();

	// Event for notifying when a player should be kicked
	public static event Action<uint, string>? OnPlayerBanned;

	/// <summary>
	/// Ban an account for a specific duration
	/// </summary>
	public static BanRecord BanAccount(
		string accountId,
		string reason,
		string cheatType,
		string ipAddress,
		TimeSpan? duration = null,
		string issuedBy = "System",
		string details = "")
	{
		var ban = new BanRecord
		{
			AccountId = accountId,
			Reason = reason,
			CheatType = cheatType,
			IpAddress = ipAddress,
			ExpiresAt = duration.HasValue ? DateTime.UtcNow.Add(duration.Value) : null,
			IssuedBy = issuedBy,
			Details = details
		};

		lock (_lock)
		{
			_activeBans[accountId] = ban;
		}

		Logger.Log($"[BAN] Account {accountId} banned. Reason: {reason}, Expires: {ban.ExpiresAt?.ToString() ?? "Never"}", LogLevel.Warning);

		return ban;
	}

	/// <summary>
	/// Check if an account is currently banned
	/// </summary>
	public static bool IsAccountBanned(string accountId)
	{
		lock (_lock)
		{
			if (!_activeBans.TryGetValue(accountId, out var ban))
				return false;

			if (!ban.IsActive)
			{
				_activeBans.Remove(accountId);
				return false;
			}

			return true;
		}
	}

	/// <summary>
	/// Get the ban record for an account
	/// </summary>
	public static BanRecord? GetBanRecord(string accountId)
	{
		lock (_lock)
		{
			return _activeBans.TryGetValue(accountId, out var ban) && ban.IsActive ? ban : null;
		}
	}

	/// <summary>
	/// Remove a ban from an account
	/// </summary>
	public static bool UnbanAccount(string accountId)
	{
		lock (_lock)
		{
			if (_activeBans.Remove(accountId))
			{
				Logger.Log($"[UNBAN] Account {accountId} has been unbanned", LogLevel.Information);
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Trigger a kick for a player (called by CheatDetection)
	/// </summary>
	public static void KickPlayer(uint playerId, string reason)
	{
		Logger.Log($"[KICK] Requesting kick for player {playerId}: {reason}", LogLevel.Warning);
		OnPlayerBanned?.Invoke(playerId, reason);
	}

	/// <summary>
	/// Get all active bans
	/// </summary>
	public static IEnumerable<BanRecord> GetAllActiveBans()
	{
		lock (_lock)
		{
			return _activeBans.Values.Where(b => b.IsActive).ToList();
		}
	}

	/// <summary>
	/// Clean up expired bans
	/// </summary>
	public static int CleanupExpiredBans()
	{
		int removed = 0;
		lock (_lock)
		{
			var expired = _activeBans.Where(kvp => !kvp.Value.IsActive).Select(kvp => kvp.Key).ToList();
			foreach (var key in expired)
			{
				_activeBans.Remove(key);
				removed++;
			}
		}
		if (removed > 0)
			Logger.Log($"[BAN] Cleaned up {removed} expired bans", LogLevel.Debug);
		return removed;
	}

	/// <summary>
	/// Get ban duration based on cheat type and offense count
	/// </summary>
	public static TimeSpan? GetBanDuration(string cheatType, int offenseCount)
	{
		// First offense: warning duration
		// Second offense: longer ban
		// Third+ offense: permanent
		return offenseCount switch
		{
			1 => cheatType switch
			{
				"ItemDuplication" => null, // Permanent for duplication
				"Teleport" => TimeSpan.FromHours(24),
				"SpeedHack" => TimeSpan.FromHours(12),
				"FlyHack" => TimeSpan.FromHours(24),
				_ => TimeSpan.FromHours(6)
			},
			2 => cheatType switch
			{
				"ItemDuplication" => null,
				_ => TimeSpan.FromDays(7)
			},
			_ => null // Permanent after 3rd offense
		};
	}
}
