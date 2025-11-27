/// <summary>
/// Handles CS_CANCEL_CREATE_MISSION_ZONE packet - cancel mission zone creation.
/// </summary>
/// <remarks>
/// Triggered by: Player canceling dungeon/mission zone creation
/// Response packets: None
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Zone;

[PacketHandlerAttr(PacketType.CS_CANCEL_CREATE_MISSION_ZONE)]
class CS_CANCEL_CREATE_MISSION_ZONE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[ZONE] {playerName} canceling mission zone creation", LogLevel.Debug);

		// Mission zone creation is canceled - no specific cleanup needed at this time
		// The client handles the UI state change, server just acknowledges
	}
}
