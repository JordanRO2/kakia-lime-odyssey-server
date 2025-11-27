/// <summary>
/// Handles CS_REALM_SAY_PC packet - player sends a realm-wide chat message.
/// </summary>
/// <remarks>
/// Triggered by: Player using realm/faction chat channel
/// Response packets: SC_REALM_SAY (broadcast to all realm/faction members)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Social;

[PacketHandlerAttr(PacketType.CS_REALM_SAY_PC)]
class CS_REALM_SAY_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var chat = pr.Read<CS_REALM_SAY_PC>();
		string message = pr.ReadRemaining();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHAT:REALM] {playerName}: {message}", LogLevel.Debug);

		// TODO: Get player's realm/faction
		// TODO: Broadcast SC_REALM_SAY to all realm members
	}
}
