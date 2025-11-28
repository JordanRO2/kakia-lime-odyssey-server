/// <summary>
/// Service for managing player currency (Peder and Lant).
/// </summary>
/// <remarks>
/// Handles all currency operations including adding, removing, and transferring currency.
/// Peder is the primary in-game currency (gold).
/// Lant is the secondary/premium currency.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Network;

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
}
