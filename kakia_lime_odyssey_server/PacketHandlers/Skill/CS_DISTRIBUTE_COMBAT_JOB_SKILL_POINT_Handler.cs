/// <summary>
/// Handles CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT packet - player invests skill points.
/// </summary>
/// <remarks>
/// Triggered by: Player distributing skill points from combat job level ups
/// Response packets: SC_SKILL_LV_UP on success
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

[PacketHandlerAttr(PacketType.CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT)]
class CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[SKILL] {playerName} distributing {packet.point} points to skill {packet.skillTypeID}", LogLevel.Debug);

		LimeServer.SkillService.DistributeSkillPoints(pc, packet.skillTypeID, packet.point);
	}
}
