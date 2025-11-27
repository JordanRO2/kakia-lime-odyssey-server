/// <summary>
/// Handles CS_CHOICED_COMBAT_JOB packet - player selects a combat job (class).
/// </summary>
/// <remarks>
/// Triggered by: Player selecting a combat job at NPC
/// Response packets: SC_SELECTED_COMBAT_JOB
/// Database: characters (update combat job)
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

[PacketHandlerAttr(PacketType.CS_CHOICED_COMBAT_JOB)]
class CS_CHOICED_COMBAT_JOB_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_CHOICED_COMBAT_JOB>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[JOB] {playerName} selecting combat job index {packet.index}", LogLevel.Information);

		// Get current character
		var character = pc.GetCurrentCharacter();
		if (character == null) return;

		// Validate: can't change job if already selected a valid job
		if (character.status.combatJob.typeID > 0)
		{
			Logger.Log($"[JOB] {playerName} already has combat job {character.status.combatJob.typeID}", LogLevel.Warning);
			return;
		}

		// Update character's combat job type (saved on next character save)
		character.status.combatJob.typeID = packet.index;

		// Send confirmation
		SC_SELECTED_COMBAT_JOB response = new()
		{
			objInstID = pc.GetObjInstID(),
			jobTypeID = (sbyte)packet.index
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();
		// Broadcast to others so they see the job change
		pc.SendGlobalPacket(pw.ToPacket(), default).Wait();

		Logger.Log($"[JOB] {playerName} selected combat job {packet.index}", LogLevel.Information);
	}
}
