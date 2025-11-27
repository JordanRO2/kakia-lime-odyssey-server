/// <summary>
/// Handles CS_REQUEST_LEARN_SKILL packet - player learns a new skill from NPC trainer.
/// </summary>
/// <remarks>
/// Triggered by: Player interacting with skill trainer NPC and selecting a skill
/// Response packets: SC_SKILL_ADD on success
/// Database: player_skills (write)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Skill;

[PacketHandlerAttr(PacketType.CS_REQUEST_LEARN_SKILL)]
class CS_REQUEST_LEARN_SKILL_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_REQUEST_LEARN_SKILL>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[SKILL] {playerName} requesting to learn skill {packet.typeID} at level {packet.lv}", LogLevel.Debug);

		LimeServer.SkillService.LearnSkill(pc, packet.typeID, packet.lv);
	}
}
