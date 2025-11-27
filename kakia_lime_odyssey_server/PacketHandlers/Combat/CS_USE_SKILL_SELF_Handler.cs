using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Combat;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_USE_SKILL_SELF)]
class CS_USE_SKILL_SELF_Handler : PacketHandler
{
	private static readonly HealingService _healingService = new();

	// Skill types that are considered healing skills
	private static readonly HashSet<string> HealingSkillTypes = new(StringComparer.OrdinalIgnoreCase)
	{
		"Heal",
		"Recovery",
		"Cure",
		"Regeneration",
		"Restoration",
		"FirstAid",
		"Blessing"
	};

	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var useSkill = PacketConverter.Extract<CS_USE_SKILL_ACTION_TARGET>(p.Payload);

		var skill = LimeServer.SkillDB.FirstOrDefault(skill => skill.Id == useSkill.typeID);
		if (skill is null)
		{
			Logger.Log($"Skill not found with ID: {useSkill.typeID}!", LogLevel.Error);
			return;
		}

		// SERVER-SIDE COOLDOWN VALIDATION (Anti-cheat requirement from IDA analysis)
		var playerClient = client as PlayerClient;
		var cooldownTracker = playerClient?.GetSkillCooldownTracker();
		if (cooldownTracker != null)
		{
			// Validate skill cooldown - if violated, reject the skill use
			if (!cooldownTracker.ValidateAndTrackSkillUse((int)useSkill.typeID, skill))
			{
				Logger.Log(
					$"[COOLDOWN REJECT] Player {playerClient?.GetCurrentCharacter().appearance.name} " +
					$"attempted to use self-skill {useSkill.typeID} while on cooldown",
					LogLevel.Warning
				);
				return; // Reject skill usage - cooldown not ready
			}
		}

		// Go with skill level 1 for now, since we haven't actually implemented skills for real
		var skillLv1 = skill.Subject.SubjectLists.FirstOrDefault();
		uint castTime = (uint)(skillLv1 != null ? skillLv1.CastingTime : skill.CastingTime);

		if (castTime > 0)
		{
			SC_START_CASTING_SKILL_OBJ castSkill = new()
			{
				fromInstID = client.GetObjInstID(),
				targetInstID = client.GetObjInstID(),
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

		// Check if this is a healing skill
		bool isHealingSkill = IsHealingSkill(skill.Type);

		if (isHealingSkill && client is IEntity entity)
		{
			// Handle healing skill
			var healResult = _healingService.HealSelf(entity, (int)useSkill.typeID, 1);

			if (healResult.Packet != null)
			{
				client.Send(healResult.Packet, default).Wait();
				client.SendGlobalPacket(healResult.Packet, default).Wait();
			}

			// Apply the heal
			entity.UpdateHealth((int)healResult.HealAmount);

			// Send HP change notification
			if (healResult.HPChangePacket != null)
			{
				client.Send(healResult.HPChangePacket, default).Wait();
				client.SendGlobalPacket(healResult.HPChangePacket, default).Wait();
			}

			// Log critical heals
			if (healResult.IsCritical)
			{
				Logger.Log(
					$"[CRITICAL HEAL] {playerClient?.GetCurrentCharacter().appearance.name} " +
					$"healed self for {healResult.HealAmount} HP (critical!)",
					LogLevel.Info
				);
			}
		}
		else
		{
			// Non-healing self skill (buffs, etc.)
			SC_USE_SKILL_OBJ_RESULT_LIST actionSkill = new()
			{
				fromInstID = client.GetObjInstID(),
				toInstID = client.GetObjInstID(),
				typeID = useSkill.typeID,
				useHP = (short)(skillLv1 != null ? skillLv1.UseHP : 0),
				useMP = (short)(skillLv1 != null ? skillLv1.UseMP : 0),
				useLP = (short)(skillLv1 != null ? skillLv1.UseLP : 0),
				useSP = (short)(skillLv1 != null ? skillLv1.UseSP : 0),
				coolTime = (uint)(skillLv1 != null ? skillLv1.CoolTime : skill.CoolTime)
			};

			using (PacketWriter pw = new())
			{
				pw.Write(actionSkill);
				client.Send(pw.ToSizedPacket(), default).Wait();
				client.SendGlobalPacket(pw.ToSizedPacket(), default).Wait();
			}

			// Apply buffs if this is a buff skill
			if (client is IEntity entity)
			{
				ApplySkillBuffs(entity, skill, 1, client);
			}
		}
	}

	private static bool IsHealingSkill(string skillType)
	{
		return HealingSkillTypes.Contains(skillType) ||
		       skillType.Contains("Heal", StringComparison.OrdinalIgnoreCase) ||
		       skillType.Contains("Recovery", StringComparison.OrdinalIgnoreCase) ||
		       skillType.Contains("Cure", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Applies buffs from a skill to the caster (self-buff).
	/// </summary>
	private static void ApplySkillBuffs(IEntity caster, Models.SkillXML.XmlSkill skill, int skillLevel, IPlayerClient client)
	{
		// Check if this is a buff skill type
		if (!BuffService.IsBuffSkill(skill.Type))
			return;

		var buffResults = LimeServer.BuffService.ApplyBuffsFromSkill(skill, skillLevel, caster, caster);

		foreach (var result in buffResults)
		{
			if (result.Success && result.Packet != null)
			{
				// Send buff application packet to the caster
				client.Send(result.Packet, default).Wait();
				// Broadcast to nearby players
				client.SendGlobalPacket(result.Packet, default).Wait();

				Logger.Log(
					$"[BUFF] Applied buff (InstID:{result.BuffInstId}) from skill {skill.Name} to self",
					LogLevel.Debug
				);
			}
		}
	}
}
