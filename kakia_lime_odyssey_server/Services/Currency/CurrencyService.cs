/// <summary>
/// Service for managing player currency (Peder and Lant).
/// </summary>
/// <remarks>
/// Handles all currency operations including adding, removing, and transferring currency.
/// Peder is the primary in-game currency (gold).
/// Lant is the secondary/premium currency.
/// All operations are tracked via CurrencyAuditService for complete transaction history.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Audit;

namespace kakia_lime_odyssey_server.Services.Currency;

/// <summary>
/// Result of a currency operation.
/// </summary>
public enum CurrencyResult
{
    Success,
    InsufficientFunds,
    InvalidAmount,
    PlayerNotFound
}

/// <summary>
/// Service for managing player currency transactions.
/// </summary>
public class CurrencyService
{
    /// <summary>
    /// Gets the player's current Peder (gold) balance.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <returns>Current Peder balance</returns>
    public long GetPeder(PlayerClient pc)
    {
        var inventory = pc.GetInventory();
        return inventory.WalletPeder;
    }

    /// <summary>
    /// Gets the player's current Lant (premium currency) balance.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <returns>Current Lant balance</returns>
    public long GetLant(PlayerClient pc)
    {
        var inventory = pc.GetInventory();
        return inventory.WalletLant;
    }

    /// <summary>
    /// Adds Peder to the player's wallet.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="amount">Amount to add (must be positive)</param>
    /// <returns>Result of the operation</returns>
    public CurrencyResult AddPeder(PlayerClient pc, long amount)
    {
        if (amount <= 0)
        {
            Logger.Log($"[CURRENCY] Invalid amount: {amount}", LogLevel.Warning);
            return CurrencyResult.InvalidAmount;
        }

        var inventory = pc.GetInventory();
        inventory.WalletPeder += amount;

        string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {playerName} received {amount} Peder (new balance: {inventory.WalletPeder})", LogLevel.Debug);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Removes Peder from the player's wallet.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="amount">Amount to remove (must be positive)</param>
    /// <returns>Result of the operation</returns>
    public CurrencyResult RemovePeder(PlayerClient pc, long amount)
    {
        if (amount <= 0)
        {
            Logger.Log($"[CURRENCY] Invalid amount: {amount}", LogLevel.Warning);
            return CurrencyResult.InvalidAmount;
        }

        var inventory = pc.GetInventory();

        if (inventory.WalletPeder < amount)
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} has insufficient Peder: {inventory.WalletPeder} < {amount}", LogLevel.Debug);
            return CurrencyResult.InsufficientFunds;
        }

        inventory.WalletPeder -= amount;

        string name = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {name} spent {amount} Peder (new balance: {inventory.WalletPeder})", LogLevel.Debug);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Checks if player has enough Peder.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="amount">Required amount</param>
    /// <returns>True if player has enough Peder</returns>
    public bool HasEnoughPeder(PlayerClient pc, long amount)
    {
        var inventory = pc.GetInventory();
        return inventory.WalletPeder >= amount;
    }

    /// <summary>
    /// Adds Lant (premium currency) to the player's wallet.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="amount">Amount to add (must be positive)</param>
    /// <returns>Result of the operation</returns>
    public CurrencyResult AddLant(PlayerClient pc, long amount)
    {
        if (amount <= 0)
        {
            Logger.Log($"[CURRENCY] Invalid Lant amount: {amount}", LogLevel.Warning);
            return CurrencyResult.InvalidAmount;
        }

        var inventory = pc.GetInventory();
        inventory.WalletLant += amount;

        string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {playerName} received {amount} Lant (new balance: {inventory.WalletLant})", LogLevel.Debug);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Removes Lant (premium currency) from the player's wallet.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="amount">Amount to remove (must be positive)</param>
    /// <returns>Result of the operation</returns>
    public CurrencyResult RemoveLant(PlayerClient pc, long amount)
    {
        if (amount <= 0)
        {
            Logger.Log($"[CURRENCY] Invalid Lant amount: {amount}", LogLevel.Warning);
            return CurrencyResult.InvalidAmount;
        }

        var inventory = pc.GetInventory();

        if (inventory.WalletLant < amount)
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} has insufficient Lant: {inventory.WalletLant} < {amount}", LogLevel.Debug);
            return CurrencyResult.InsufficientFunds;
        }

        inventory.WalletLant -= amount;

        string name = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {name} spent {amount} Lant (new balance: {inventory.WalletLant})", LogLevel.Debug);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Checks if player has enough Lant.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="amount">Required amount</param>
    /// <returns>True if player has enough Lant</returns>
    public bool HasEnoughLant(PlayerClient pc, long amount)
    {
        var inventory = pc.GetInventory();
        return inventory.WalletLant >= amount;
    }

    /// <summary>
    /// Transfers Peder between two players.
    /// </summary>
    /// <param name="from">Source player</param>
    /// <param name="to">Destination player</param>
    /// <param name="amount">Amount to transfer</param>
    /// <returns>Result of the operation</returns>
    public CurrencyResult TransferPeder(PlayerClient from, PlayerClient to, long amount)
    {
        if (amount <= 0)
        {
            return CurrencyResult.InvalidAmount;
        }

        var result = RemovePeder(from, amount);
        if (result != CurrencyResult.Success)
        {
            return result;
        }

        AddPeder(to, amount);

        string fromName = from.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        string toName = to.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {fromName} transferred {amount} Peder to {toName}", LogLevel.Information);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Calculates the buy price for an item (what the player pays).
    /// </summary>
    /// <param name="basePrice">Base item price</param>
    /// <param name="count">Number of items</param>
    /// <returns>Total price to pay</returns>
    public long CalculateBuyPrice(int basePrice, long count)
    {
        return basePrice * count;
    }

    /// <summary>
    /// Calculates the sell price for an item (what the player receives).
    /// </summary>
    /// <param name="basePrice">Base item price</param>
    /// <param name="count">Number of items</param>
    /// <param name="sellRatio">Ratio of base price (default 0.3 = 30%)</param>
    /// <returns>Total price player receives</returns>
    public long CalculateSellPrice(int basePrice, long count, float sellRatio = 0.3f)
    {
        return (long)(basePrice * count * sellRatio);
    }

    /// <summary>
    /// Processes a shop buy transaction.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="itemPrice">Price per item</param>
    /// <param name="count">Number of items to buy</param>
    /// <returns>True if transaction succeeded</returns>
    public bool ProcessShopBuy(PlayerClient pc, int itemPrice, long count)
    {
        long totalCost = CalculateBuyPrice(itemPrice, count);

        if (!HasEnoughPeder(pc, totalCost))
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} cannot afford {totalCost} Peder for purchase", LogLevel.Debug);
            return false;
        }

        var result = RemovePeder(pc, totalCost);
        return result == CurrencyResult.Success;
    }

    /// <summary>
    /// Processes a shop sell transaction.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="itemPrice">Base price per item</param>
    /// <param name="count">Number of items to sell</param>
    /// <returns>Amount of Peder received</returns>
    public long ProcessShopSell(PlayerClient pc, int itemPrice, long count)
    {
        long payment = CalculateSellPrice(itemPrice, count);
        AddPeder(pc, payment);
        return payment;
    }

    /// <summary>
    /// Processes a repair transaction.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="repairCost">Total repair cost</param>
    /// <returns>True if transaction succeeded</returns>
    public bool ProcessRepair(PlayerClient pc, long repairCost)
    {
        if (!HasEnoughPeder(pc, repairCost))
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} cannot afford {repairCost} Peder for repair", LogLevel.Debug);
            return false;
        }

        var result = RemovePeder(pc, repairCost);
        return result == CurrencyResult.Success;
    }

    // ============ AUDITED OPERATIONS ============
    // These methods include full audit logging for tracking

    /// <summary>
    /// Adds Peder from loot drop with audit logging.
    /// </summary>
    public CurrencyResult AddPederFromLoot(PlayerClient pc, long amount, string? monsterName = null)
    {
        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        var result = AddPeder(pc, amount);
        if (result == CurrencyResult.Success)
        {
            long balanceAfter = inventory.WalletPeder;
            LimeServer.CurrencyAuditService.LogLootDrop(pc, CurrencyType.Peder, amount, balanceBefore, balanceAfter, monsterName);
        }
        return result;
    }

    /// <summary>
    /// Adds Peder from quest reward with audit logging.
    /// </summary>
    public CurrencyResult AddPederFromQuest(PlayerClient pc, long amount, int questId, string? questName = null)
    {
        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        var result = AddPeder(pc, amount);
        if (result == CurrencyResult.Success)
        {
            long balanceAfter = inventory.WalletPeder;
            LimeServer.CurrencyAuditService.LogQuestReward(pc, CurrencyType.Peder, amount, balanceBefore, balanceAfter, questId, questName);
        }
        return result;
    }

    /// <summary>
    /// Processes a shop buy transaction with audit logging.
    /// </summary>
    public bool ProcessShopBuyAudited(PlayerClient pc, int itemTypeId, string? itemName, int itemPrice, long count, Guid? itemInstanceId = null)
    {
        long totalCost = CalculateBuyPrice(itemPrice, count);

        if (!HasEnoughPeder(pc, totalCost))
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} cannot afford {totalCost} Peder for purchase", LogLevel.Debug);
            return false;
        }

        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        var result = RemovePeder(pc, totalCost);
        if (result == CurrencyResult.Success)
        {
            long balanceAfter = inventory.WalletPeder;
            LimeServer.CurrencyAuditService.LogNpcBuy(pc, CurrencyType.Peder, totalCost, balanceBefore, balanceAfter, itemTypeId, itemName, (ulong)count, itemInstanceId);
        }
        return result == CurrencyResult.Success;
    }

    /// <summary>
    /// Processes a shop sell transaction with audit logging.
    /// </summary>
    public long ProcessShopSellAudited(PlayerClient pc, int itemTypeId, string? itemName, int itemPrice, long count, Guid? itemInstanceId = null)
    {
        long payment = CalculateSellPrice(itemPrice, count);
        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        AddPeder(pc, payment);
        long balanceAfter = inventory.WalletPeder;
        LimeServer.CurrencyAuditService.LogNpcSell(pc, CurrencyType.Peder, payment, balanceBefore, balanceAfter, itemTypeId, itemName, (ulong)count, itemInstanceId);
        return payment;
    }

    /// <summary>
    /// Processes a repair transaction with audit logging.
    /// </summary>
    public bool ProcessRepairAudited(PlayerClient pc, long repairCost, int itemTypeId, string? itemName, Guid? itemInstanceId = null)
    {
        if (!HasEnoughPeder(pc, repairCost))
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} cannot afford {repairCost} Peder for repair", LogLevel.Debug);
            return false;
        }

        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        var result = RemovePeder(pc, repairCost);
        if (result == CurrencyResult.Success)
        {
            long balanceAfter = inventory.WalletPeder;
            LimeServer.CurrencyAuditService.LogRepair(pc, CurrencyType.Peder, repairCost, balanceBefore, balanceAfter, itemTypeId, itemName, itemInstanceId);
        }
        return result == CurrencyResult.Success;
    }

    /// <summary>
    /// Transfers Peder between two players with audit logging.
    /// </summary>
    public CurrencyResult TransferPederAudited(PlayerClient from, PlayerClient to, long amount, string transferType)
    {
        if (amount <= 0)
        {
            return CurrencyResult.InvalidAmount;
        }

        var fromInventory = from.GetInventory();
        var toInventory = to.GetInventory();

        long fromBalanceBefore = fromInventory.WalletPeder;
        long toBalanceBefore = toInventory.WalletPeder;

        var result = RemovePeder(from, amount);
        if (result != CurrencyResult.Success)
        {
            return result;
        }

        AddPeder(to, amount);

        long fromBalanceAfter = fromInventory.WalletPeder;
        long toBalanceAfter = toInventory.WalletPeder;

        LimeServer.CurrencyAuditService.LogPlayerTransfer(from, to, CurrencyType.Peder, amount, fromBalanceBefore, fromBalanceAfter, toBalanceBefore, toBalanceAfter, transferType);

        string fromName = from.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        string toName = to.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {fromName} transferred {amount} Peder to {toName} via {transferType}", LogLevel.Information);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Removes Peder for crafting with audit logging.
    /// </summary>
    public CurrencyResult RemovePederForCrafting(PlayerClient pc, long amount, int recipeId, string? recipeName = null)
    {
        if (!HasEnoughPeder(pc, amount))
        {
            return CurrencyResult.InsufficientFunds;
        }

        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        var result = RemovePeder(pc, amount);
        if (result == CurrencyResult.Success)
        {
            long balanceAfter = inventory.WalletPeder;
            LimeServer.CurrencyAuditService.LogCrafting(pc, CurrencyType.Peder, amount, balanceBefore, balanceAfter, recipeId, recipeName);
        }
        return result;
    }

    /// <summary>
    /// Adds Peder via GM command with audit logging.
    /// </summary>
    public CurrencyResult GMAddPeder(PlayerClient pc, long amount, string gmName, string? reason = null)
    {
        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        var result = AddPeder(pc, amount);
        if (result == CurrencyResult.Success)
        {
            long balanceAfter = inventory.WalletPeder;
            LimeServer.CurrencyAuditService.LogGMGive(pc, CurrencyType.Peder, amount, balanceBefore, balanceAfter, gmName, reason);
        }
        return result;
    }

    /// <summary>
    /// Removes Peder via GM command with audit logging.
    /// </summary>
    public CurrencyResult GMRemovePeder(PlayerClient pc, long amount, string gmName, string? reason = null)
    {
        var inventory = pc.GetInventory();
        long balanceBefore = inventory.WalletPeder;
        var result = RemovePeder(pc, amount);
        if (result == CurrencyResult.Success)
        {
            long balanceAfter = inventory.WalletPeder;
            LimeServer.CurrencyAuditService.LogGMRemove(pc, CurrencyType.Peder, amount, balanceBefore, balanceAfter, gmName, reason);
        }
        return result;
    }

    // ============ CURRENCY CONVERSION ============

    /// <summary>
    /// Exchange rate: 1 Lant = 100 Peder (100:1).
    /// </summary>
    public const long LantToPederRate = 100;

    /// <summary>
    /// Converts Lant to Peder at the standard exchange rate.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="lantAmount">Amount of Lant to convert</param>
    /// <returns>Result of the operation</returns>
    public CurrencyResult ConvertLantToPeder(PlayerClient pc, long lantAmount)
    {
        if (lantAmount <= 0)
        {
            Logger.Log($"[CURRENCY] Invalid Lant conversion amount: {lantAmount}", LogLevel.Warning);
            return CurrencyResult.InvalidAmount;
        }

        var inventory = pc.GetInventory();

        if (inventory.WalletLant < lantAmount)
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} has insufficient Lant for conversion: {inventory.WalletLant} < {lantAmount}", LogLevel.Debug);
            return CurrencyResult.InsufficientFunds;
        }

        long pederAmount = lantAmount * LantToPederRate;

        // Remove Lant
        inventory.WalletLant -= lantAmount;

        // Add Peder
        inventory.WalletPeder += pederAmount;

        string name = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {name} converted {lantAmount} Lant to {pederAmount} Peder (Lant: {inventory.WalletLant}, Peder: {inventory.WalletPeder})", LogLevel.Information);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Converts Peder to Lant at the standard exchange rate.
    /// </summary>
    /// <param name="pc">The player client</param>
    /// <param name="pederAmount">Amount of Peder to convert (must be divisible by exchange rate)</param>
    /// <returns>Result of the operation</returns>
    public CurrencyResult ConvertPederToLant(PlayerClient pc, long pederAmount)
    {
        if (pederAmount <= 0)
        {
            Logger.Log($"[CURRENCY] Invalid Peder conversion amount: {pederAmount}", LogLevel.Warning);
            return CurrencyResult.InvalidAmount;
        }

        // Must be divisible by exchange rate
        if (pederAmount % LantToPederRate != 0)
        {
            Logger.Log($"[CURRENCY] Peder amount {pederAmount} not divisible by exchange rate {LantToPederRate}", LogLevel.Warning);
            return CurrencyResult.InvalidAmount;
        }

        var inventory = pc.GetInventory();

        if (inventory.WalletPeder < pederAmount)
        {
            string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
            Logger.Log($"[CURRENCY] {playerName} has insufficient Peder for conversion: {inventory.WalletPeder} < {pederAmount}", LogLevel.Debug);
            return CurrencyResult.InsufficientFunds;
        }

        long lantAmount = pederAmount / LantToPederRate;

        // Remove Peder
        inventory.WalletPeder -= pederAmount;

        // Add Lant
        inventory.WalletLant += lantAmount;

        string name = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
        Logger.Log($"[CURRENCY] {name} converted {pederAmount} Peder to {lantAmount} Lant (Peder: {inventory.WalletPeder}, Lant: {inventory.WalletLant})", LogLevel.Information);

        return CurrencyResult.Success;
    }

    /// <summary>
    /// Calculates how much Peder a given amount of Lant converts to.
    /// </summary>
    /// <param name="lantAmount">Lant amount</param>
    /// <returns>Equivalent Peder amount</returns>
    public static long CalculateLantToPeder(long lantAmount)
    {
        return lantAmount * LantToPederRate;
    }

    /// <summary>
    /// Calculates how much Lant a given amount of Peder converts to.
    /// </summary>
    /// <param name="pederAmount">Peder amount</param>
    /// <returns>Equivalent Lant amount (truncated)</returns>
    public static long CalculatePederToLant(long pederAmount)
    {
        return pederAmount / LantToPederRate;
    }

    /// <summary>
    /// Converts Lant to Peder with audit logging.
    /// </summary>
    public CurrencyResult ConvertLantToPederAudited(PlayerClient pc, long lantAmount)
    {
        var inventory = pc.GetInventory();
        long lantBefore = inventory.WalletLant;
        long pederBefore = inventory.WalletPeder;

        var result = ConvertLantToPeder(pc, lantAmount);

        if (result == CurrencyResult.Success)
        {
            long lantAfter = inventory.WalletLant;
            long pederAfter = inventory.WalletPeder;
            long pederGained = lantAmount * LantToPederRate;

            LimeServer.CurrencyAuditService.LogCurrencyConversion(
                pc, CurrencyType.Lant, CurrencyType.Peder,
                lantAmount, pederGained,
                lantBefore, lantAfter, pederBefore, pederAfter);
        }

        return result;
    }

    /// <summary>
    /// Converts Peder to Lant with audit logging.
    /// </summary>
    public CurrencyResult ConvertPederToLantAudited(PlayerClient pc, long pederAmount)
    {
        var inventory = pc.GetInventory();
        long pederBefore = inventory.WalletPeder;
        long lantBefore = inventory.WalletLant;

        var result = ConvertPederToLant(pc, pederAmount);

        if (result == CurrencyResult.Success)
        {
            long pederAfter = inventory.WalletPeder;
            long lantAfter = inventory.WalletLant;
            long lantGained = pederAmount / LantToPederRate;

            LimeServer.CurrencyAuditService.LogCurrencyConversion(
                pc, CurrencyType.Peder, CurrencyType.Lant,
                pederAmount, lantGained,
                pederBefore, pederAfter, lantBefore, lantAfter);
        }

        return result;
    }
}
