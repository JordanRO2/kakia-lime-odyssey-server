/// <summary>
/// Handles CS_CANCEL_USING_SKILL packet - player cancels skill cast.
/// </summary>
/// <remarks>
/// Triggered by: Player canceling a skill being cast
/// Response packets: None (casting stops silently)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_CANCEL_USING_SKILL)]
class CS_CANCEL_USING_SKILL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[COMBAT] {playerName} canceling skill cast", LogLevel.Debug);

		// TODO: Cancel any pending skill cast
		// TODO: Clear casting state
		// TODO: Notify nearby players if needed

		Logger.Log($"[COMBAT] {playerName} skill cast canceled", LogLevel.Debug);
	}
}
