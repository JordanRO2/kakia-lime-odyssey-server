/// <summary>
/// Service for managing player accounts and authentication.
/// </summary>
/// <remarks>
/// Handles account creation, authentication, password hashing, and account management.
/// Uses: IDatabase for persistence, SHA256 for password hashing.
/// </remarks>
using System.Security.Cryptography;
using System.Text;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Models.Persistence;

namespace kakia_lime_odyssey_server.Services.Account;

/// <summary>
/// Service for account management and authentication.
/// </summary>
public static class AccountService
{
    private static readonly IDatabase _db = DatabaseFactory.Instance;

    /// <summary>
    /// Authentication result codes.
    /// </summary>
    public enum AuthResult
    {
        /// <summary>Authentication successful.</summary>
        Success = 0,
        /// <summary>Account not found.</summary>
        AccountNotFound = 1,
        /// <summary>Invalid password.</summary>
        InvalidPassword = 2,
        /// <summary>Account is banned.</summary>
        AccountBanned = 3,
        /// <summary>Account is suspended.</summary>
        AccountSuspended = 4,
        /// <summary>Account is locked due to failed attempts.</summary>
        AccountLocked = 5,
        /// <summary>Account pending verification.</summary>
        PendingVerification = 6,
        /// <summary>Account already logged in.</summary>
        AlreadyLoggedIn = 7
    }

    /// <summary>
    /// Authenticates an account with the given credentials.
    /// </summary>
    /// <param name="accountId">Account ID to authenticate.</param>
    /// <param name="password">Plain text password.</param>
    /// <param name="ipAddress">Client IP address for logging.</param>
    /// <returns>Authentication result code.</returns>
    public static AuthResult Authenticate(string accountId, string password, string ipAddress = "")
    {
        var account = _db.GetAccount(accountId);

        if (account == null)
        {
            Logger.Log($"[Auth] Login attempt for non-existent account: {accountId}", LogLevel.Warning);
            return AuthResult.AccountNotFound;
        }

        // Check if account is locked
        if (account.LockoutEndTime.HasValue && account.LockoutEndTime > DateTime.UtcNow)
        {
            Logger.Log($"[Auth] Login attempt for locked account: {accountId}", LogLevel.Warning);
            return AuthResult.AccountLocked;
        }

        // Check account status
        switch (account.Status)
        {
            case AccountStatus.Banned:
                Logger.Log($"[Auth] Login attempt for banned account: {accountId}", LogLevel.Warning);
                return AuthResult.AccountBanned;
            case AccountStatus.Suspended:
                Logger.Log($"[Auth] Login attempt for suspended account: {accountId}", LogLevel.Warning);
                return AuthResult.AccountSuspended;
            case AccountStatus.PendingVerification:
                return AuthResult.PendingVerification;
            case AccountStatus.Locked:
                return AuthResult.AccountLocked;
        }

        // Check if already logged in
        if (account.IsOnline)
        {
            Logger.Log($"[Auth] Login attempt for already online account: {accountId}", LogLevel.Warning);
            return AuthResult.AlreadyLoggedIn;
        }

        // Verify password
        var passwordHash = HashPassword(password, account.PasswordSalt);
        if (passwordHash != account.PasswordHash)
        {
            account.FailedLoginAttempts++;

            // Lock account after 5 failed attempts
            if (account.FailedLoginAttempts >= 5)
            {
                account.LockoutEndTime = DateTime.UtcNow.AddMinutes(15);
                account.Status = AccountStatus.Locked;
                Logger.Log($"[Auth] Account locked due to failed attempts: {accountId}", LogLevel.Warning);
            }

            _db.SaveAccount(account);
            Logger.Log($"[Auth] Invalid password for account: {accountId} (attempt {account.FailedLoginAttempts})", LogLevel.Warning);
            return AuthResult.InvalidPassword;
        }

        // Success - update account
        account.FailedLoginAttempts = 0;
        account.LockoutEndTime = null;
        account.LastLogin = DateTime.UtcNow;
        account.LastLoginIp = ipAddress;
        account.LoginCount++;
        account.IsOnline = true;
        account.SessionToken = GenerateSessionToken();

        if (account.Status == AccountStatus.Locked)
            account.Status = AccountStatus.Active;

        _db.SaveAccount(account);

        Logger.Log($"[Auth] Account authenticated: {accountId}", LogLevel.Information);
        return AuthResult.Success;
    }

    /// <summary>
    /// Creates a new account.
    /// </summary>
    /// <param name="accountId">Account ID (login name).</param>
    /// <param name="password">Plain text password.</param>
    /// <param name="email">Email address (optional).</param>
    /// <returns>True if account was created successfully.</returns>
    public static bool CreateAccount(string accountId, string password, string email = "")
    {
        if (_db.AccountExists(accountId))
        {
            Logger.Log($"[Account] Account creation failed - already exists: {accountId}", LogLevel.Warning);
            return false;
        }

        if (!string.IsNullOrEmpty(email) && _db.EmailExists(email))
        {
            Logger.Log($"[Account] Account creation failed - email exists: {email}", LogLevel.Warning);
            return false;
        }

        var salt = GenerateSalt();
        var passwordHash = HashPassword(password, salt);

        var account = new AccountDocument
        {
            AccountId = accountId,
            PasswordHash = passwordHash,
            PasswordSalt = salt,
            Email = email,
            Status = AccountStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _db.SaveAccount(account);
        Logger.Log($"[Account] Account created: {accountId}", LogLevel.Information);
        return true;
    }

    /// <summary>
    /// Gets an account by ID.
    /// </summary>
    /// <param name="accountId">Account ID to retrieve.</param>
    /// <returns>AccountDocument or null if not found.</returns>
    public static AccountDocument? GetAccount(string accountId)
    {
        return _db.GetAccount(accountId);
    }

    /// <summary>
    /// Marks an account as logged out.
    /// </summary>
    /// <param name="accountId">Account ID to log out.</param>
    public static void Logout(string accountId)
    {
        var account = _db.GetAccount(accountId);
        if (account == null) return;

        account.IsOnline = false;
        account.SessionToken = null;
        _db.SaveAccount(account);

        Logger.Log($"[Auth] Account logged out: {accountId}", LogLevel.Information);
    }

    /// <summary>
    /// Changes an account's password.
    /// </summary>
    /// <param name="accountId">Account ID.</param>
    /// <param name="oldPassword">Current password.</param>
    /// <param name="newPassword">New password.</param>
    /// <returns>True if password was changed.</returns>
    public static bool ChangePassword(string accountId, string oldPassword, string newPassword)
    {
        var account = _db.GetAccount(accountId);
        if (account == null) return false;

        var oldHash = HashPassword(oldPassword, account.PasswordSalt);
        if (oldHash != account.PasswordHash) return false;

        var newSalt = GenerateSalt();
        account.PasswordSalt = newSalt;
        account.PasswordHash = HashPassword(newPassword, newSalt);
        _db.SaveAccount(account);

        Logger.Log($"[Account] Password changed for: {accountId}", LogLevel.Information);
        return true;
    }

    /// <summary>
    /// Sets an account's permission level.
    /// </summary>
    /// <param name="accountId">Account ID.</param>
    /// <param name="level">Permission level (0=normal, 1=GM, 2=Admin).</param>
    /// <returns>True if permission was set.</returns>
    public static bool SetPermissionLevel(string accountId, int level)
    {
        var account = _db.GetAccount(accountId);
        if (account == null) return false;

        account.PermissionLevel = level;
        _db.SaveAccount(account);

        Logger.Log($"[Account] Permission level set to {level} for: {accountId}", LogLevel.Information);
        return true;
    }

    /// <summary>
    /// Checks if an account exists.
    /// </summary>
    /// <param name="accountId">Account ID to check.</param>
    /// <returns>True if account exists.</returns>
    public static bool AccountExists(string accountId)
    {
        return _db.AccountExists(accountId);
    }

    /// <summary>
    /// Gets or creates an account (auto-registration mode).
    /// </summary>
    /// <param name="accountId">Account ID.</param>
    /// <param name="password">Password.</param>
    /// <returns>True if account exists or was created.</returns>
    public static bool GetOrCreateAccount(string accountId, string password)
    {
        if (_db.AccountExists(accountId))
            return true;

        return CreateAccount(accountId, password);
    }

    /// <summary>
    /// Generates a random salt for password hashing.
    /// </summary>
    /// <returns>Base64 encoded salt string.</returns>
    private static string GenerateSalt()
    {
        var salt = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }

    /// <summary>
    /// Hashes a password with the given salt.
    /// </summary>
    /// <param name="password">Plain text password.</param>
    /// <param name="salt">Base64 encoded salt.</param>
    /// <returns>Base64 encoded password hash.</returns>
    private static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var combined = new byte[saltBytes.Length + passwordBytes.Length];

        Buffer.BlockCopy(saltBytes, 0, combined, 0, saltBytes.Length);
        Buffer.BlockCopy(passwordBytes, 0, combined, saltBytes.Length, passwordBytes.Length);

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Generates a random session token.
    /// </summary>
    /// <returns>Session token string.</returns>
    private static string GenerateSessionToken()
    {
        var token = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(token);
        return Convert.ToBase64String(token);
    }
}
