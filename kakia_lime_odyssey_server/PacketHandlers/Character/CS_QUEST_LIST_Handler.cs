using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Character;

[PacketHandlerAttr(PacketType.CS_QUEST_LIST)]
class CS_QUEST_LIST_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var quests = client.GetQuests();

		SC_QUEST_LIST questList = new()
		{
			count = quests.ActiveQuestCount,
			completedMain = quests.CompletedMainQuests,
			completedSub = quests.CompletedSubQuests,
			completedNormal = quests.CompletedNormalQuests
		};

		using PacketWriter pw = new();
		pw.Write(questList);
		client.Send(pw.ToSizedPacket(), default).Wait();

		// Send individual quest data for each active quest
		foreach (var questId in quests.GetActiveQuestIds())
		{
			SC_QUEST_ADD questAdd = new()
			{
				typeID = (uint)questId
			};

			using PacketWriter qpw = new();
			qpw.Write(questAdd);
			client.Send(qpw.ToSizedPacket(), default).Wait();
		}

		Logger.Log($"Sent quest list: {quests.ActiveQuestCount} active, {quests.CompletedMainQuests} main, {quests.CompletedSubQuests} sub, {quests.CompletedNormalQuests} normal completed");
	}
}
