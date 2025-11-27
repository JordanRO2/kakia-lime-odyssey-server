using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.Persistence;

namespace kakia_lime_odyssey_server.Database;

/// <summary>
/// Database interface for player and game data persistence
/// </summary>
public interface IDatabase
{
    // Account operations
    AccountDocument? GetAccount(string accountId);
    AccountDocument? GetAccountByEmail(string email);
    void SaveAccount(AccountDocument account);
    bool DeleteAccount(string accountId);
    bool AccountExists(string accountId);
    bool EmailExists(string email);

    // Ban operations
    List<BanRecord> GetActiveBans();
    List<BanRecord> GetBanHistory(string accountId);
    void SaveBan(BanRecord ban);
    bool RemoveBan(string accountId);

    // Player character operations
    List<CLIENT_PC> LoadPC(string accountId);

    // Inventory
    PlayerInventory GetPlayerInventory(string accountId, string character);
    void SavePlayerInventory(string accountId, string character, PlayerInventory inventory);

    // Equipment
    PlayerEquips GetPlayerEquipment(string accountId, string character);
    void SavePlayerEquipment(string accountId, string character, PlayerEquips equipment);

    // World Position
    WorldPosition GetWorldPosition(string accountId, string character);
    void SaveWorldPosition(string accountId, string character, WorldPosition worldPosition);

    // Appearance
    APPEARANCE_PC_KR GetAppearance(string accountId, string character);
    void StoreAppearance(string accountId, string character, APPEARANCE_PC_KR data);

    // Status
    SAVED_STATUS_PC_KR GetSavedStatusPC(string accountId, string character);
    void StoreSavedStatusPC(string accountId, string character, SAVED_STATUS_PC_KR data);

    // Skills
    PlayerSkills GetPlayerSkills(string accountId, string character);
    void SavePlayerSkills(string accountId, string character, PlayerSkills skills);

    // Quests
    PlayerQuests GetPlayerQuests(string accountId, string character);
    void SavePlayerQuests(string accountId, string character, PlayerQuests quests);

    // Bank
    PlayerBank GetPlayerBank(string accountId, string character);
    void SavePlayerBank(string accountId, string character, PlayerBank bank);

    // Mail
    PlayerMail GetPlayerMail(string accountId, string character);
    void SavePlayerMail(string accountId, string character, PlayerMail mail);
    bool SendMail(MailMessage mail);

    // Social
    PlayerSocial GetPlayerSocial(string accountId, string character);
    void SavePlayerSocial(string accountId, string character, PlayerSocial social);

    // Guild
    List<GuildData> LoadAllGuilds();
    GuildData? GetGuild(int guildId);
    GuildData? GetGuildByName(string guildName);
    void SaveGuild(GuildData guild);
    int GetNextGuildId();
    bool DeleteGuild(int guildId);
    GuildData? GetCharacterGuild(string characterName);
}

/// <summary>
/// Database factory for creating database instances
/// </summary>
public static class DatabaseFactory
{
    private static IDatabase? _instance;
    private static readonly object _lock = new();

    public static IDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= CreateDatabase();
                }
            }
            return _instance;
        }
    }

    private static IDatabase CreateDatabase()
    {
        var mongoDb = new MongoDBService();
        Console.WriteLine("[Database] Connected to MongoDB at 172.22.0.1:27017");
        return mongoDb;
    }

    /// <summary>
    /// Force use of a specific database type
    /// </summary>
    public static void SetDatabase(IDatabase database)
    {
        lock (_lock)
        {
            _instance = database;
        }
    }
}
