/// <summary>
/// Handles CS_TALKING_CHOICE_CLICK packet - player selects a dialog choice.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking a choice in NPC dialog menu
/// Response packets: SC_TALKING (response to choice) or other actions
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.NPC;

[PacketHandlerAttr(PacketType.CS_TALKING_CHOICE_CLICK)]
class CS_TALKING_CHOICE_CLICK_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var choice = PacketConverter.Extract<CS_TALKING_CHOICE_CLICK>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		long targetId = pc.GetCurrentTarget();

		Logger.Log($"[NPC] {playerName} selected dialog choice {choice.choiceNum} with NPC {targetId}", LogLevel.Debug);

		// TODO: Process the choice - this would typically:
		// - Advance quest state
		// - Open shop window
		// - Start crafting
		// - Trigger other NPC actions

		// For now, send a default response
		SC_TALKING talking = new()
		{
			objInstID = targetId,
			dialog = $"You selected option {choice.choiceNum}. This feature is not yet implemented."
		};

		using PacketWriter pw = new();
		pw.Write(talking);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}
}
