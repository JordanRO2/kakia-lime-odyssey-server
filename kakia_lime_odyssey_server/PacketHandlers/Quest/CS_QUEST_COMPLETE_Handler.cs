using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Quest;

[PacketHandlerAttr(PacketType.CS_QUEST_COMPLETE)]
class CS_QUEST_COMPLETE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_QUEST_COMPLETE>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} completing quest with reward choices", LogLevel.Debug);

		// Note: questTypeID would normally come from quest context/NPC dialog
		// For now we pass 0 - actual implementation would track active turn-in quest
		uint questTypeID = 0;
		LimeServer.QuestService.CompleteQuest(pc, questTypeID, packet.choiceItems);
	}
}
