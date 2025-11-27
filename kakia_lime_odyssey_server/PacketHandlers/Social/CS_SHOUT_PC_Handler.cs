/// <summary>
/// Handles CS_SHOUT_PC packet - player sends a zone-wide shout message.
/// </summary>
/// <remarks>
/// Triggered by: Player using shout chat channel
/// Response packets: SC_SHOUT (broadcast to all players in zone)
/// </remarks>
using System.Text;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Social;

[PacketHandlerAttr(PacketType.CS_SHOUT_PC)]
class CS_SHOUT_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var shout = PacketConverter.Extract<CS_SHOUT_PC>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		Logger.Log($"[CHAT:SHOUT] {playerName}: {shout.message}", LogLevel.Debug);

		// Broadcast shout to all players in the zone
		using PacketWriter pw = new();
		SC_SHOUT response = new()
		{
			instID = pc.GetObjInstID(),
			name = Encoding.ASCII.GetBytes(playerName)
		};
		pw.Write(response);
		// Write variable-length message
		pw.Write(Encoding.ASCII.GetBytes(shout.message));
		pw.Write((byte)0); // Null terminator

		// Send to self
		pc.Send(pw.ToSizedPacket(), default).Wait();
		// Broadcast to all other players in zone
		pc.SendGlobalPacket(pw.ToSizedPacket(), default).Wait();
	}
}
