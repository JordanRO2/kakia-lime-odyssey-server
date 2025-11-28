using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Exchange;

/// <summary>
/// Represents an active exchange session between two players.
/// </summary>
public class ExchangeSession
{
	/// <summary>Unique session identifier</summary>
	public long SessionId { get; set; }

	/// <summary>First player in the exchange (requester)</summary>
	public PlayerClient Player1 { get; set; } = null!;

	/// <summary>Second player in the exchange (target)</summary>
	public PlayerClient Player2 { get; set; } = null!;

	/// <summary>Items offered by Player1</summary>
	public List<ExchangeItem> Player1Items { get; set; } = new();

	/// <summary>Items offered by Player2</summary>
	public List<ExchangeItem> Player2Items { get; set; } = new();

	/// <summary>Peder (gold) offered by Player1</summary>
	public long Player1Peder { get; set; }

	/// <summary>Peder (gold) offered by Player2</summary>
	public long Player2Peder { get; set; }

	/// <summary>Whether Player1 has marked ready</summary>
	public bool Player1Ready { get; set; }

	/// <summary>Whether Player2 has marked ready</summary>
	public bool Player2Ready { get; set; }

	/// <summary>Whether Player1 has confirmed final OK</summary>
	public bool Player1Ok { get; set; }

	/// <summary>Whether Player2 has confirmed final OK</summary>
	public bool Player2Ok { get; set; }

	/// <summary>When the session was created</summary>
	public DateTime CreatedAt { get; set; }

	public PlayerClient? GetOtherPlayer(PlayerClient player)
	{
		if (player == Player1) return Player2;
		if (player == Player2) return Player1;
		return null;
	}

	public List<ExchangeItem> GetPlayerItems(PlayerClient player)
	{
		if (player == Player1) return Player1Items;
		if (player == Player2) return Player2Items;
		return new List<ExchangeItem>();
	}

	public bool IsPlayerReady(PlayerClient player)
	{
		if (player == Player1) return Player1Ready;
		if (player == Player2) return Player2Ready;
		return false;
	}

	public void SetPlayerReady(PlayerClient player, bool ready)
	{
		if (player == Player1) Player1Ready = ready;
		else if (player == Player2) Player2Ready = ready;
	}

	public bool IsPlayerOk(PlayerClient player)
	{
		if (player == Player1) return Player1Ok;
		if (player == Player2) return Player2Ok;
		return false;
	}

	public void SetPlayerOk(PlayerClient player, bool ok)
	{
		if (player == Player1) Player1Ok = ok;
		else if (player == Player2) Player2Ok = ok;
	}

	public bool BothReady => Player1Ready && Player2Ready;
	public bool BothOk => Player1Ok && Player2Ok;

	/// <summary>
	/// Gets the Peder amount offered by a player.
	/// </summary>
	public long GetPlayerPeder(PlayerClient player)
	{
		if (player == Player1) return Player1Peder;
		if (player == Player2) return Player2Peder;
		return 0;
	}

	/// <summary>
	/// Sets the Peder amount offered by a player.
	/// </summary>
	public void SetPlayerPeder(PlayerClient player, long amount)
	{
		if (player == Player1) Player1Peder = amount;
		else if (player == Player2) Player2Peder = amount;
	}

	public void ResetReady()
	{
		Player1Ready = false;
		Player2Ready = false;
		Player1Ok = false;
		Player2Ok = false;
	}
}

/// <summary>
/// Represents an item added to the exchange window.
/// </summary>
public class ExchangeItem
{
	/// <summary>Slot in the exchange window (0-based)</summary>
	public int Slot { get; set; }

	/// <summary>Original inventory slot where this item came from</summary>
	public int OriginalInventorySlot { get; set; }

	/// <summary>Item type ID from ItemInfo</summary>
	public int ItemTypeID { get; set; }

	/// <summary>Amount being traded</summary>
	public long Count { get; set; }

	/// <summary>Current durability</summary>
	public int Durability { get; set; }

	/// <summary>Maximum durability</summary>
	public int MaxDurability { get; set; }

	/// <summary>Item grade/enchant level</summary>
	public int Grade { get; set; }
}

public class ExchangeRequest
{
	public PlayerClient Requester { get; set; } = null!;
	public PlayerClient Target { get; set; } = null!;
	public DateTime SentAt { get; set; }
	public bool IsExpired => DateTime.Now - SentAt > TimeSpan.FromSeconds(30);
}
