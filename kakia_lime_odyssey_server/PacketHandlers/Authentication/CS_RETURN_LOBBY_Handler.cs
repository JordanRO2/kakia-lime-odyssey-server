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

		// Save character state before returning
		pc.Save();

		// Notify other players that this character is leaving
		pc.SendGlobalPacket(new PacketWriter().Write(new SC_LEAVE_SIGHT_PC
		{
			leave_zone = new SC_LEAVE_ZONEOBJ { objInstID = pc.GetObjInstID() }
		}).ToPacket(), default).Wait();

		// Clean up services state
		LimeServer.PartyService.OnPlayerDisconnect(pc);
		LimeServer.GuildService.OnPlayerDisconnect(pc);
		LimeServer.ChatroomService.OnPlayerDisconnect(pc);
		LimeServer.CraftingService.CleanupPlayer(pc.GetId());
		LimeServer.SkillService.CleanupPlayer(pc.GetId());
		LimeServer.TradeService.CleanupPlayer(pc.GetId());

		// Send lobby reenter confirmation
		SC_REENTER_LOBBY response = new();

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();

		// Reset to pre-game state (character selection)
		pc.ResetToLobby();

		Logger.Log($"[AUTH] {playerName} returned to lobby", LogLevel.Information);
	}
}
