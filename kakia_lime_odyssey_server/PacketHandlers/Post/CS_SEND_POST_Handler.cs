/// <summary>
/// Handles CS_SEND_POST packet - player sends mail to another player.
/// </summary>
/// <remarks>
/// Triggered by: Player sending mail from post UI
/// Response packets: SC_SEND_POST_RESULT
/// Database: mail (write)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Post;
using System.Text;

namespace kakia_lime_odyssey_server.PacketHandlers.Post;

[PacketHandlerAttr(PacketType.CS_SEND_POST)]
class CS_SEND_POST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new PacketReader(p.Payload);
		var packet = PacketConverter.Extract<CS_SEND_POST>(p.Payload);

		string toName = Encoding.ASCII.GetString(packet.toName).TrimEnd('\0');
		string title = Encoding.ASCII.GetString(packet.title).TrimEnd('\0');

		string body = string.Empty;
		if (packet.len > 0)
		{
			var bodyBytes = pr.ReadBytes(packet.len);
			body = Encoding.ASCII.GetString(bodyBytes).TrimEnd('\0');
		}

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (string.IsNullOrWhiteSpace(toName))
		{
			Logger.Log($"[POST] {playerName} tried to send mail with empty recipient", LogLevel.Debug);
			LimeServer.PostService.SendPostResultDirect(pc, false);
			return;
		}

		// Get player inventory to extract actual item data from slots
		var inventory = pc.GetInventory();
		var attachments = new List<PostAttachment>();
		var itemsToRemove = new List<(int slot, long count)>();

		for (int i = 0; i < packet.attaching.Length; i++)
		{
			var att = packet.attaching[i];
			if (att.slot >= 0 && att.count > 0)
			{
				// Get item from inventory slot
				var item = inventory.AtSlot(att.slot) as Models.Item;
				if (item == null)
				{
					Logger.Log($"[POST] {playerName} tried to attach item from empty slot {att.slot}", LogLevel.Debug);
					continue;
				}

				// Validate count
				if ((long)item.GetAmount() < att.count)
				{
					Logger.Log($"[POST] {playerName} tried to attach more items than available in slot {att.slot}", LogLevel.Debug);
					continue;
				}

				attachments.Add(new PostAttachment
				{
					TypeID = item.Id,
					Count = (int)att.count,
					Durability = item.GetDurability(),
					MaxDurability = item.GetMaxDurability(),
					Grade = item.Grade,
					RemainExpiryTime = -1,
					Inherits = item.GetInherits()
				});

				itemsToRemove.Add((att.slot, att.count));
			}
		}

		Logger.Log($"[POST] {playerName} sending mail to '{toName}': {title} ({attachments.Count} attachments)", LogLevel.Debug);

		// Send mail first
		bool success = LimeServer.PostService.SendPost(pc, toName, title, body, attachments);

		// Only remove items from inventory if mail was sent successfully
		if (success && itemsToRemove.Count > 0)
		{
			foreach (var (slot, count) in itemsToRemove)
			{
				var item = inventory.AtSlot(slot) as Models.Item;
				if (item != null)
				{
					if ((long)item.GetAmount() <= count)
					{
						inventory.RemoveItem(slot);
					}
					else
					{
						item.UpdateAmount(item.GetAmount() - (ulong)count);
					}
				}
			}
			Logger.Log($"[POST] Removed {itemsToRemove.Count} items from {playerName}'s inventory", LogLevel.Debug);
		}
	}
}
