/// <summary>
/// Handles CS_BOARD_CANCEL packet - player closes quest board.
/// </summary>
/// <remarks>
/// Triggered by: Player closing the quest board UI
/// Response packets: None
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Quest;

[PacketHandlerAttr(PacketType.CS_BOARD_CANCEL)]
class CS_BOARD_CANCEL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[QUEST] {playerName} closing quest board", LogLevel.Debug);

		// Quest board UI closure is handled client-side
		// Server just acknowledges the close event
	}
}
