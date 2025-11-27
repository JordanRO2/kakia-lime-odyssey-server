/// <summary>
/// Handles CS_UNRIDE_PC packet - player dismounts from mount/pet.
/// </summary>
/// <remarks>
/// Triggered by: Player pressing dismount or using dismount command
/// Response packets: SC_UNRIDE_PC
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

[PacketHandlerAttr(PacketType.CS_UNRIDE_PC)]
class CS_UNRIDE_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[MOUNT] {playerName} dismounting", LogLevel.Debug);

		// Mount state is handled client-side for now
		// Server just acknowledges and broadcasts the dismount

		// Send dismount confirmation to player and broadcast to others
		SC_UNRIDE_PC response = new()
		{
			objInstID = pc.GetObjInstID()
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();
		pc.SendGlobalPacket(pw.ToPacket(), default).Wait();

		Logger.Log($"[MOUNT] {playerName} dismounted successfully", LogLevel.Debug);
	}
}
