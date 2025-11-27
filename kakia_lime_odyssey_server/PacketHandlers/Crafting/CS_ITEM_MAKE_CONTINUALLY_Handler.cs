/// <summary>
/// Handles CS_ITEM_MAKE_CONTINUALLY packet - player starts continuous crafting.
/// </summary>
/// <remarks>
/// Triggered by: Player selecting continuous crafting mode
/// Response packets: SC_ITEM_MAKE_START_CASTING
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_ITEM_MAKE_CONTINUALLY)]
class CS_ITEM_MAKE_CONTINUALLY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_ITEM_MAKE_CONTINUALLY>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} starting continuous crafting x{packet.count}", LogLevel.Debug);

		LimeServer.CraftingService.StartContinuousCrafting(pc, packet.count);
	}
}
