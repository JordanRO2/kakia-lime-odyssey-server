/// <summary>
/// Handles CS_TALKING_CONFIRM packet - player confirms/proceeds in NPC dialog.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking confirm/next in NPC dialog
/// Response packets: SC_TALKING (next dialog) or closes dialog
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.NPC;

[PacketHandlerAttr(PacketType.CS_TALKING_CONFIRM)]
class CS_TALKING_CONFIRM_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[NPC] {playerName} confirmed dialog", LogLevel.Debug);

		// TODO: Advance dialog state, trigger quest actions, etc.
		// For now, just log the confirmation - the dialog system would
		// track state and send the next appropriate response
	}
}
