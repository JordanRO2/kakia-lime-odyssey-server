/// <summary>
/// Handles CS_REQUEST_BOARD_QUESTS packet - player requests quest board list.
/// </summary>
/// <remarks>
/// Triggered by: Player requesting quest board when already selected
/// Response packets: SC_BOARD_QUESTS
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Quest;

[PacketHandlerAttr(PacketType.CS_REQUEST_BOARD_QUESTS)]
class CS_REQUEST_BOARD_QUESTS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} requesting quest board list", LogLevel.Debug);

		// TODO: Get quest board quests from selected target
		// For now send empty list
		SendBoardQuests(pc);
	}

	private static void SendBoardQuests(PlayerClient pc)
	{
		// SC_BOARD_QUESTS is a variable packet - send empty list for now
		SC_BOARD_QUESTS response = new();

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}
}
