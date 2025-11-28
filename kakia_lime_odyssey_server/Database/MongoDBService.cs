using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.Persistence;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace kakia_lime_odyssey_server.Database;

public class MongoDBService : IDatabase
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<PlayerDocument> _players;
    private readonly IMongoCollection<GuildDocument> _guilds;
    private readonly IMongoCollection<AccountDocument> _accounts;
    private readonly IMongoCollection<BanDocument> _bans;

    private static MongoDBService? _instance;
    public static MongoDBService Instance => _instance ??= new MongoDBService();

    /// <summary>
    /// Gets a collection by name for custom document types.
    /// </summary>
    /// <typeparam name="T">Document type.</typeparam>
    /// <param name="collectionName">Name of the collection.</param>
    /// <returns>MongoDB collection.</returns>
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

    public MongoDBService(string connectionString = "mongodb://172.22.0.1:27017", string databaseName = "lime_odyssey")
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);

        _players = _database.GetCollection<PlayerDocument>("players");
        _guilds = _database.GetCollection<GuildDocument>("guilds");
        _accounts = _database.GetCollection<AccountDocument>("accounts");
        _bans = _database.GetCollection<BanDocument>("bans");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        // Player indexes
        var playerIndexes = Builders<PlayerDocument>.IndexKeys;
        _players.Indexes.CreateOne(new CreateIndexModel<PlayerDocument>(
            playerIndexes.Ascending(p => p.AccountId)));
        _players.Indexes.CreateOne(new CreateIndexModel<PlayerDocument>(
            playerIndexes.Ascending(p => p.CharacterName)));

        // Guild indexes
        var guildIndexes = Builders<GuildDocument>.IndexKeys;
        _guilds.Indexes.CreateOne(new CreateIndexModel<GuildDocument>(
            guildIndexes.Ascending(g => g.GuildId)));
        _guilds.Indexes.CreateOne(new CreateIndexModel<GuildDocument>(
            guildIndexes.Ascending(g => g.Name)));

        // Account indexes
        var accountIndexes = Builders<AccountDocument>.IndexKeys;
        _accounts.Indexes.CreateOne(new CreateIndexModel<AccountDocument>(
            accountIndexes.Ascending(a => a.AccountId),
            new CreateIndexOptions { Unique = true }));
        _accounts.Indexes.CreateOne(new CreateIndexModel<AccountDocument>(
            accountIndexes.Ascending(a => a.Email)));

        // Ban indexes
        var banIndexes = Builders<BanDocument>.IndexKeys;
        _bans.Indexes.CreateOne(new CreateIndexModel<BanDocument>(
            banIndexes.Ascending(b => b.AccountId)));
        _bans.Indexes.CreateOne(new CreateIndexModel<BanDocument>(
            banIndexes.Ascending(b => b.IsActive)));
    }

    #region Player Operations

    public async Task<List<CLIENT_PC>> LoadPCAsync(string accountId)
    {
        var filter = Builders<PlayerDocument>.Filter.Eq(p => p.AccountId, accountId);
        var players = await _players.Find(filter).ToListAsync();

        return players.Select(p => new CLIENT_PC
        {
            appearance = p.Appearance,
            status = p.Status
        }).ToList();
    }

    public List<CLIENT_PC> LoadPC(string accountId)
    {
        return LoadPCAsync(accountId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets all characters for an account (alias for LoadPC).
    /// </summary>
    public List<CLIENT_PC> GetCharacters(string accountId)
    {
        return LoadPC(accountId);
    }

    /// <summary>
    /// Deletes a character from the database.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="characterName">The character name to delete</param>
    /// <returns>True if character was deleted, false otherwise</returns>
    public async Task<bool> DeleteCharacterAsync(string accountId, string characterName)
    {
        var filter = Builders<PlayerDocument>.Filter.And(
            Builders<PlayerDocument>.Filter.Eq(p => p.AccountId, accountId),
            Builders<PlayerDocument>.Filter.Eq(p => p.CharacterName, characterName)
        );
        var result = await _players.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Deletes a character from the database (synchronous).
    /// </summary>
    public bool DeleteCharacter(string accountId, string characterName)
    {
        return DeleteCharacterAsync(accountId, characterName).GetAwaiter().GetResult();
    }

    public async Task<PlayerDocument?> GetPlayerAsync(string accountId, string characterName)
    {
        var filter = Builders<PlayerDocument>.Filter.And(
            Builders<PlayerDocument>.Filter.Eq(p => p.AccountId, accountId),
            Builders<PlayerDocument>.Filter.Eq(p => p.CharacterName, characterName)
        );
        return await _players.Find(filter).FirstOrDefaultAsync();
    }

    public PlayerDocument? GetPlayer(string accountId, string characterName)
    {
        return GetPlayerAsync(accountId, characterName).GetAwaiter().GetResult();
    }

    public async Task SavePlayerAsync(PlayerDocument player)
    {
        var filter = Builders<PlayerDocument>.Filter.And(
            Builders<PlayerDocument>.Filter.Eq(p => p.AccountId, player.AccountId),
            Builders<PlayerDocument>.Filter.Eq(p => p.CharacterName, player.CharacterName)
        );
        await _players.ReplaceOneAsync(filter, player, new ReplaceOptions { IsUpsert = true });
    }

    public void SavePlayer(PlayerDocument player)
    {
        SavePlayerAsync(player).GetAwaiter().GetResult();
    }

    public async Task<PlayerDocument?> GetPlayerByNameAsync(string characterName)
    {
        var filter = Builders<PlayerDocument>.Filter.Eq(p => p.CharacterName, characterName);
        return await _players.Find(filter).FirstOrDefaultAsync();
    }

    public PlayerDocument? GetPlayerByName(string characterName)
    {
        return GetPlayerByNameAsync(characterName).GetAwaiter().GetResult();
    }

    #endregion

    #region Inventory Operations

    public PlayerInventory GetPlayerInventory(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newInventory = new PlayerInventory();
            SavePlayerInventory(accountId, character, newInventory);
            return newInventory;
        }
        return player.Inventory;
    }

    public void SavePlayerInventory(string accountId, string character, PlayerInventory inventory)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Inventory = inventory;
        SavePlayer(player);
    }

    #endregion

    #region Equipment Operations

    public PlayerEquips GetPlayerEquipment(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newEquips = new PlayerEquips();
            SavePlayerEquipment(accountId, character, newEquips);
            return newEquips;
        }
        return player.Equipment;
    }

    public void SavePlayerEquipment(string accountId, string character, PlayerEquips equipment)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Equipment = equipment;
        SavePlayer(player);
    }

    #endregion

    #region World Position Operations

    public WorldPosition GetWorldPosition(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newPos = new WorldPosition();
            SaveWorldPosition(accountId, character, newPos);
            return newPos;
        }
        return player.WorldPosition;
    }

    public void SaveWorldPosition(string accountId, string character, WorldPosition worldPosition)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.WorldPosition = worldPosition;
        SavePlayer(player);
    }

    #endregion

    #region Appearance Operations

    public APPEARANCE_PC_KR GetAppearance(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        return player?.Appearance ?? new APPEARANCE_PC_KR();
    }

    public void StoreAppearance(string accountId, string character, APPEARANCE_PC_KR data)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Appearance = data;
        SavePlayer(player);
    }

    #endregion

    #region Status Operations

    public SAVED_STATUS_PC_KR GetSavedStatusPC(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        return player?.Status ?? new SAVED_STATUS_PC_KR();
    }

    public void StoreSavedStatusPC(string accountId, string character, SAVED_STATUS_PC_KR data)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Status = data;
        SavePlayer(player);
    }

    #endregion

    #region Skills Operations

    public PlayerSkills GetPlayerSkills(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newSkills = new PlayerSkills();
            SavePlayerSkills(accountId, character, newSkills);
            return newSkills;
        }
        return player.Skills;
    }

    public void SavePlayerSkills(string accountId, string character, PlayerSkills skills)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Skills = skills;
        SavePlayer(player);
    }

    #endregion

    #region Quests Operations

    public PlayerQuests GetPlayerQuests(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newQuests = new PlayerQuests();
            SavePlayerQuests(accountId, character, newQuests);
            return newQuests;
        }
        return player.Quests;
    }

    public void SavePlayerQuests(string accountId, string character, PlayerQuests quests)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Quests = quests;
        SavePlayer(player);
    }

    #endregion

    #region Bank Operations

    public PlayerBank GetPlayerBank(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newBank = new PlayerBank();
            SavePlayerBank(accountId, character, newBank);
            return newBank;
        }
        return player.Bank;
    }

    public void SavePlayerBank(string accountId, string character, PlayerBank bank)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Bank = bank;
        SavePlayer(player);
    }

    #endregion

    #region Mail Operations

    public PlayerMail GetPlayerMail(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newMail = new PlayerMail();
            SavePlayerMail(accountId, character, newMail);
            return newMail;
        }
        return player.Mail;
    }

    public void SavePlayerMail(string accountId, string character, PlayerMail mail)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Mail = mail;
        SavePlayer(player);
    }

    public bool SendMail(MailMessage mail)
    {
        var recipient = GetPlayerByName(mail.RecipientName);
        if (recipient == null)
            return false;

        mail.MailId = DateTime.UtcNow.Ticks;
        recipient.Mail.Inbox.Add(mail);
        SavePlayer(recipient);
        return true;
    }

    #endregion

    #region Social Operations

    public PlayerSocial GetPlayerSocial(string accountId, string character)
    {
        var player = GetPlayer(accountId, character);
        if (player == null)
        {
            var newSocial = new PlayerSocial();
            SavePlayerSocial(accountId, character, newSocial);
            return newSocial;
        }
        return player.Social;
    }

    public void SavePlayerSocial(string accountId, string character, PlayerSocial social)
    {
        var player = GetPlayer(accountId, character) ?? new PlayerDocument
        {
            AccountId = accountId,
            CharacterName = character
        };
        player.Social = social;
        SavePlayer(player);
    }

    #endregion

    #region Guild Operations

    public async Task<List<GuildData>> LoadAllGuildsAsync()
    {
        var guilds = await _guilds.Find(_ => true).ToListAsync();
        return guilds.Select(g => g.Data).ToList();
    }

    public List<GuildData> LoadAllGuilds()
    {
        return LoadAllGuildsAsync().GetAwaiter().GetResult();
    }

    public async Task<GuildData?> GetGuildAsync(int guildId)
    {
        var filter = Builders<GuildDocument>.Filter.Eq(g => g.GuildId, guildId);
        var doc = await _guilds.Find(filter).FirstOrDefaultAsync();
        return doc?.Data;
    }

    public GuildData? GetGuild(int guildId)
    {
        return GetGuildAsync(guildId).GetAwaiter().GetResult();
    }

    public async Task<GuildData?> GetGuildByNameAsync(string guildName)
    {
        var filter = Builders<GuildDocument>.Filter.Regex(g => g.Name,
            new BsonRegularExpression($"^{guildName}$", "i"));
        var doc = await _guilds.Find(filter).FirstOrDefaultAsync();
        return doc?.Data;
    }

    public GuildData? GetGuildByName(string guildName)
    {
        return GetGuildByNameAsync(guildName).GetAwaiter().GetResult();
    }

    public async Task SaveGuildAsync(GuildData guild)
    {
        var doc = new GuildDocument
        {
            GuildId = guild.GuildId,
            Name = guild.Name,
            Data = guild
        };
        var filter = Builders<GuildDocument>.Filter.Eq(g => g.GuildId, guild.GuildId);
        await _guilds.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true });
    }

    public void SaveGuild(GuildData guild)
    {
        SaveGuildAsync(guild).GetAwaiter().GetResult();
    }

    public async Task<int> GetNextGuildIdAsync()
    {
        var sort = Builders<GuildDocument>.Sort.Descending(g => g.GuildId);
        var doc = await _guilds.Find(_ => true).Sort(sort).FirstOrDefaultAsync();
        return (doc?.GuildId ?? 0) + 1;
    }

    public int GetNextGuildId()
    {
        return GetNextGuildIdAsync().GetAwaiter().GetResult();
    }

    public async Task<bool> DeleteGuildAsync(int guildId)
    {
        var filter = Builders<GuildDocument>.Filter.Eq(g => g.GuildId, guildId);
        var result = await _guilds.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public bool DeleteGuild(int guildId)
    {
        return DeleteGuildAsync(guildId).GetAwaiter().GetResult();
    }

    public GuildData? GetCharacterGuild(string characterName)
    {
        var guilds = LoadAllGuilds();
        return guilds.FirstOrDefault(g =>
            g.Members.Any(m => m.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase)));
    }

    #endregion

    #region Account Operations

    /// <summary>
    /// Gets an account by account ID.
    /// </summary>
    /// <param name="accountId">Account ID to search for.</param>
    /// <returns>AccountDocument or null if not found.</returns>
    public async Task<AccountDocument?> GetAccountAsync(string accountId)
    {
        var filter = Builders<AccountDocument>.Filter.Eq(a => a.AccountId, accountId);
        return await _accounts.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets an account by account ID (synchronous).
    /// </summary>
    public AccountDocument? GetAccount(string accountId)
    {
        return GetAccountAsync(accountId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets an account by email address.
    /// </summary>
    /// <param name="email">Email address to search for.</param>
    /// <returns>AccountDocument or null if not found.</returns>
    public async Task<AccountDocument?> GetAccountByEmailAsync(string email)
    {
        var filter = Builders<AccountDocument>.Filter.Regex(a => a.Email,
            new BsonRegularExpression($"^{email}$", "i"));
        return await _accounts.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets an account by email address (synchronous).
    /// </summary>
    public AccountDocument? GetAccountByEmail(string email)
    {
        return GetAccountByEmailAsync(email).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Saves an account to the database.
    /// </summary>
    /// <param name="account">Account document to save.</param>
    public async Task SaveAccountAsync(AccountDocument account)
    {
        var filter = Builders<AccountDocument>.Filter.Eq(a => a.AccountId, account.AccountId);
        await _accounts.ReplaceOneAsync(filter, account, new ReplaceOptions { IsUpsert = true });
    }

    /// <summary>
    /// Saves an account to the database (synchronous).
    /// </summary>
    public void SaveAccount(AccountDocument account)
    {
        SaveAccountAsync(account).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Deletes an account from the database.
    /// </summary>
    /// <param name="accountId">Account ID to delete.</param>
    /// <returns>True if deleted, false otherwise.</returns>
    public async Task<bool> DeleteAccountAsync(string accountId)
    {
        var filter = Builders<AccountDocument>.Filter.Eq(a => a.AccountId, accountId);
        var result = await _accounts.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Deletes an account from the database (synchronous).
    /// </summary>
    public bool DeleteAccount(string accountId)
    {
        return DeleteAccountAsync(accountId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Checks if an account exists.
    /// </summary>
    /// <param name="accountId">Account ID to check.</param>
    /// <returns>True if account exists.</returns>
    public bool AccountExists(string accountId)
    {
        var filter = Builders<AccountDocument>.Filter.Eq(a => a.AccountId, accountId);
        return _accounts.Find(filter).Any();
    }

    /// <summary>
    /// Checks if an email is already registered.
    /// </summary>
    /// <param name="email">Email to check.</param>
    /// <returns>True if email exists.</returns>
    public bool EmailExists(string email)
    {
        var filter = Builders<AccountDocument>.Filter.Regex(a => a.Email,
            new BsonRegularExpression($"^{email}$", "i"));
        return _accounts.Find(filter).Any();
    }

    #endregion

    #region Ban Operations

    /// <summary>
    /// Gets all active bans.
    /// </summary>
    /// <returns>List of active ban records.</returns>
    public async Task<List<BanRecord>> GetActiveBansAsync()
    {
        var filter = Builders<BanDocument>.Filter.Eq(b => b.IsActive, true);
        var docs = await _bans.Find(filter).ToListAsync();
        return docs.Select(d => d.ToBanRecord()).ToList();
    }

    /// <summary>
    /// Gets all active bans (synchronous).
    /// </summary>
    public List<BanRecord> GetActiveBans()
    {
        return GetActiveBansAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets ban history for an account.
    /// </summary>
    /// <param name="accountId">Account ID to get history for.</param>
    /// <returns>List of all bans for the account.</returns>
    public async Task<List<BanRecord>> GetBanHistoryAsync(string accountId)
    {
        var filter = Builders<BanDocument>.Filter.Eq(b => b.AccountId, accountId);
        var docs = await _bans.Find(filter).ToListAsync();
        return docs.Select(d => d.ToBanRecord()).ToList();
    }

    /// <summary>
    /// Gets ban history for an account (synchronous).
    /// </summary>
    public List<BanRecord> GetBanHistory(string accountId)
    {
        return GetBanHistoryAsync(accountId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Saves a ban record to the database.
    /// </summary>
    /// <param name="ban">Ban record to save.</param>
    public async Task SaveBanAsync(BanRecord ban)
    {
        var doc = BanDocument.FromBanRecord(ban);
        var filter = Builders<BanDocument>.Filter.And(
            Builders<BanDocument>.Filter.Eq(b => b.AccountId, ban.AccountId),
            Builders<BanDocument>.Filter.Eq(b => b.BannedAt, ban.BannedAt)
        );
        await _bans.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true });
    }

    /// <summary>
    /// Saves a ban record to the database (synchronous).
    /// </summary>
    public void SaveBan(BanRecord ban)
    {
        SaveBanAsync(ban).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Removes active ban for an account.
    /// </summary>
    /// <param name="accountId">Account ID to unban.</param>
    /// <returns>True if ban was removed.</returns>
    public async Task<bool> RemoveBanAsync(string accountId)
    {
        var filter = Builders<BanDocument>.Filter.And(
            Builders<BanDocument>.Filter.Eq(b => b.AccountId, accountId),
            Builders<BanDocument>.Filter.Eq(b => b.IsActive, true)
        );
        var update = Builders<BanDocument>.Update.Set(b => b.IsActive, false);
        var result = await _bans.UpdateManyAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Removes active ban for an account (synchronous).
    /// </summary>
    public bool RemoveBan(string accountId)
    {
        return RemoveBanAsync(accountId).GetAwaiter().GetResult();
    }

    #endregion
}

#region MongoDB Document Models

public class PlayerDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    public string? Id { get; set; }

    [BsonElement("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [BsonElement("characterName")]
    public string CharacterName { get; set; } = string.Empty;

    [BsonElement("appearance")]
    public APPEARANCE_PC_KR Appearance { get; set; } = new();

    [BsonElement("status")]
    public SAVED_STATUS_PC_KR Status { get; set; } = new();

    [BsonElement("worldPosition")]
    public WorldPosition WorldPosition { get; set; } = new();

    [BsonElement("inventory")]
    public PlayerInventory Inventory { get; set; } = new();

    [BsonElement("equipment")]
    public PlayerEquips Equipment { get; set; } = new();

    [BsonElement("skills")]
    public PlayerSkills Skills { get; set; } = new();

    [BsonElement("quests")]
    public PlayerQuests Quests { get; set; } = new();

    [BsonElement("bank")]
    public PlayerBank Bank { get; set; } = new();

    [BsonElement("mail")]
    public PlayerMail Mail { get; set; } = new();

    [BsonElement("social")]
    public PlayerSocial Social { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("lastLogin")]
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
}

public class GuildDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("guildId")]
    public int GuildId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("data")]
    public GuildData Data { get; set; } = new();
}

/// <summary>
/// MongoDB document for ban records.
/// </summary>
public class BanDocument
{
    /// <summary>MongoDB document ID.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>Account ID of the banned player.</summary>
    [BsonElement("accountId")]
    public string AccountId { get; set; } = string.Empty;

    /// <summary>Character name if ban is character-specific.</summary>
    [BsonElement("characterName")]
    public string? CharacterName { get; set; }

    /// <summary>Reason for the ban.</summary>
    [BsonElement("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>Type of cheat that triggered the ban.</summary>
    [BsonElement("cheatType")]
    public string CheatType { get; set; } = string.Empty;

    /// <summary>When the ban was issued.</summary>
    [BsonElement("bannedAt")]
    public DateTime BannedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When the ban expires (null for permanent).</summary>
    [BsonElement("expiresAt")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>IP address at time of ban.</summary>
    [BsonElement("ipAddress")]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>Who issued the ban.</summary>
    [BsonElement("issuedBy")]
    public string IssuedBy { get; set; } = "System";

    /// <summary>Additional details about the violation.</summary>
    [BsonElement("details")]
    public string Details { get; set; } = string.Empty;

    /// <summary>Whether the ban is currently active.</summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Converts this document to a BanRecord.
    /// </summary>
    /// <returns>BanRecord instance.</returns>
    public BanRecord ToBanRecord()
    {
        return new BanRecord
        {
            AccountId = AccountId,
            CharacterName = CharacterName,
            Reason = Reason,
            CheatType = CheatType,
            BannedAt = BannedAt,
            ExpiresAt = ExpiresAt,
            IpAddress = IpAddress,
            IssuedBy = IssuedBy,
            Details = Details
        };
    }

    /// <summary>
    /// Creates a BanDocument from a BanRecord.
    /// </summary>
    /// <param name="ban">Ban record to convert.</param>
    /// <returns>BanDocument instance.</returns>
    public static BanDocument FromBanRecord(BanRecord ban)
    {
        return new BanDocument
        {
            AccountId = ban.AccountId,
            CharacterName = ban.CharacterName,
            Reason = ban.Reason,
            CheatType = ban.CheatType,
            BannedAt = ban.BannedAt,
            ExpiresAt = ban.ExpiresAt,
            IpAddress = ban.IpAddress,
            IssuedBy = ban.IssuedBy,
            Details = ban.Details,
            IsActive = ban.IsActive
        };
    }
}

#endregion
