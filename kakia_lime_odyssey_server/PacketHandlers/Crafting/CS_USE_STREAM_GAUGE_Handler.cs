/// <summary>
/// Handles CS_USE_STREAM_GAUGE packet - stream gauge interaction (fishing/gathering).
/// </summary>
/// <remarks>
/// Triggered by: Player interacting with stream gauge minigame
/// Response packets: SC_USE_STREAM_GAUGE_RESULT
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Crafting;

[PacketHandlerAttr(PacketType.CS_USE_STREAM_GAUGE)]
class CS_USE_STREAM_GAUGE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CRAFT] {playerName} using stream gauge", LogLevel.Debug);

		// TODO: Process stream gauge result
		// TODO: Send SC_USE_STREAM_GAUGE_RESULT
	}
}
