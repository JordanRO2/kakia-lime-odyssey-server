/// <summary>
/// Handles CS_QUEST_ADD packet - player accepts a quest.
/// </summary>
/// <remarks>
/// Triggered by: Player accepting quest from NPC or quest board
/// Response packets: SC_QUEST_ADD (via QuestService)
/// Database: PlayerQuests (write)
/// Validates: Quest exists, level requirement, not already active/completed
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Quest;

[PacketHandlerAttr(PacketType.CS_QUEST_ADD)]
class CS_QUEST_ADD_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_QUEST_ADD>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} accepting quest {packet.typeID}", LogLevel.Debug);

		LimeServer.QuestService.AcceptQuest(pc, packet.typeID);
	}
}
