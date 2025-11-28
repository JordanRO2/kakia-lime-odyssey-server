/// <summary>
/// Service for tracking currency (Peder/Lant) transactions and audit logging.
/// </summary>
/// <remarks>
/// Tracks all currency operations: earned, spent, transferred, rewards.
/// Provides complete transaction history for any character.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Audit;

/// <summary>
/// Types of currency transactions.
/// </summary>
public enum CurrencyTransactionType
{
	/// <summary>Currency gained from monster loot</summary>
	LootDrop,
	/// <summary>Currency gained from quest reward</summary>
	QuestReward,
	/// <summary>Currency gained from selling items to NPC</summary>
	NpcSell,
	/// <summary>Currency spent buying from NPC</summary>
	NpcBuy,
	/// <summary>Currency spent on item repair</summary>
	Repair,
	/// <summary>Currency transferred to another player (trade/exchange)</summary>
	PlayerTransferOut,
	/// <summary>Currency received from another player (trade/exchange)</summary>
	PlayerTransferIn,
	/// <summary>Currency sent via mail</summary>
	MailSent,
	/// <summary>Currency received via mail</summary>
	MailReceived,
	/// <summary>Currency spent on crafting</summary>
	Crafting,
	/// <summary>Currency spent on guild operations</summary>
	GuildOperation,
	/// <summary>Currency given by GM command</summary>
	GMCommand,
	/// <summary>Currency removed by GM command</summary>
	GMRemove,
	/// <summary>Currency spent on teleport/warp</summary>
	Teleport,
	/// <summary>Currency converted to another type</summary>
	CurrencyConversion,
	/// <summary>Currency from system (unknown source)</summary>
	System
}

/// <summary>
/// Currency type being tracked.
/// </summary>
public enum CurrencyType
{
	/// <summary>Peder - main currency (gold)</summary>
	Peder,
	/// <summary>Lant - premium/secondary currency</summary>
	Lant
}

/// <summary>
/// Record of a currency transaction.
/// </summary>
public class CurrencyAuditRecord
{
	/// <summary>Unique ID of this transaction</summary>
	public Guid TransactionId { get; set; } = Guid.NewGuid();

	/// <summary>Type of currency</summary>
	public CurrencyType Currency { get; set; }

	/// <summary>Type of transaction</summary>
	public CurrencyTransactionType TransactionType { get; set; }

	/// <summary>Amount of currency (positive = gain, negative = loss)</summary>
	public long Amount { get; set; }

	/// <summary>Balance before this transaction</summary>
	public long BalanceBefore { get; set; }

	/// <summary>Balance after this transaction</summary>
	public long BalanceAfter { get; set; }

	/// <summary>When this transaction occurred</summary>
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;

	/// <summary>Character name involved</summary>
	public string CharacterName { get; set; } = string.Empty;

	/// <summary>Account ID involved</summary>
	public string AccountId { get; set; } = string.Empty;

	/// <summary>Other party involved (for transfers)</summary>
	public string? OtherParty { get; set; }

	/// <summary>Other party account ID (for transfers)</summary>
	public string? OtherPartyAccountId { get; set; }

	/// <summary>Additional details (item sold, quest name, etc)</summary>
	public string? Details { get; set; }

	/// <summary>Related item instance ID if applicable</summary>
	public Guid? RelatedItemInstanceId { get; set; }

	public override string ToString()
	{
		string sign = Amount >= 0 ? "+" : "";
		return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {TransactionType}: {CharacterName} " +
			   $"{sign}{Amount} {Currency} ({BalanceBefore} -> {BalanceAfter}) " +
			   $"OtherParty={OtherParty ?? "N/A"} Details={Details ?? "N/A"}";
	}
}

/// <summary>
/// Service for tracking currency transactions.
/// </summary>
public class CurrencyAuditService
{
	private readonly List<CurrencyAuditRecord> _auditLog = new();
	private readonly object _lock = new();

	/// <summary>
	/// Logs currency gained from loot drop.
	/// </summary>
	public void LogLootDrop(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, string? monsterName = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.LootDrop, amount, balanceBefore, balanceAfter,
			details: monsterName != null ? $"From={monsterName}" : null);
	}

	/// <summary>
	/// Logs currency gained from quest reward.
	/// </summary>
	public void LogQuestReward(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, int questId, string? questName = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.QuestReward, amount, balanceBefore, balanceAfter,
			details: $"QuestId={questId} Name={questName ?? "Unknown"}");
	}

	/// <summary>
	/// Logs currency gained from selling to NPC.
	/// </summary>
	public void LogNpcSell(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, int itemTypeId, string? itemName, ulong quantity, Guid? itemInstanceId = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.NpcSell, amount, balanceBefore, balanceAfter,
			details: $"ItemId={itemTypeId} Name={itemName ?? "Unknown"} Qty={quantity}",
			relatedItemId: itemInstanceId);
	}

	/// <summary>
	/// Logs currency spent buying from NPC.
	/// </summary>
	public void LogNpcBuy(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, int itemTypeId, string? itemName, ulong quantity, Guid? itemInstanceId = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.NpcBuy, -Math.Abs(amount), balanceBefore, balanceAfter,
			details: $"ItemId={itemTypeId} Name={itemName ?? "Unknown"} Qty={quantity}",
			relatedItemId: itemInstanceId);
	}

	/// <summary>
	/// Logs currency spent on repair.
	/// </summary>
	public void LogRepair(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, int itemTypeId, string? itemName, Guid? itemInstanceId = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.Repair, -Math.Abs(amount), balanceBefore, balanceAfter,
			details: $"ItemId={itemTypeId} Name={itemName ?? "Unknown"}",
			relatedItemId: itemInstanceId);
	}

	/// <summary>
	/// Logs currency transferred between players.
	/// </summary>
	public void LogPlayerTransfer(PlayerClient from, PlayerClient to, CurrencyType currency, long amount, long fromBalanceBefore, long fromBalanceAfter, long toBalanceBefore, long toBalanceAfter, string transferType)
	{
		var fromName = from.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var toName = to.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Log outgoing transfer
		LogTransaction(from, currency, CurrencyTransactionType.PlayerTransferOut, -Math.Abs(amount), fromBalanceBefore, fromBalanceAfter,
			otherParty: toName, otherPartyAccountId: to.GetAccountId(),
			details: $"Via={transferType}");

		// Log incoming transfer
		LogTransaction(to, currency, CurrencyTransactionType.PlayerTransferIn, Math.Abs(amount), toBalanceBefore, toBalanceAfter,
			otherParty: fromName, otherPartyAccountId: from.GetAccountId(),
			details: $"Via={transferType}");

		Logger.Log($"[AUDIT] Currency transferred: {amount} {currency} From={fromName} To={toName} Via={transferType}", LogLevel.Information);
	}

	/// <summary>
	/// Logs currency sent via mail.
	/// </summary>
	public void LogMailSent(PlayerClient sender, string recipientName, CurrencyType currency, long amount, long balanceBefore, long balanceAfter)
	{
		LogTransaction(sender, currency, CurrencyTransactionType.MailSent, -Math.Abs(amount), balanceBefore, balanceAfter,
			otherParty: recipientName,
			details: "Via=Mail");
	}

	/// <summary>
	/// Logs currency received via mail.
	/// </summary>
	public void LogMailReceived(PlayerClient recipient, string senderName, CurrencyType currency, long amount, long balanceBefore, long balanceAfter)
	{
		LogTransaction(recipient, currency, CurrencyTransactionType.MailReceived, Math.Abs(amount), balanceBefore, balanceAfter,
			otherParty: senderName,
			details: "Via=Mail");
	}

	/// <summary>
	/// Logs currency spent on crafting.
	/// </summary>
	public void LogCrafting(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, int recipeId, string? recipeName = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.Crafting, -Math.Abs(amount), balanceBefore, balanceAfter,
			details: $"RecipeId={recipeId} Name={recipeName ?? "Unknown"}");
	}

	/// <summary>
	/// Logs currency given by GM.
	/// </summary>
	public void LogGMGive(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, string gmName, string? reason = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.GMCommand, Math.Abs(amount), balanceBefore, balanceAfter,
			otherParty: gmName,
			details: reason ?? "GM Command");

		Logger.Log($"[AUDIT] GM currency give: {amount} {currency} To={player.GetCurrentCharacter()?.appearance.name} By={gmName} Reason={reason}", LogLevel.Warning);
	}

	/// <summary>
	/// Logs currency removed by GM.
	/// </summary>
	public void LogGMRemove(PlayerClient player, CurrencyType currency, long amount, long balanceBefore, long balanceAfter, string gmName, string? reason = null)
	{
		LogTransaction(player, currency, CurrencyTransactionType.GMRemove, -Math.Abs(amount), balanceBefore, balanceAfter,
			otherParty: gmName,
			details: reason ?? "GM Command");

		Logger.Log($"[AUDIT] GM currency remove: {amount} {currency} From={player.GetCurrentCharacter()?.appearance.name} By={gmName} Reason={reason}", LogLevel.Warning);
	}

	/// <summary>
	/// Logs currency conversion between Peder and Lant.
	/// </summary>
	/// <param name="player">The player</param>
	/// <param name="fromCurrency">Source currency type</param>
	/// <param name="toCurrency">Target currency type</param>
	/// <param name="fromAmount">Amount spent of source currency</param>
	/// <param name="toAmount">Amount gained of target currency</param>
	/// <param name="fromBalanceBefore">Source currency balance before</param>
	/// <param name="fromBalanceAfter">Source currency balance after</param>
	/// <param name="toBalanceBefore">Target currency balance before</param>
	/// <param name="toBalanceAfter">Target currency balance after</param>
	public void LogCurrencyConversion(
		PlayerClient player,
		CurrencyType fromCurrency,
		CurrencyType toCurrency,
		long fromAmount,
		long toAmount,
		long fromBalanceBefore,
		long fromBalanceAfter,
		long toBalanceBefore,
		long toBalanceAfter)
	{
		// Log the spent currency
		LogTransaction(player, fromCurrency, CurrencyTransactionType.CurrencyConversion, -Math.Abs(fromAmount), fromBalanceBefore, fromBalanceAfter,
			details: $"ConvertedTo={toCurrency} Amount={toAmount}");

		// Log the gained currency
		LogTransaction(player, toCurrency, CurrencyTransactionType.CurrencyConversion, Math.Abs(toAmount), toBalanceBefore, toBalanceAfter,
			details: $"ConvertedFrom={fromCurrency} Amount={fromAmount}");

		var characterName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[AUDIT] Currency conversion: {characterName} converted {fromAmount} {fromCurrency} to {toAmount} {toCurrency}", LogLevel.Information);
	}

	/// <summary>
	/// Gets all transactions for a specific character.
	/// </summary>
	public List<CurrencyAuditRecord> GetCharacterHistory(string characterName)
	{
		lock (_lock)
		{
			return _auditLog.Where(r => r.CharacterName == characterName).ToList();
		}
	}

	/// <summary>
	/// Gets all transactions for a specific account.
	/// </summary>
	public List<CurrencyAuditRecord> GetAccountHistory(string accountId)
	{
		lock (_lock)
		{
			return _auditLog.Where(r => r.AccountId == accountId).ToList();
		}
	}

	/// <summary>
	/// Gets all transactions involving two specific players.
	/// </summary>
	public List<CurrencyAuditRecord> GetTransfersBetween(string character1, string character2)
	{
		lock (_lock)
		{
			return _auditLog.Where(r =>
				(r.CharacterName == character1 && r.OtherParty == character2) ||
				(r.CharacterName == character2 && r.OtherParty == character1))
				.ToList();
		}
	}

	/// <summary>
	/// Gets recent transactions.
	/// </summary>
	public List<CurrencyAuditRecord> GetRecentTransactions(int count = 100)
	{
		lock (_lock)
		{
			return _auditLog.OrderByDescending(r => r.Timestamp).Take(count).ToList();
		}
	}

	/// <summary>
	/// Gets transactions by type.
	/// </summary>
	public List<CurrencyAuditRecord> GetTransactionsByType(CurrencyTransactionType type, int count = 100)
	{
		lock (_lock)
		{
			return _auditLog.Where(r => r.TransactionType == type)
				.OrderByDescending(r => r.Timestamp)
				.Take(count)
				.ToList();
		}
	}

	/// <summary>
	/// Gets total currency gained/lost by a character in a time period.
	/// </summary>
	public (long totalGained, long totalLost, long netChange) GetCharacterSummary(string characterName, DateTime? since = null)
	{
		lock (_lock)
		{
			var records = _auditLog.Where(r => r.CharacterName == characterName);
			if (since.HasValue)
			{
				records = records.Where(r => r.Timestamp >= since.Value);
			}

			var list = records.ToList();
			long gained = list.Where(r => r.Amount > 0).Sum(r => r.Amount);
			long lost = list.Where(r => r.Amount < 0).Sum(r => Math.Abs(r.Amount));
			return (gained, lost, gained - lost);
		}
	}

	private void LogTransaction(
		PlayerClient player,
		CurrencyType currency,
		CurrencyTransactionType type,
		long amount,
		long balanceBefore,
		long balanceAfter,
		string? otherParty = null,
		string? otherPartyAccountId = null,
		string? details = null,
		Guid? relatedItemId = null)
	{
		var characterName = player.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		var accountId = player.GetAccountId();

		var record = new CurrencyAuditRecord
		{
			Currency = currency,
			TransactionType = type,
			Amount = amount,
			BalanceBefore = balanceBefore,
			BalanceAfter = balanceAfter,
			CharacterName = characterName,
			AccountId = accountId,
			OtherParty = otherParty,
			OtherPartyAccountId = otherPartyAccountId,
			Details = details,
			RelatedItemInstanceId = relatedItemId
		};

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

		string sign = amount >= 0 ? "+" : "";
		Logger.Log($"[AUDIT] {type}: {characterName} {sign}{amount} {currency} ({balanceBefore} -> {balanceAfter})", LogLevel.Debug);
	}
}
