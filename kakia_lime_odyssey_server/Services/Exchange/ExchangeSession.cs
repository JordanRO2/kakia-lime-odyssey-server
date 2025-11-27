using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Exchange;

public class ExchangeSession
{
	public long SessionId { get; set; }
	public PlayerClient Player1 { get; set; } = null!;
	public PlayerClient Player2 { get; set; } = null!;
	public List<ExchangeItem> Player1Items { get; set; } = new();
	public List<ExchangeItem> Player2Items { get; set; } = new();
	public bool Player1Ready { get; set; }
	public bool Player2Ready { get; set; }
	public bool Player1Ok { get; set; }
	public bool Player2Ok { get; set; }
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

	public void ResetReady()
	{
		Player1Ready = false;
		Player2Ready = false;
		Player1Ok = false;
		Player2Ok = false;
	}
}

public class ExchangeItem
{
	public int Slot { get; set; }
	public int ItemTypeID { get; set; }
	public long Count { get; set; }
	public int Durability { get; set; }
	public int MaxDurability { get; set; }
	public int Grade { get; set; }
}

public class ExchangeRequest
{
	public PlayerClient Requester { get; set; } = null!;
	public PlayerClient Target { get; set; } = null!;
	public DateTime SentAt { get; set; }
	public bool IsExpired => DateTime.Now - SentAt > TimeSpan.FromSeconds(30);
}
