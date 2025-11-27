/// <summary>
/// Handles CS_USE_SKILL_POS packet - player uses a skill at a specific position (ground target).
/// </summary>
/// <remarks>
/// Triggered by: Player using an AoE or ground-targeted skill
/// Response packets: SC_START_CASTING_SKILL_POS (if cast time > 0), SC_USE_SKILL_POS_RESULT_LIST
/// Database: None directly
/// Note: Used for AoE skills like fireballs, ground effects, etc.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_USE_SKILL_POS)]
class CS_USE_SKILL_POS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var useSkill = PacketConverter.Extract<CS_USE_SKILL_POS>(p.Payload);

		var skill = LimeServer.SkillDB.FirstOrDefault(skill => skill.Id == useSkill.typeID);
		if (skill is null)
		{
			Logger.Log($"Skill not found with ID: {useSkill.typeID}!", LogLevel.Error);
			return;
		}

		// SERVER-SIDE COOLDOWN VALIDATION
		var playerClient = client as PlayerClient;
		var cooldownTracker = playerClient?.GetSkillCooldownTracker();
		if (cooldownTracker != null)
		{
			if (!cooldownTracker.ValidateAndTrackSkillUse((int)useSkill.typeID, skill))
			{
				Logger.Log(
					$"[COOLDOWN REJECT] Player {playerClient?.GetCurrentCharacter()?.appearance.name} " +
					$"attempted to use position skill {useSkill.typeID} while on cooldown",
					LogLevel.Warning
				);
				return;
			}
		}

		string playerName = playerClient?.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[COMBAT] {playerName} using skill {skill.Name} at position ({useSkill.pos.x}, {useSkill.pos.y}, {useSkill.pos.z})", LogLevel.Debug);

		// Get skill level (use level 1 for now)
		var skillLv1 = skill.Subject.SubjectLists.FirstOrDefault();
		uint castTime = (uint)(skillLv1 != null ? skillLv1.CastingTime : skill.CastingTime);

		// Send cast start if there's a cast time
		if (castTime > 0)
		{
			SC_START_CASTING_SKILL_POS castSkill = new()
			{
				fromInstID = client.GetObjInstID(),
				toPos = useSkill.pos,
				typeID = useSkill.typeID,
				castTime = castTime
			};

			using (PacketWriter pw = new())
			{
				pw.Write(castSkill);
				client.Send(pw.ToPacket(), default).Wait();
				client.SendGlobalPacket(pw.ToPacket(), default).Wait();
			}
		}

		// Send skill result
		SC_USE_SKILL_POS_RESULT_LIST actionSkill = new()
		{
			fromInstID = client.GetObjInstID(),
			toPos = useSkill.pos,
			typeID = useSkill.typeID,
			useHP = (ushort)(skillLv1 != null ? skillLv1.UseHP : 0),
			useMP = (ushort)(skillLv1 != null ? skillLv1.UseMP : 0),
			useLP = (ushort)(skillLv1 != null ? skillLv1.UseLP : 0),
			useSP = (ushort)(skillLv1 != null ? skillLv1.UseSP : 0),
			coolTime = (uint)(skillLv1 != null ? skillLv1.CoolTime : skill.CoolTime)
		};

		using (PacketWriter pw = new())
		{
			pw.Write(actionSkill);
			client.Send(pw.ToSizedPacket(), default).Wait();
			client.SendGlobalPacket(pw.ToSizedPacket(), default).Wait();
		}

		// TODO: Implement AoE damage calculation
		// This would involve:
		// 1. Finding all entities within skill radius of useSkill.pos
		// 2. Applying damage/effects to each entity
		// 3. Sending hit results for each affected entity

		Logger.Log($"[COMBAT] {playerName} skill {skill.Name} executed at position", LogLevel.Debug);
	}
}
