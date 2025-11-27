namespace kakia_lime_odyssey_server.Models.Persistence;

/// <summary>
/// Persisted mail/post data for a character
/// </summary>
public class PlayerMail
{
	/// <summary>Inbox messages</summary>
	public List<MailMessage> Inbox { get; set; } = new();

	/// <summary>Sent messages (for tracking)</summary>
	public List<MailMessage> Sent { get; set; } = new();
}

/// <summary>
/// A single mail message
/// </summary>
public class MailMessage
{
	/// <summary>Unique mail ID</summary>
	public long MailId { get; set; }

	/// <summary>Sender character name</summary>
	public string SenderName { get; set; } = string.Empty;

	/// <summary>Recipient character name</summary>
	public string RecipientName { get; set; } = string.Empty;

	/// <summary>Mail subject (max 50 chars)</summary>
	public string Subject { get; set; } = string.Empty;

	/// <summary>Mail body text</summary>
	public string Body { get; set; } = string.Empty;

	/// <summary>Attached items (up to 11)</summary>
	public List<Item> Attachments { get; set; } = new();

	/// <summary>Attached Peder currency</summary>
	public long AttachedPeder { get; set; }

	/// <summary>Attached Lant currency</summary>
	public long AttachedLant { get; set; }

	/// <summary>When the mail was sent</summary>
	public DateTime SentAt { get; set; } = DateTime.UtcNow;

	/// <summary>When the mail expires (30 days default)</summary>
	public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);

	/// <summary>Whether the mail has been read</summary>
	public bool IsRead { get; set; }

	/// <summary>Whether attachments have been claimed</summary>
	public bool AttachmentsClaimed { get; set; }

	/// <summary>Mail type (normal, system, COD, etc.)</summary>
	public MailType Type { get; set; } = MailType.Normal;
}

public enum MailType
{
	Normal = 0,
	System = 1,
	CashOnDelivery = 2,
	GuildNotice = 3,
	AuctionResult = 4
}
