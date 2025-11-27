/// <summary>
/// Handles CS_FINISH_WARP packet - client has completed warp/teleport animation.
/// </summary>
/// <remarks>
/// Triggered by: Client finishing warp animation after receiving SC_WARP
/// Response packets: None (acknowledgment packet)
/// Note: This is an empty packet that signals the client has completed warping.
/// The server can use this to trigger post-warp logic like loading zone entities.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

[PacketHandlerAttr(PacketType.CS_FINISH_WARP)]
class CS_FINISH_WARP_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[MOVEMENT] {playerName} finished warp animation", LogLevel.Debug);

		// Mark warp as complete - client has finished the warp animation
		// This can be used to trigger post-warp logic such as:
		// - Loading nearby entities (NPCs, monsters, other players)
		// - Starting zone-specific effects
		// - Updating the player's visible state to others

		// Request zone presence to load NPCs/Mobs in the new area
		pc.RequestPresence(default).Wait();

		Logger.Log($"[MOVEMENT] {playerName} warp completed at zone {pc.GetZone()}", LogLevel.Debug);
	}
}
