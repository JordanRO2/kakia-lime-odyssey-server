/// <summary>
/// Persisted account data for authentication and account management.
/// </summary>
/// <remarks>
/// Database collection: accounts
/// Stores credentials, account status, and login history.
/// </remarks>
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Account document for MongoDB persistence.
/// </summary>
public class AccountDocument
{
    /// <summary>MongoDB document ID.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>Account ID (login name).</summary>
    [BsonElement("accountId")]
    public string AccountId { get; set; } = string.Empty;

    /// <summary>Hashed password.</summary>
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Salt used for password hashing.</summary>
    [BsonElement("passwordSalt")]
    public string PasswordSalt { get; set; } = string.Empty;

    /// <summary>Email address for account recovery.</summary>
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Account status.</summary>
    [BsonElement("status")]
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    /// <summary>Account creation timestamp.</summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Last successful login timestamp.</summary>
    [BsonElement("lastLogin")]
    public DateTime? LastLogin { get; set; }

    /// <summary>Last login IP address.</summary>
    [BsonElement("lastLoginIp")]
    public string LastLoginIp { get; set; } = string.Empty;

    /// <summary>Total login count.</summary>
    [BsonElement("loginCount")]
    public int LoginCount { get; set; }

    /// <summary>Failed login attempts since last success.</summary>
    [BsonElement("failedLoginAttempts")]
    public int FailedLoginAttempts { get; set; }

    /// <summary>Account lockout end time (if locked).</summary>
    [BsonElement("lockoutEndTime")]
    public DateTime? LockoutEndTime { get; set; }

    /// <summary>Maximum characters allowed on this account.</summary>
    [BsonElement("maxCharacters")]
    public int MaxCharacters { get; set; } = 5;

    /// <summary>Account permission level (0=normal, 1=GM, 2=Admin).</summary>
    [BsonElement("permissionLevel")]
    public int PermissionLevel { get; set; }

    /// <summary>Whether the account is currently logged in.</summary>
    [BsonElement("isOnline")]
    public bool IsOnline { get; set; }

    /// <summary>Session token for current login session.</summary>
    [BsonElement("sessionToken")]
    public string? SessionToken { get; set; }
}

/// <summary>
/// Account status values.
/// </summary>
public enum AccountStatus
{
    /// <summary>Account is active and can login.</summary>
    Active = 0,

    /// <summary>Account is temporarily suspended.</summary>
    Suspended = 1,

    /// <summary>Account is permanently banned.</summary>
    Banned = 2,

    /// <summary>Account pending email verification.</summary>
    PendingVerification = 3,

    /// <summary>Account is locked due to failed login attempts.</summary>
    Locked = 4
}
