/// <summary>
/// Handles CS_USE_SKILL_OBJ packet - player uses a skill on a specific target object.
/// </summary>
/// <remarks>
/// Triggered by: Player using a skill on a target (target specified in packet)
/// Response packets: SC_START_CASTING_SKILL_OBJ (if cast time > 0), SC_USE_SKILL_OBJ_RESULT_LIST
/// Database: None directly
/// Note: Similar to CS_USE_SKILL_ACTION_TARGET but target is specified in the packet
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Combat;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Combat;
using kakia_lime_odyssey_server.Services.Notification;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_USE_SKILL_OBJ)]
class CS_USE_SKILL_OBJ_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var useSkill = PacketConverter.Extract<CS_USE_SKILL_OBJ>(p.Payload);

		var skill = LimeServer.SkillDB.FirstOrDefault(skill => skill.Id == useSkill.typeID);
		if (skill is null)
		{
			Logger.Log($"Skill not found with ID: {useSkill.typeID}!", LogLevel.Error);
			SystemNotificationService.SendSkillError(client, SystemErrorCode.SkillNotFound, useSkill.typeID);
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
					$"attempted to use skill {useSkill.typeID} on object {useSkill.objInstID} while on cooldown",
					LogLevel.Warning
				);
				SystemNotificationService.SendSkillError(client, SystemErrorCode.SkillOnCooldown, useSkill.typeID);
				return;
			}
		}

		// Get target from packet (not from selected target)
		if (!LimeServer.TryGetEntity(useSkill.objInstID, out IEntity? target) || target is null)
		{
			SystemNotificationService.SendCombatError(client, SystemErrorCode.TargetInvalid);
			return;
		}

		if (target.GetEntityStatus().BasicStatus.Hp == 0)
		{
			SystemNotificationService.SendCombatError(client, SystemErrorCode.TargetDead);
			return;
		}

		// Get skill level (use level 1 for now)
		var skillLv1 = skill.Subject.SubjectLists.FirstOrDefault();
		uint castTime = (uint)(skillLv1 != null ? skillLv1.CastingTime : skill.CastingTime);

		// Send cast start if there's a cast time
		if (castTime > 0)
		{
			SC_START_CASTING_SKILL_OBJ castSkill = new()
			{
				fromInstID = client.GetObjInstID(),
				targetInstID = useSkill.objInstID,
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
		SC_USE_SKILL_OBJ_RESULT_LIST actionSkill = new()
		{
			fromInstID = client.GetObjInstID(),
			toInstID = useSkill.objInstID,
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

		// Apply buffs or debuffs from skill if applicable
		if (client is IEntity casterEntity)
		{
			ApplySkillEffects(casterEntity, target, skill, 1, client);
		}

		// Calculate and apply damage
		var damage = DamageHandler.DealWeaponHitDamage((client as IEntity)!, target);
		if (damage.Packet is null)
		{
			SystemNotificationService.SendError(client, SystemErrorCode.InternalError, "Damage calculation failed");
			return;
		}

		// Send bullet packet first for ranged attacks
		if (damage.IsRanged && damage.BulletPacket != null)
		{
			client.Send(damage.BulletPacket, default).Wait();
			client.SendGlobalPacket(damage.BulletPacket, default).Wait();
		}

		client.Send(damage.Packet, default).Wait();
		client.SendGlobalPacket(damage.Packet, default).Wait();

		// Apply damage to target
		DamageResult result;
		if (target is PlayerClient playerTarget)
		{
			result = playerTarget.TakeDamage((int)damage.Damage);
		}
		else
		{
			result = target.UpdateHealth((int)(damage.Damage * -1));
			if (result.TargetKilled)
			{
				SendDeathPackets(target, client);
			}
		}

		// Handle experience reward and level up
		if (result.TargetKilled && result.ExpReward > 0 && client is IEntity pcEntity)
		{
			HandleExpReward(pcEntity, result.ExpReward, client);
		}
	}

	private static void ApplySkillEffects(IEntity caster, IEntity target, Models.SkillXML.XmlSkill skill, int skillLevel, IPlayerClient client)
	{
		if (BuffService.IsBuffSkill(skill.Type))
		{
			var buffResults = LimeServer.BuffService.ApplyBuffsFromSkill(skill, skillLevel, caster, caster);
			SendBuffPackets(buffResults, client);
		}

		if (BuffService.IsDebuffSkill(skill.Type))
		{
			var debuffResults = LimeServer.BuffService.ApplyBuffsFromSkill(skill, skillLevel, target, caster);
			SendBuffPackets(debuffResults, client);
		}
	}

	private static void SendBuffPackets(List<BuffResult> results, IPlayerClient client)
	{
		foreach (var result in results)
		{
			if (result.Success && result.Packet != null)
			{
				client.Send(result.Packet, default).Wait();
				client.SendGlobalPacket(result.Packet, default).Wait();
			}
		}
	}

	private static void SendDeathPackets(IEntity target, IPlayerClient client)
	{
		SC_STOP sc_stop = new()
		{
			objInstID = target.GetId(),
			pos = target.GetPosition(),
			dir = target.GetDirection(),
			tick = LimeServer.GetCurrentTick(),
			stopType = 0
		};

		using PacketWriter pw_stop = new();
		pw_stop.Write(sc_stop);
		client.Send(pw_stop.ToPacket(), default).Wait();
		client.SendGlobalPacket(pw_stop.ToPacket(), default).Wait();

		using (PacketWriter pw = new())
		{
			SC_DEAD dead = new()
			{
				objInstID = target.GetId()
			};

			pw.Write(dead);
			client.Send(pw.ToPacket(), default).Wait();
			client.SendGlobalPacket(pw.ToPacket(), default).Wait();
		}
	}

	private static void HandleExpReward(IEntity pcEntity, int expReward, IPlayerClient client)
	{
		var levelUp = pcEntity.AddExp((ulong)expReward);
		var currentStatus = pcEntity.GetEntityStatus();

		using (PacketWriter pw = new())
		{
			SC_GOT_COMBAT_JOB_EXP addExp = new()
			{
				exp = (uint)currentStatus.Exp,
				addExp = (uint)expReward
			};
			pw.Write(addExp);
			client.Send(pw.ToPacket(), default).Wait();
		}

		if (levelUp)
		{
			using (PacketWriter pw = new())
			{
				SC_PC_COMBAT_JOB_LEVEL_UP lvUp = new()
				{
					objInstID = pcEntity.GetId(),
					lv = currentStatus.Lv,
					exp = (uint)currentStatus.Exp,
					newStr = 5,
					newInl = 5,
					newAgi = 5,
					newDex = 5,
					newSpi = 5,
					newVit = 5
				};
				pw.Write(lvUp);
				client.Send(pw.ToPacket(), default).Wait();
				client.SendGlobalPacket(pw.ToPacket(), default).Wait();
			}
		}
	}
}
