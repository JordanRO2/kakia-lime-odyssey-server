/// <summary>
/// Handles CS_READY_WEAPON_HITING packet - weapon hit ready notification.
/// </summary>
/// <remarks>
/// Triggered by: Client notifying server weapon hit is ready
/// Response packets: None
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_READY_WEAPON_HITING)]
class CS_READY_WEAPON_HITING_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[COMBAT] {playerName} weapon hit ready", LogLevel.Debug);

		// Weapon hit timing is handled by the combat system
		// The client sends this to confirm it's ready for damage application
		// Actual damage is calculated and sent via SC_WEAPON_HIT_RESULT
	}
}
