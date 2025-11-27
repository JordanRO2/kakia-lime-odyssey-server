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
using kakia_lime_odyssey_packets.Packets.SC;
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

		pc.SetUnderwater(false);

		// Broadcast surface state to nearby players
		using PacketWriter pw = new();
		pw.Write(new SC_HEAD_OVER_WATER { objInstID = pc.GetObjInstID() });
		pc.SendGlobalPacket(pw.ToPacket(), default).Wait();
	}
}
