/// <summary>
/// Handles CS_STUFF_MAKE_READY packet - prepare for material gathering/processing.
/// </summary>
/// <remarks>
/// Triggered by: Player opening gathering/processing interface
/// Response packets: SC_STUFF_MAKE_READY_SUCCESS
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_STUFF_MAKE_READY)]
class CS_STUFF_MAKE_READY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_STUFF_MAKE_READY>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} preparing stuff make type {packet.typeID}", LogLevel.Debug);

		LimeServer.CraftingService.ReadyStuffMake(pc, packet.typeID);
	}
}
