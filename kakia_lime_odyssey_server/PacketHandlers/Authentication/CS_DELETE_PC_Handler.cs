/// <summary>
/// Handles CS_DELETE_PC packet - player deletes a character.
/// </summary>
/// <remarks>
/// Triggered by: Player clicking delete on character selection screen
/// Response packets: SC_DELETED_PC
/// Database: characters (delete)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Authentication;

[PacketHandlerAttr(PacketType.CS_DELETE_PC)]
class CS_DELETE_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_DELETE_PC>(p.Payload);

		string accountId = pc.GetAccountId();
		Logger.Log($"[AUTH] Account {accountId} requesting to delete character at slot {packet.charNum}", LogLevel.Debug);

		// Get all characters for the account
		var characters = MongoDBService.Instance.GetCharacters(accountId);
		if (characters == null || packet.charNum >= characters.Count)
		{
			Logger.Log($"[AUTH] Delete failed: Invalid character slot {packet.charNum} for account {accountId}", LogLevel.Warning);
			return;
		}

		// Get the character to delete
		var characterToDelete = characters[(int)packet.charNum];
		string characterName = global::System.Text.Encoding.ASCII.GetString(characterToDelete.appearance.name).TrimEnd('\0');

		Logger.Log($"[AUTH] Deleting character {characterName} (slot {packet.charNum}) for account {accountId}", LogLevel.Information);

		// Delete the character from database
		bool success = MongoDBService.Instance.DeleteCharacter(accountId, characterName);

		if (success)
		{
			Logger.Log($"[AUTH] Character {characterName} deleted successfully", LogLevel.Information);

			// Send deletion confirmation
			var response = new SC_DELETED_PC
			{
				charNum = packet.charNum
			};

			using PacketWriter pw = new();
			pw.Write(response);
			pc.Send(pw.ToSizedPacket(), default).Wait();
		}
		else
		{
			Logger.Log($"[AUTH] Failed to delete character {characterName}", LogLevel.Error);
		}
	}
}
