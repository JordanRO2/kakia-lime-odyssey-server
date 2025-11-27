/// <summary>
/// Login result codes returned by server.
/// </summary>
/// <remarks>
/// IDA Verified: Partial (enum exists as PACKET_SC_LOGIN_RESULT::RESULT)
/// Used by: SC_LOGIN_RESULT
/// </remarks>
namespace kakia_lime_odyssey_packets.Packets.Enums;

public enum LOGIN_RESULT : int
{
	/// <summary>Login successful</summary>
	SUCCESS = 0,

	/// <summary>Invalid account ID or password</summary>
	INVALID_CREDENTIALS = 1,

	/// <summary>Account is banned</summary>
	ACCOUNT_BANNED = 2,

	/// <summary>Account is already logged in</summary>
	ALREADY_LOGGED_IN = 3,

	/// <summary>Server is full</summary>
	SERVER_FULL = 4,

	/// <summary>Client version mismatch</summary>
	VERSION_MISMATCH = 5,

	/// <summary>Server maintenance</summary>
	MAINTENANCE = 6,

	/// <summary>Unknown error</summary>
	UNKNOWN_ERROR = 99
}
