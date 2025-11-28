using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;

namespace kakia_lime_odyssey_contracts.Interfaces;

/// <summary>
/// Quest progress information for a player
/// </summary>
public interface IPlayerQuests
{
    /// <summary>Number of active quests</summary>
    int ActiveQuestCount { get; }

    /// <summary>Number of completed main story quests</summary>
    int CompletedMainQuests { get; }

    /// <summary>Number of completed side quests</summary>
    int CompletedSubQuests { get; }

    /// <summary>Number of completed normal quests</summary>
    int CompletedNormalQuests { get; }

    /// <summary>Get all active quest IDs</summary>
    IEnumerable<int> GetActiveQuestIds();

    /// <summary>Check if a quest is active</summary>
    bool HasQuest(int questId);

    /// <summary>Check if a quest has been completed</summary>
    bool IsQuestCompleted(int questId);
}

public interface IPlayerClient
{
    Task<bool> Send(byte[] packet, CancellationToken token);
    Task<bool> Send(PacketType header, byte[] packet, CancellationToken token);
    Task<bool> SendGlobalPacket(byte[] packet, CancellationToken token);

    bool IsConnected();

    bool IsLoaded();
    void SetUnloaded();
    void SetLoaded();
    void SetEquipToCurrentJob();
    void ChangeJob(int jobId);

    int GetClientRevision();
    void SetClientRevision(int rev);

    void SetAccountId(string accountId);
    string GetAccountId();
    uint GetObjInstID();
    void SetCurrentCharacter(CLIENT_PC pc);
    void SetCurrentTarget(long target);
    long GetCurrentTarget();

    void InitCombat();
    bool InCombat();
    void StopCombat();

    IPlayerInventory GetInventory();
    IPlayerEquipment GetEquipment(bool combat);
    IPlayerBank GetBank();
    IPlayerQuests GetQuests();

    void SendInventory();
    void SendEquipment();

    FPOS GetPosition();
    FPOS GetDirection();
    void UpdatePosition(FPOS pos);
    void UpdateDirection(FPOS dir);

    VELOCITIES GetVelocities();
    void UpdateVelocities(VELOCITIES vel);

    void SetInMotion(bool inMotion);
    bool IsInMotion();

    uint GetZone();
    void SetZone(uint zone);

    ModClientPC GetCurrentCharacter();
    REGION_PC GetRegionPC();
    SC_ENTER_SIGHT_PC GetEnterSight();
    Task RequestPresence(CancellationToken token);

    COMMON_STATUS GetStatus();
    void UpdateStatus(ModCommonStatus status);
    COMMON_STATUS RequestCommonStatus(long id);

    bool KnowOf(uint id);
    void Seen(uint id);
    void PcLeft(uint id);
    void AddNpcOrMob(INpc npc);
}
