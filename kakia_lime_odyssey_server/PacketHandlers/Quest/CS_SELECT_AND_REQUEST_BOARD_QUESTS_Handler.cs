/// <summary>
/// Handles CS_SELECT_AND_REQUEST_BOARD_QUESTS packet - player selects quest board and requests quests.
/// </summary>
/// <remarks>
/// Triggered by: Player selecting a quest board NPC/object
/// Response packets: SC_BOARD_QUESTS
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Quest;

[PacketHandlerAttr(PacketType.CS_SELECT_AND_REQUEST_BOARD_QUESTS)]
class CS_SELECT_AND_REQUEST_BOARD_QUESTS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_SELECT_AND_REQUEST_BOARD_QUESTS>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} selecting quest board {packet.objInstID} and requesting quests", LogLevel.Debug);

		// Set the quest board as target
		pc.SetCurrentTarget(packet.objInstID);

		// Get quests from QuestService and send to player
		LimeServer.QuestService.GetBoardQuests(pc, packet.objInstID);
	}
}
