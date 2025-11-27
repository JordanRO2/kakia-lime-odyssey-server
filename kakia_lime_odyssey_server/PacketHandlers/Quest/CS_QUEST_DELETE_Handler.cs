/// <summary>
/// Handles CS_QUEST_DELETE packet - player abandons a quest.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking "Abandon" on a quest in quest log
/// Response packets: SC_QUEST_DELETE (via QuestService)
/// Database: PlayerQuests (write)
/// Validates: Quest is active, quest is cancelable
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Quest;

[PacketHandlerAttr(PacketType.CS_QUEST_DELETE)]
class CS_QUEST_DELETE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_QUEST_DELETE>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} abandoning quest {packet.typeID}", LogLevel.Debug);

		LimeServer.QuestService.AbandonQuest(pc, packet.typeID);
	}
}
