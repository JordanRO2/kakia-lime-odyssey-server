namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Represents a ban record for an account or character
/// </summary>
public class BanRecord
{
	/// <summary>Account ID of the banned player</summary>
	public string AccountId { get; set; } = string.Empty;

	/// <summary>Character name if ban is character-specific (null for account-wide)</summary>
	public string? CharacterName { get; set; }

	/// <summary>Reason for the ban</summary>
	public string Reason { get; set; } = string.Empty;

	/// <summary>Type of cheat that triggered the ban</summary>
	public string CheatType { get; set; } = string.Empty;

	/// <summary>When the ban was issued</summary>
	public DateTime BannedAt { get; set; } = DateTime.UtcNow;

	/// <summary>When the ban expires (null for permanent)</summary>
	public DateTime? ExpiresAt { get; set; }

	/// <summary>IP address at time of ban</summary>
	public string IpAddress { get; set; } = string.Empty;

	/// <summary>Who issued the ban (system or GM name)</summary>
	public string IssuedBy { get; set; } = "System";

	/// <summary>Additional details about the violation</summary>
	public string Details { get; set; } = string.Empty;

	/// <summary>Whether the ban is currently active</summary>
	public bool IsActive => ExpiresAt == null || ExpiresAt > DateTime.UtcNow;
}
