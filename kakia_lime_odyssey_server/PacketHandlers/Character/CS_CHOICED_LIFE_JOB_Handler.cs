/// <summary>
/// Handles CS_CHOICED_LIFE_JOB packet - player selects a life job (crafting profession).
/// </summary>
/// <remarks>
/// Triggered by: Player selecting a life job at NPC
/// Response packets: SC_SELECTED_LIFE_JOB
/// Database: characters (update life job)
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

[PacketHandlerAttr(PacketType.CS_CHOICED_LIFE_JOB)]
class CS_CHOICED_LIFE_JOB_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var choice = PacketConverter.Extract<CS_CHOICED_LIFE_JOB>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[JOB] {playerName} selecting life job index {choice.index}", LogLevel.Information);

		// Get current character
		var character = pc.GetCurrentCharacter();
		if (character == null) return;

		// Validate: can't change job if already selected a valid job
		if (character.status.lifeJob.typeID > 0)
		{
			Logger.Log($"[JOB] {playerName} already has life job {character.status.lifeJob.typeID}", LogLevel.Warning);
			return;
		}

		// Update character's life job type (saved on next character save)
		character.status.lifeJob.typeID = choice.index;

		// Send confirmation
		SC_SELECTED_LIFE_JOB response = new()
		{
			objInstID = pc.GetObjInstID(),
			jobTypeID = (sbyte)choice.index
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();
		// Broadcast to others so they see the job change
		pc.SendGlobalPacket(pw.ToPacket(), default).Wait();

		Logger.Log($"[JOB] {playerName} selected life job {choice.index}", LogLevel.Information);
	}
}
