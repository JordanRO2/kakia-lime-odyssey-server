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

    private static MongoDBService? _instance;
    public static MongoDBService Instance => _instance ??= new MongoDBService();

    public MongoDBService(string connectionString = "mongodb://172.22.0.1:27017", string databaseName = "lime_odyssey")
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);

        _players = _database.GetCollection<PlayerDocument>("players");
        _guilds = _database.GetCollection<GuildDocument>("guilds");

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
}

#region MongoDB Document Models

public class PlayerDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
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

#endregion
