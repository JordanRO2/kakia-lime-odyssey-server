using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Combat;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Combat;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_USE_SKILL_ACTION_TARGET)]
class CS_USE_SKILL_ACTION_TARGET_Handler : PacketHandler
{
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
					$"attempted to use skill {useSkill.typeID} while on cooldown",
					LogLevel.Warning
				);
				return; // Reject skill usage - cooldown not ready
			}
		}

		if (!LimeServer.TryGetEntity(client.GetCurrentTarget(), out IEntity? target)) return;
		if (target is null) return;

		if (target.GetEntityStatus().BasicStatus.Hp == 0)
		{
			// Target dead
			return;
		}

		// Go with skill level 1 for now, since we haven't actually implemented skills for real
		var skillLv1 = skill.Subject.SubjectLists.FirstOrDefault();
		uint castTime = (uint)(skillLv1 != null ? skillLv1.CastingTime : skill.CastingTime);

		if (castTime > 0)
		{
			SC_START_CASTING_SKILL_OBJ castSkill = new()
			{
				fromInstID = client.GetObjInstID(),
				targetInstID = client.GetCurrentTarget(),
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

		SC_USE_SKILL_OBJ_RESULT_LIST actionSkill = new()
		{
			fromInstID = client.GetObjInstID(),
			toInstID = client.GetCurrentTarget(),
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

		var damage = DamageHandler.DealWeaponHitDamage((client as IEntity)!, target);
		if (damage.Packet is null) return;

		// Send bullet packet first for ranged attacks
		if (damage.IsRanged && damage.BulletPacket != null)
		{
			client.Send(damage.BulletPacket, default).Wait();
			client.SendGlobalPacket(damage.BulletPacket, default).Wait();
		}

		client.Send(damage.Packet, default).Wait();
		client.SendGlobalPacket(damage.Packet, default).Wait();

		// Check if target is a player or monster and handle accordingly
		DamageResult result;
		if (target is PlayerClient playerTarget)
		{
			// For player targets, use TakeDamage which handles death notification
			result = playerTarget.TakeDamage((int)damage.Damage);
		}
		else
		{
			// For monsters and other entities, use the old UpdateHealth
			result = target.UpdateHealth((int)(damage.Damage * -1));
			if (result.TargetKilled)
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
		}

		// Handle experience reward and level up
		if (result.TargetKilled && result.ExpReward > 0 && client is IEntity pcEntity)
		{
			var levelUp = pcEntity.AddExp((ulong)result.ExpReward);
			var currentStatus = pcEntity.GetEntityStatus();

			using (PacketWriter pw = new())
			{
				SC_GOT_COMBAT_JOB_EXP addExp = new()
				{
					exp = (uint)currentStatus.Exp,
					addExp = (uint)result.ExpReward
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

	/// <summary>
	/// Applies buffs or debuffs from a skill to the appropriate target.
	/// Buff skills apply to the caster, debuff skills apply to the target.
	/// </summary>
	private static void ApplySkillEffects(IEntity caster, IEntity target, Models.SkillXML.XmlSkill skill, int skillLevel, IPlayerClient client)
	{
		// Check if this is a buff skill (self-buff when targeting enemy)
		if (BuffService.IsBuffSkill(skill.Type))
		{
			var buffResults = LimeServer.BuffService.ApplyBuffsFromSkill(skill, skillLevel, caster, caster);
			SendBuffPackets(buffResults, client, "self");
		}

		// Check if this is a debuff skill (apply to target)
		if (BuffService.IsDebuffSkill(skill.Type))
		{
			var debuffResults = LimeServer.BuffService.ApplyBuffsFromSkill(skill, skillLevel, target, caster);
			SendBuffPackets(debuffResults, client, "target");
		}
	}

	/// <summary>
	/// Sends buff application packets to the caster and nearby players.
	/// </summary>
	private static void SendBuffPackets(List<BuffResult> results, IPlayerClient client, string targetType)
	{
		foreach (var result in results)
		{
			if (result.Success && result.Packet != null)
			{
				client.Send(result.Packet, default).Wait();
				client.SendGlobalPacket(result.Packet, default).Wait();

				Logger.Log(
					$"[BUFF] Applied buff/debuff (InstID:{result.BuffInstId}) to {targetType}",
					LogLevel.Debug
				);
			}
		}
	}
}
