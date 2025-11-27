namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Persisted bank/storage data for a character
/// </summary>
public class PlayerBank
{
	/// <summary>Bank storage slots (slot number -> item)</summary>
	public Dictionary<int, Item> Items { get; set; } = new();

	/// <summary>Number of unlocked bank slots (default: 30, can expand)</summary>
	public int UnlockedSlots { get; set; } = 30;

	/// <summary>Currency stored in bank (Peder)</summary>
	public long StoredPeder { get; set; }

	/// <summary>Secondary currency stored in bank (Lant)</summary>
	public long StoredLant { get; set; }
}
