using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;

namespace kakia_lime_odyssey_server.Services.Notification;

/// <summary>
/// Error codes for system notifications.
/// </summary>
public enum SystemErrorCode
{
	/// <summary>No error.</summary>
	None = 0,

	/// <summary>Skill is not found.</summary>
	SkillNotFound = 1,

	/// <summary>Skill is on cooldown.</summary>
	SkillOnCooldown = 2,

	/// <summary>Target is invalid or not found.</summary>
	TargetInvalid = 3,

	/// <summary>Target is dead.</summary>
	TargetDead = 4,

	/// <summary>Target is out of range.</summary>
	TargetOutOfRange = 5,

	/// <summary>Character not found.</summary>
	CharacterNotFound = 6,

	/// <summary>Invalid slot or inventory position.</summary>
	InvalidSlot = 7,

	/// <summary>Item not found.</summary>
	ItemNotFound = 8,

	/// <summary>Already loaded or initialized.</summary>
	AlreadyLoaded = 9,

	/// <summary>Job already selected.</summary>
	JobAlreadySelected = 10,

	/// <summary>Insufficient resources (MP, HP, etc).</summary>
	InsufficientResources = 11,

	/// <summary>Attacker is dead.</summary>
	AttackerDead = 12,

	/// <summary>Player not found.</summary>
	PlayerNotFound = 13,

	/// <summary>Cannot resurrect - not dead.</summary>
	NotDead = 14,

	/// <summary>Chatroom operation failed.</summary>
	ChatroomError = 15,

	/// <summary>Crafting operation failed.</summary>
	CraftingError = 16,

	/// <summary>Movement validation failed.</summary>
	MovementRejected = 17,

	/// <summary>Internal server error.</summary>
	InternalError = 99
}

/// <summary>
/// Service for sending system notifications and error messages to players.
/// Uses SC_WHISPER packet with [System] sender for error messages.
/// </summary>
/// <remarks>
/// This service addresses silent handler failures by providing consistent
/// error notification to players when operations fail.
/// </remarks>
public static class SystemNotificationService
{
	/// <summary>Default sender name for system messages.</summary>
	private const string SystemSender = "[System]";

	/// <summary>Default sender name for error messages.</summary>
	private const string ErrorSender = "[Error]";

	/// <summary>
	/// Sends a system message to the player.
	/// </summary>
	/// <param name="client">The player client to send the message to.</param>
	/// <param name="message">The message to send.</param>
	public static void SendSystemMessage(IPlayerClient client, string message)
	{
		SendWhisper(client, SystemSender, message);
	}

	/// <summary>
	/// Sends an error notification to the player.
	/// </summary>
	/// <param name="client">The player client to send the error to.</param>
	/// <param name="errorCode">The error code.</param>
	/// <param name="additionalInfo">Additional information about the error (optional).</param>
	public static void SendError(IPlayerClient client, SystemErrorCode errorCode, string? additionalInfo = null)
	{
		string message = GetErrorMessage(errorCode);
		if (!string.IsNullOrEmpty(additionalInfo))
			message = $"{message}: {additionalInfo}";

		SendWhisper(client, ErrorSender, message);
		Logger.Log($"[NOTIFY] Sent error to client: {errorCode} - {message}", LogLevel.Debug);
	}

	/// <summary>
	/// Sends a skill failure notification.
	/// </summary>
	/// <param name="client">The player client.</param>
	/// <param name="errorCode">The specific error code.</param>
	/// <param name="skillId">The skill ID that failed (optional).</param>
	public static void SendSkillError(IPlayerClient client, SystemErrorCode errorCode, uint? skillId = null)
	{
		string message = GetErrorMessage(errorCode);
		if (skillId.HasValue)
			message = $"Skill {skillId}: {message}";

		SendWhisper(client, ErrorSender, message);
	}

	/// <summary>
	/// Sends a combat failure notification.
	/// </summary>
	/// <param name="client">The player client.</param>
	/// <param name="errorCode">The specific error code.</param>
	public static void SendCombatError(IPlayerClient client, SystemErrorCode errorCode)
	{
		SendError(client, errorCode);
	}

	/// <summary>
	/// Sends a notification that an operation succeeded (for confirmation).
	/// </summary>
	/// <param name="client">The player client.</param>
	/// <param name="message">The success message.</param>
	public static void SendSuccess(IPlayerClient client, string message)
	{
		SendWhisper(client, SystemSender, message);
	}

	/// <summary>
	/// Gets the user-friendly error message for an error code.
	/// </summary>
	private static string GetErrorMessage(SystemErrorCode errorCode)
	{
		return errorCode switch
		{
			SystemErrorCode.None => "No error",
			SystemErrorCode.SkillNotFound => "Skill not available",
			SystemErrorCode.SkillOnCooldown => "Skill is on cooldown",
			SystemErrorCode.TargetInvalid => "Target is invalid",
			SystemErrorCode.TargetDead => "Target is already dead",
			SystemErrorCode.TargetOutOfRange => "Target is out of range",
			SystemErrorCode.CharacterNotFound => "Character not found",
			SystemErrorCode.InvalidSlot => "Invalid inventory slot",
			SystemErrorCode.ItemNotFound => "Item not found",
			SystemErrorCode.AlreadyLoaded => "Already initialized",
			SystemErrorCode.JobAlreadySelected => "Job already selected",
			SystemErrorCode.InsufficientResources => "Insufficient resources",
			SystemErrorCode.AttackerDead => "Cannot attack while dead",
			SystemErrorCode.PlayerNotFound => "Player not found",
			SystemErrorCode.NotDead => "Cannot resurrect - you are not dead",
			SystemErrorCode.ChatroomError => "Chatroom operation failed",
			SystemErrorCode.CraftingError => "Crafting operation failed",
			SystemErrorCode.MovementRejected => "Movement rejected",
			SystemErrorCode.InternalError => "Internal server error",
			_ => "Unknown error"
		};
	}

	/// <summary>
	/// Sends a whisper packet to the player.
	/// </summary>
	private static void SendWhisper(IPlayerClient client, string sender, string message)
	{
		SC_WHISPER whisper = new()
		{
			fromName = sender,
			message = message
		};

		using PacketWriter pw = new();
		pw.Write(whisper);
		client.Send(pw.ToSizedPacket(), default).Wait();
	}
}
