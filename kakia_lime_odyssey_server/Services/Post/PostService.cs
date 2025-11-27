/// <summary>
/// Service for managing player mail/post with MongoDB persistence.
/// </summary>
/// <remarks>
/// Uses: MongoDBService for mail persistence
/// Handles: Send, receive, delete mail, take attachments
/// </remarks>
using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Models.Persistence;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Post;

/// <summary>
/// Service for managing player mail/post with MongoDB persistence.
/// </summary>
public class PostService
{
	/// <summary>In-memory cache for mail (synced with DB)</summary>
	private readonly ConcurrentDictionary<string, PlayerMail> _mailCache = new();
	private const int MaxMailboxSize = 100;
	private const int MaxAttachments = 11;

	/// <summary>
	/// Sends mail from one player to another.
	/// </summary>
	public bool SendPost(PlayerClient sender, string toName, string title, string body, List<PostAttachment> attachments)
	{
		var senderChar = sender.GetCurrentCharacter();
		if (senderChar == null)
			return false;

		if (string.IsNullOrWhiteSpace(toName) || string.IsNullOrWhiteSpace(title))
			return false;

		if (attachments.Count > MaxAttachments)
			return false;

		// Find recipient (online or offline)
		var recipientPlayer = MongoDBService.Instance.GetPlayerByName(toName);
		if (recipientPlayer == null)
		{
			SendPostResult(sender, false);
			Logger.Log($"[POST] {senderChar.appearance.name} failed to send mail to '{toName}': recipient not found", LogLevel.Debug);
			return false;
		}

		// Convert attachments to persistence format
		var mailAttachments = attachments.Select(a => new Models.Item
		{
			TypeID = a.TypeID,
			Slot = a.Slot
		}).ToList();

		// Create mail message
		var mail = new MailMessage
		{
			MailId = DateTime.UtcNow.Ticks,
			SenderName = senderChar.appearance.name,
			RecipientName = toName,
			Subject = title,
			Body = body,
			Attachments = mailAttachments,
			SentAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddDays(30),
			IsRead = false,
			AttachmentsClaimed = false,
			Type = MailType.Normal
		};

		// Save to database
		bool success = MongoDBService.Instance.SendMail(mail);
		if (!success)
		{
			SendPostResult(sender, false);
			Logger.Log($"[POST] {senderChar.appearance.name} failed to send mail to '{toName}': database error", LogLevel.Warning);
			return false;
		}

		// Clear cache for recipient so they get fresh data
		string cacheKey = $"{recipientPlayer.AccountId}:{toName}";
		_mailCache.TryRemove(cacheKey, out _);

		SendPostResult(sender, true);

		// Notify recipient if online
		var recipientOnline = LimeServer.PlayerClients.FirstOrDefault(p =>
		{
			var c = p.GetCurrentCharacter();
			return c != null && c.appearance.name.Equals(toName, StringComparison.OrdinalIgnoreCase);
		});

		if (recipientOnline != null && recipientOnline.IsConnected())
		{
			var recipientMail = GetOrLoadMail(recipientPlayer.AccountId, toName);
			int unreadCount = recipientMail.Inbox.Count(m => !m.IsRead);
			SendNewPostAlarm(recipientOnline, unreadCount);
		}

		Logger.Log($"[POST] {senderChar.appearance.name} sent mail to '{toName}': {title}", LogLevel.Debug);
		return true;
	}

	/// <summary>
	/// Gets a specific mail by index.
	/// </summary>
	public MailMessage? GetPost(PlayerClient player, long mailId)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return null;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var mail = GetOrLoadMail(accountId, charName);

		return mail.Inbox.FirstOrDefault(m => m.MailId == mailId);
	}

	/// <summary>
	/// Gets all mail for a player.
	/// </summary>
	public List<MailMessage> GetMailbox(PlayerClient player)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return new List<MailMessage>();

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var mail = GetOrLoadMail(accountId, charName);

		return mail.Inbox;
	}

	/// <summary>
	/// Deletes a mail message.
	/// </summary>
	public bool DeletePost(PlayerClient player, int indexNumber)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var mail = GetOrLoadMail(accountId, charName);

		// Find by index (1-based from client)
		if (indexNumber < 1 || indexNumber > mail.Inbox.Count)
			return false;

		var post = mail.Inbox[indexNumber - 1];

		// Can't delete if has unclaimed attachments
		if (post.Attachments.Count > 0 && !post.AttachmentsClaimed)
			return false;

		mail.Inbox.RemoveAt(indexNumber - 1);
		SaveMail(accountId, charName, mail);

		return true;
	}

	/// <summary>
	/// Takes attachments from a mail message.
	/// </summary>
	public bool TakePostItems(PlayerClient player, int indexNumber)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return false;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var mail = GetOrLoadMail(accountId, charName);

		// Find by index (1-based from client)
		if (indexNumber < 1 || indexNumber > mail.Inbox.Count)
			return false;

		var post = mail.Inbox[indexNumber - 1];

		if (post.AttachmentsClaimed || post.Attachments.Count == 0)
			return false;

		post.AttachmentsClaimed = true;
		SaveMail(accountId, charName, mail);

		return true;
	}

	/// <summary>
	/// Gets unread mail count for a player.
	/// </summary>
	public int GetUnreadCount(PlayerClient player)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return 0;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var mail = GetOrLoadMail(accountId, charName);

		return mail.Inbox.Count(m => !m.IsRead);
	}

	/// <summary>
	/// Marks a mail as read.
	/// </summary>
	public void MarkAsRead(PlayerClient player, int indexNumber)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var mail = GetOrLoadMail(accountId, charName);

		// Find by index (1-based from client)
		if (indexNumber < 1 || indexNumber > mail.Inbox.Count)
			return;

		var post = mail.Inbox[indexNumber - 1];
		if (!post.IsRead)
		{
			post.IsRead = true;
			SaveMail(accountId, charName, mail);
		}
	}

	/// <summary>
	/// Sends mail list to player.
	/// </summary>
	public void SendPostList(PlayerClient player)
	{
		var mailbox = GetMailbox(player);

		using PacketWriter pw = new();
		pw.Write(new SC_POST_LIST
		{
			count = (byte)Math.Min(mailbox.Count, 255)
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Sends mail details to player.
	/// </summary>
	public void SendPostDetails(PlayerClient player, int indexNumber)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		var mail = GetOrLoadMail(accountId, charName);

		// Find by index (1-based from client)
		if (indexNumber < 1 || indexNumber > mail.Inbox.Count)
			return;

		var post = mail.Inbox[indexNumber - 1];

		// Mark as read
		if (!post.IsRead)
		{
			post.IsRead = true;
			SaveMail(accountId, charName, mail);
		}

		var packet = new PACKET_SC_POST
		{
			indexNumber = indexNumber,
			fromName = new byte[26],
			title = new byte[51],
			attached = new POST_ATTACHED[11]
		};

		var fromBytes = System.Text.Encoding.ASCII.GetBytes(post.SenderName);
		Array.Copy(fromBytes, packet.fromName, Math.Min(fromBytes.Length, 25));

		var titleBytes = System.Text.Encoding.ASCII.GetBytes(post.Subject);
		Array.Copy(titleBytes, packet.title, Math.Min(titleBytes.Length, 50));

		for (int i = 0; i < 11; i++)
		{
			if (i < post.Attachments.Count && !post.AttachmentsClaimed)
			{
				packet.attached[i] = new POST_ATTACHED
				{
					typeID = (uint)(post.Attachments[i].TypeID),
					count = 1
				};
			}
			else
			{
				packet.attached[i] = new POST_ATTACHED();
			}
		}

		using PacketWriter pw = new();
		pw.Write(packet);
		player.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Sends deleted post confirmation.
	/// </summary>
	public void SendDeletedPost(PlayerClient player, int indexNumber)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_DELETED_POST
		{
			indexNumber = indexNumber
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Sends post item taken confirmation.
	/// </summary>
	public void SendPostItem(PlayerClient player, int indexNumber)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_POST_ITEM
		{
			indexNumber = indexNumber
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Loads mail for a player on login.
	/// </summary>
	public void OnPlayerLogin(PlayerClient player)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;

		// Force load from DB
		string cacheKey = $"{accountId}:{charName}";
		var mail = MongoDBService.Instance.GetPlayerMail(accountId, charName);
		_mailCache[cacheKey] = mail;

		int unreadCount = mail.Inbox.Count(m => !m.IsRead);
		if (unreadCount > 0)
		{
			SendNewPostAlarm(player, unreadCount);
		}

		Logger.Log($"[POST] Loaded {mail.Inbox.Count} mail items for {charName} ({unreadCount} unread)", LogLevel.Debug);
	}

	/// <summary>
	/// Clears cached mail for a player on logout.
	/// </summary>
	public void OnPlayerLogout(PlayerClient player)
	{
		var character = player.GetCurrentCharacter();
		if (character == null) return;

		string accountId = player.GetAccountId();
		string charName = character.appearance.name;
		string cacheKey = $"{accountId}:{charName}";

		_mailCache.TryRemove(cacheKey, out _);
	}

	#region Private Helpers

	private PlayerMail GetOrLoadMail(string accountId, string charName)
	{
		string cacheKey = $"{accountId}:{charName}";
		return _mailCache.GetOrAdd(cacheKey, _ => MongoDBService.Instance.GetPlayerMail(accountId, charName));
	}

	private void SaveMail(string accountId, string charName, PlayerMail mail)
	{
		string cacheKey = $"{accountId}:{charName}";
		_mailCache[cacheKey] = mail;
		MongoDBService.Instance.SavePlayerMail(accountId, charName, mail);
	}

	private void SendPostResult(PlayerClient player, bool success)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_SEND_POST_RESULT
		{
			isSuccess = success
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private void SendNewPostAlarm(PlayerClient player, int count)
	{
		using PacketWriter pw = new();
		pw.Write(new SC_NEW_POST_ALARM
		{
			count = count
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	#endregion
}
