/// <summary>
/// Handles CS_STUFF_MAKE_START packet - start material gathering/processing.
/// </summary>
/// <remarks>
/// Triggered by: Player starting gathering/processing action
/// Response packets: SC_STUFF_MAKE_START_CASTING
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_STUFF_MAKE_START)]
class CS_STUFF_MAKE_START_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} starting stuff make", LogLevel.Debug);

		LimeServer.CraftingService.StartStuffMake(pc);
	}
}
