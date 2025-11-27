/// <summary>
/// Handles CS_REQUEST_TALKING packet - player requests to continue dialog with targeted NPC.
/// </summary>
/// <remarks>
/// Triggered by: Player interacting with already targeted NPC
/// Response packets: SC_TALKING or SC_TALKING_CHOICE
/// Note: Used when NPC is already selected (vs CS_SELECT_AND_REQUEST_TALKING)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.NPC;

[PacketHandlerAttr(PacketType.CS_REQUEST_TALKING)]
class CS_REQUEST_TALKING_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		long targetId = pc.GetCurrentTarget();

		Logger.Log($"[NPC] {playerName} requesting dialog with NPC {targetId}", LogLevel.Debug);

		// Get NPC dialog from quest/NPC system
		// Initial dialog is sent by QuestService based on NPC type and active quests
		SC_TALKING talking = new()
		{
			objInstID = targetId,
			dialog = "Hello, traveler. How may I help you today?"
		};

		using PacketWriter pw = new();
		pw.Write(talking);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}
}
