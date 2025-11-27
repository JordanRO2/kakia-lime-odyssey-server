/// <summary>
/// Handles CS_ITEM_MAKE_READY packet - player prepares to craft an item.
/// </summary>
/// <remarks>
/// Triggered by: Player selecting a recipe to craft
/// Response packets: SC_ITEM_MAKE_UPDATE_REPORT
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_ITEM_MAKE_READY)]
class CS_ITEM_MAKE_READY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_ITEM_MAKE_READY>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} preparing to craft item type {packet.typeID}", LogLevel.Debug);

		// TODO: Validate recipe exists and player can craft it
		// TODO: Check materials are available
		// TODO: Send SC_ITEM_MAKE_UPDATE_REPORT with crafting info

		Logger.Log($"[CRAFT] {playerName} ready to craft item {packet.typeID}", LogLevel.Debug);
	}
}
