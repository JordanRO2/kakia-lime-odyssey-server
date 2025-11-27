using kakia_lime_odyssey_contracts.Enums;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;

namespace kakia_lime_odyssey_contracts.Interfaces;

public interface IPlayerEquipment
{
    bool IsEquipped(EQUIP_SLOT slot);
    IItem? UnEquip(EQUIP_SLOT slot);
    bool Equip(EQUIP_SLOT slot, IItem item);
    SC_COMBAT_JOB_EQUIP_ITEM_LIST GetCombatEquipList();
    IItem? GetItemInSlot(EQUIP_SLOT slot);
    EQUIPPED GetEquipped();
    int GetInheritBonus(InheritType inheritType);
}
