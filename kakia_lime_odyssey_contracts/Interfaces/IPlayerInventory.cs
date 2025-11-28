using kakia_lime_odyssey_packets.Packets.SC;

namespace kakia_lime_odyssey_contracts.Interfaces;

public interface IPlayerInventory
{
    int FreeSlotsCount();
    IItem? AtSlot(int slot);
    SC_INVENTORY_ITEM_LIST AsInventoryPacket();
    bool RemoveItem(int slot);
    int FindItem(long id, ulong maxStackSize);
    void UpdateItemAtSlot(int slot, IItem item);

    /// <summary>
    /// Adds an item to the inventory.
    /// If slot is -1, uses first available free slot.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slot">Target slot index, or -1 for first free slot</param>
    /// <returns>The slot index where item was placed, or -1 if failed</returns>
    int AddItem(IItem item, int slot = -1);

    /// <summary>
    /// Gets or sets the player's Peder (gold) balance.
    /// </summary>
    long WalletPeder { get; set; }

    /// <summary>
    /// Gets or sets the player's Lant (premium currency) balance.
    /// </summary>
    long WalletLant { get; set; }
}
