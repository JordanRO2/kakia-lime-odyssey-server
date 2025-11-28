/// <summary>
/// Handles CS_RETURN_LOBBY packet - player wants to return to character selection.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking return to lobby/character select
/// Response packets: SC_PC_LIST (SC_REENTER_LOBBY removed - causes crash)
///
/// IDA Analysis findings:
/// - ms_bRegionPacketMode is set to 1 when client sends CS_CLIENT_READY (packet 0x3C)
/// - CGameApp::SwitchMode(MODE_TYPE_LOGIN) is DEFERRED with fade animation delay
/// - During fade delay, client still processes packets in old mode
/// - Zone packets (0x64+) have assertions checking ms_bRegionPacketMode
///
/// CRITICAL: Order of operations to prevent client crash:
/// 1. Save and cleanup on server side first
/// 2. Reset player state IMMEDIATELY (stops zone packet broadcasts to this player)
/// 3. Notify other players (they see player leave)
/// 4. Send SC_PC_LIST (triggers deferred mode switch to character select)
///
/// This order ensures no zone packets can arrive during the transition window.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Authentication;

[PacketHandlerAttr(PacketType.CS_RETURN_LOBBY)]
class CS_RETURN_LOBBY_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		uint objInstId = pc.GetObjInstID();
		string accountId = pc.GetAccountId();
		Logger.Log($"[AUTH] {playerName} returning to lobby", LogLevel.Information);

		// STEP 1: Save character state before any changes
		pc.Save();

		// Clean up services state
		LimeServer.PartyService.OnPlayerDisconnect(pc);
		LimeServer.GuildService.OnPlayerDisconnect(pc);
		LimeServer.ChatroomService.OnPlayerDisconnect(pc);
		LimeServer.CraftingService.CleanupPlayer(pc.GetId());
		LimeServer.SkillService.CleanupPlayer(pc.GetId());
		LimeServer.TradeService.CleanupPlayer(pc.GetId());

		// STEP 2: Reset player state IMMEDIATELY
		// This prevents receiving any more zone packets during transition
		// CRITICAL: Must happen BEFORE sending any packets
		pc.ResetToLobby();

		// STEP 3: Notify other players that this character left
		// Player won't receive this because IsLoaded() is now false
		PacketWriter leaveWriter = new();
		leaveWriter.Write(new SC_LEAVE_SIGHT_PC
		{
			leave_zone = new SC_LEAVE_ZONEOBJ { objInstID = objInstId }
		});
		pc.SendGlobalPacket(leaveWriter.ToPacket(), default).Wait();

		// STEP 4: Send SC_PC_LIST to trigger mode switch
		// Note: SC_REENTER_LOBBY was removed because SC_PC_LIST already:
		// - Clears player list (via gameProxy::OnPacketPcList)
		// - Sets login type
		// - Calls CGameApp::SwitchMode(MODE_TYPE_LOGIN)
		// SC_REENTER_LOBBY was causing a crash in game mode packet processing
		var db = DatabaseFactory.Instance;
		var characters = db.LoadPC(accountId);
		var updated = new List<CLIENT_PC>();

		foreach (var character in characters)
		{
			var characterName = global::System.Text.Encoding.ASCII.GetString(character.appearance.name).TrimEnd('\0');
			var equip = db.GetPlayerEquipment(accountId, characterName);
			var equipped = equip.Combat.GetEquipped();
			var modApp = new ModAppearance(character.appearance);
			modApp.equiped = new ModEquipped(equipped);
			modApp.playingJobClass = 1;

			updated.Add(new CLIENT_PC()
			{
				status = character.status,
				appearance = modApp.AsStruct()
			});
		}

		SC_PC_LIST charList = new()
		{
			count = (byte)characters.Count,
			pc_list = updated.ToArray()
		};

		using PacketWriter charListWriter = new();
		charListWriter.Write(charList);
		pc.Send(charListWriter.ToSizedPacket(), default).Wait();

		Logger.Log($"[AUTH] {playerName} returned to lobby", LogLevel.Information);
	}
}
