/// <summary>
/// Handles CS_QUEST_COMPLETE packet - player submits quest for completion.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking "Complete" in quest turn-in dialog
/// Response packets: SC_QUEST_COMPLETE (via QuestService)
/// Note: Quest ID comes from pending turn-in state set by SC_QUEST_REPORT_TALK.
/// The client only sends reward choice selections, not the quest ID.
/// </remarks>
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

		// Get quest ID from pending turn-in state (set when SC_QUEST_REPORT_TALK was sent)
		uint questTypeID = pc.GetPendingTurnInQuest();

		if (questTypeID == 0)
		{
			Logger.Log($"[QUEST] {playerName} tried to complete quest but no pending turn-in", LogLevel.Warning);
			return;
		}

		Logger.Log($"[QUEST] {playerName} completing quest {questTypeID} with reward choices", LogLevel.Debug);

		bool success = LimeServer.QuestService.CompleteQuest(pc, questTypeID, packet.choiceItems);

		// Clear pending turn-in regardless of success (dialog closes either way)
		pc.ClearPendingTurnInQuest();

		if (!success)
		{
			Logger.Log($"[QUEST] {playerName} failed to complete quest {questTypeID}", LogLevel.Warning);
		}
	}
}
