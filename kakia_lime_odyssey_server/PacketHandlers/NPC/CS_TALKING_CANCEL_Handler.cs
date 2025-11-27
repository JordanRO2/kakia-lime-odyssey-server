/// <summary>
/// Handles CS_TALKING_CANCEL packet - player cancels/closes NPC dialog.
/// </summary>
/// <remarks>
/// Triggered by: Player closing NPC dialog window
/// Response packets: None
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.NPC;

[PacketHandlerAttr(PacketType.CS_TALKING_CANCEL)]
class CS_TALKING_CANCEL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[NPC] {playerName} cancelled dialog", LogLevel.Debug);

		// TODO: Clean up any dialog state
		// The client handles closing the dialog window
	}
}
