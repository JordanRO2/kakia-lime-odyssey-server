/// <summary>
/// Handles CS_RETURN_LOBBY packet - player wants to return to character selection.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking return to lobby/character select
/// Response packets: SC_REENTER_LOBBY
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Authentication;

[PacketHandlerAttr(PacketType.CS_RETURN_LOBBY)]
class CS_RETURN_LOBBY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[AUTH] {playerName} returning to lobby", LogLevel.Information);

		// TODO: Save character state before returning
		// TODO: Notify other players of character leaving
		// TODO: Clean up zone presence

		// Send lobby reenter confirmation
		SC_REENTER_LOBBY response = new();

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();

		// TODO: Reset client state to pre-game

		Logger.Log($"[AUTH] {playerName} returned to lobby", LogLevel.Information);
	}
}
