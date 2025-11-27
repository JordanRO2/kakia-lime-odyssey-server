/// <summary>
/// Handles CS_HEAD_OVER_WATER packet - player surfaces above water.
/// </summary>
/// <remarks>
/// Triggered by: Player's head coming above water surface
/// Response packets: Broadcast to nearby players
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

[PacketHandlerAttr(PacketType.CS_HEAD_OVER_WATER)]
class CS_HEAD_OVER_WATER_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[MOVE] {playerName} head over water", LogLevel.Debug);

		// TODO: Update player swimming state
		// TODO: Stop breath timer if applicable
		// TODO: Notify nearby players
	}
}
