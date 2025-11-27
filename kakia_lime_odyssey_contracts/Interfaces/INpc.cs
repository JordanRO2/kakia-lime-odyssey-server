namespace kakia_lime_odyssey_contracts.Interfaces;

public interface INpc
{
    NpcType GetNpcType();
}

public enum NpcType
{
    Npc,
    Mob
}
