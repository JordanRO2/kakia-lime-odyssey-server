/// <summary>
/// Handles CS_REQUEST_PC_STATUS packet - request another player's status.
/// </summary>
/// <remarks>
/// Triggered by: Player inspecting another player
/// Response packets: SC_PC_STATUS or similar
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Character;

[PacketHandlerAttr(PacketType.CS_REQUEST_PC_STATUS)]
class CS_REQUEST_PC_STATUS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_REQUEST_PC_STATUS>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHAR] {playerName} requesting status of player {packet.objInstID}", LogLevel.Debug);

		// Find target player by objInstID
		var targetPlayer = LimeServer.PlayerClients.FirstOrDefault(p => p.GetObjInstID() == packet.objInstID);
		if (targetPlayer == null)
		{
			Logger.Log($"[CHAR] Target player {packet.objInstID} not found", LogLevel.Debug);
			return;
		}

		// Get target player's character info
		var targetChar = targetPlayer.GetCurrentCharacter();
		if (targetChar == null) return;

		// Send target player's status info
		using PacketWriter pw = new();
		pw.Write(new SC_PC_STATUS
		{
			objInstID = packet.objInstID,
			status = targetChar.status.AsPacketStruct()
		});
		pc.Send(pw.ToPacket(), default).Wait();
	}
}
