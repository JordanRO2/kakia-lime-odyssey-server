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

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<PACKET_CS_SEND_POST>();

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
			return;
		}

		var attachments = new List<PostAttachment>();
		for (int i = 0; i < packet.attaching.Length; i++)
		{
			var att = packet.attaching[i];
			if (att.slot > 0 && att.count > 0)
			{
				attachments.Add(new PostAttachment
				{
					TypeID = att.slot,
					Count = (int)att.count
				});
			}
		}

		Logger.Log($"[POST] {playerName} sending mail to '{toName}': {title} ({attachments.Count} attachments)", LogLevel.Debug);

		LimeServer.PostService.SendPost(pc, toName, title, body, attachments);
	}
}
