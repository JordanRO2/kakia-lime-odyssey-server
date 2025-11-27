namespace kakia_lime_odyssey_contracts.Interfaces;

public interface IItem
{
    ulong GetId();
    void UpdateAmount(ulong amount);
    ulong GetAmount();
    bool Stackable();
}
