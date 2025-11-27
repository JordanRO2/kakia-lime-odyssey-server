using System.Collections.Concurrent;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Post;

public class PostService
{
	private static int _nextIndexNumber = 1;
	private readonly ConcurrentDictionary<long, List<PostMessage>> _playerMailboxes = new();
	private const int MaxMailboxSize = 100;
	private const int MaxAttachments = 11;

	public bool SendPost(PlayerClient sender, string toName, string title, string body, List<PostAttachment> attachments)
	{
		var senderChar = sender.GetCurrentCharacter();
		if (senderChar == null)
			return false;

		if (string.IsNullOrWhiteSpace(toName) || string.IsNullOrWhiteSpace(title))
			return false;

		if (attachments.Count > MaxAttachments)
			return false;

		var recipient = LimeServer.PlayerClients.FirstOrDefault(p =>
		{
			var c = p.GetCurrentCharacter();
			return c != null && c.appearance.name.Equals(toName, StringComparison.OrdinalIgnoreCase);
		});

		long recipientId;
		if (recipient != null)
		{
			recipientId = recipient.GetId();
		}
		else
		{
			SendPostResult(sender, false);
			Logger.Log($"[POST] {senderChar.appearance.name} failed to send mail to '{toName}': recipient not found", LogLevel.Debug);
			return false;
		}

		var mailbox = GetOrCreateMailbox(recipientId);
		if (mailbox.Count >= MaxMailboxSize)
		{
			SendPostResult(sender, false);
			Logger.Log($"[POST] {senderChar.appearance.name} failed to send mail to '{toName}': mailbox full", LogLevel.Debug);
			return false;
		}

		var post = new PostMessage
		{
			IndexNumber = Interlocked.Increment(ref _nextIndexNumber),
			FromName = senderChar.appearance.name,
			FromPlayerId = sender.GetId(),
			ToName = toName,
			ToPlayerId = recipientId,
			Title = title,
			Body = body,
			Attachments = attachments,
			SentAt = DateTime.Now,
			IsRead = false,
			ItemsTaken = false
		};

		mailbox.Add(post);

		SendPostResult(sender, true);

		if (recipient != null && recipient.IsConnected())
		{
			SendNewPostAlarm(recipient, GetUnreadCount(recipientId));
		}

		Logger.Log($"[POST] {senderChar.appearance.name} sent mail to '{toName}': {title}", LogLevel.Debug);
		return true;
	}

	public PostMessage? GetPost(long playerId, int indexNumber)
	{
		var mailbox = GetOrCreateMailbox(playerId);
		return mailbox.FirstOrDefault(p => p.IndexNumber == indexNumber);
	}

	public List<PostMessage> GetMailbox(long playerId)
	{
		return GetOrCreateMailbox(playerId);
	}

	public bool DeletePost(long playerId, int indexNumber)
	{
		var mailbox = GetOrCreateMailbox(playerId);
		var post = mailbox.FirstOrDefault(p => p.IndexNumber == indexNumber);
		if (post == null)
			return false;

		if (post.Attachments.Count > 0 && !post.ItemsTaken)
			return false;

		mailbox.Remove(post);
		return true;
	}

	public bool TakePostItems(PlayerClient player, int indexNumber)
	{
		long playerId = player.GetId();
		var mailbox = GetOrCreateMailbox(playerId);
		var post = mailbox.FirstOrDefault(p => p.IndexNumber == indexNumber);

		if (post == null || post.ItemsTaken || post.Attachments.Count == 0)
			return false;

		post.ItemsTaken = true;
		return true;
	}

	public int GetUnreadCount(long playerId)
	{
		var mailbox = GetOrCreateMailbox(playerId);
		return mailbox.Count(p => !p.IsRead);
	}

	public void MarkAsRead(long playerId, int indexNumber)
	{
		var mailbox = GetOrCreateMailbox(playerId);
		var post = mailbox.FirstOrDefault(p => p.IndexNumber == indexNumber);
		if (post != null)
			post.IsRead = true;
	}

	public void SendPostList(PlayerClient player)
	{
		long playerId = player.GetId();
		var mailbox = GetOrCreateMailbox(playerId);

		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_POST_LIST
		{
			count = (byte)Math.Min(mailbox.Count, 255)
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	public void SendPostDetails(PlayerClient player, int indexNumber)
	{
		long playerId = player.GetId();
		var post = GetPost(playerId, indexNumber);

		if (post == null)
			return;

		MarkAsRead(playerId, indexNumber);

		var packet = new PACKET_SC_POST
		{
			indexNumber = post.IndexNumber,
			fromName = new byte[26],
			title = new byte[51],
			attached = new POST_ATTACHED[11]
		};

		var fromBytes = System.Text.Encoding.ASCII.GetBytes(post.FromName);
		Array.Copy(fromBytes, packet.fromName, Math.Min(fromBytes.Length, 25));

		var titleBytes = System.Text.Encoding.ASCII.GetBytes(post.Title);
		Array.Copy(titleBytes, packet.title, Math.Min(titleBytes.Length, 50));

		for (int i = 0; i < 11; i++)
		{
			if (i < post.Attachments.Count && !post.ItemsTaken)
			{
				packet.attached[i] = post.Attachments[i].ToPacketStruct();
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

	public void SendDeletedPost(PlayerClient player, int indexNumber)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_DELETED_POST
		{
			indexNumber = indexNumber
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	public void SendPostItem(PlayerClient player, int indexNumber)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_POST_ITEM
		{
			indexNumber = indexNumber
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private void SendPostResult(PlayerClient player, bool success)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_SEND_POST_RESULT
		{
			isSuccess = success
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private void SendNewPostAlarm(PlayerClient player, int count)
	{
		using PacketWriter pw = new();
		pw.Write(new PACKET_SC_NEW_POST_ALARM
		{
			count = count
		});
		player.Send(pw.ToPacket(), default).Wait();
	}

	private List<PostMessage> GetOrCreateMailbox(long playerId)
	{
		return _playerMailboxes.GetOrAdd(playerId, _ => new List<PostMessage>());
	}

	public void OnPlayerLogin(PlayerClient player)
	{
		long playerId = player.GetId();
		int unreadCount = GetUnreadCount(playerId);

		if (unreadCount > 0)
		{
			SendNewPostAlarm(player, unreadCount);
		}
	}
}
