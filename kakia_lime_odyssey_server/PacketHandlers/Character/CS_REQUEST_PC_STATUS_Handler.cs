/// <summary>
/// Handles CS_REQUEST_PC_STATUS packet - request another player's status.
/// </summary>
/// <remarks>
/// Triggered by: Player inspecting another player
/// Response packets: SC_PC_STATUS or similar
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Character;

[PacketHandlerAttr(PacketType.CS_REQUEST_PC_STATUS)]
class CS_REQUEST_PC_STATUS_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_REQUEST_PC_STATUS>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHAR] {playerName} requesting status of player {packet.objInstID}", LogLevel.Debug);

		// Find target player by objInstID
		var targetPlayer = LimeServer.PlayerClients.FirstOrDefault(p => p.GetObjInstID() == packet.objInstID);
		if (targetPlayer == null)
		{
			Logger.Log($"[CHAR] Target player {packet.objInstID} not found", LogLevel.Debug);
			return;
		}

		// Get target player's character info
		var targetChar = targetPlayer.GetCurrentCharacter();
		if (targetChar == null) return;

		// Build STATUS_PC from target player's data
		var entityStatus = targetPlayer.GetEntityStatus();
		var commonStatus = targetPlayer.GetStatus();

		var statusPc = new STATUS_PC
		{
			commonStatus = commonStatus,
			lp = entityStatus.Lp,
			mlp = entityStatus.MaxLp,
			streamPoint = targetChar.status.streamPoint,
			meleeHitRate = entityStatus.MeleeAttack.Hit,
			dodge = entityStatus.Dodge,
			meleeAtk = entityStatus.MeleeAttack.Atk,
			meleeDefense = entityStatus.MeleeAttack.Def,
			spellAtk = entityStatus.SpellAttack.Atk,
			spellDefense = entityStatus.SpellAttack.Def,
			parry = entityStatus.Parry,
			block = entityStatus.Block,
			resist = entityStatus.Resist,
			criticalRate = entityStatus.MeleeAttack.CritRate,
			hitSpeedRatio = entityStatus.HitSpeedRatio,
			lifeJob = new LIFE_JOB_STATUS_
			{
				lv = targetChar.status.lifeJob.lv,
				exp = targetChar.status.lifeJob.exp,
				statusPoint = targetChar.status.lifeJob.statusPoint,
				idea = targetChar.status.lifeJob.idea,
				mind = targetChar.status.lifeJob.mind,
				craft = targetChar.status.lifeJob.craft,
				sense = targetChar.status.lifeJob.sense
			},
			combatJob = new COMBAT_JOB_STATUS_
			{
				lv = targetChar.status.combatJob.lv,
				exp = targetChar.status.combatJob.exp,
				strength = targetChar.status.combatJob.strength,
				intelligence = targetChar.status.combatJob.intelligence,
				dexterity = targetChar.status.combatJob.dexterity,
				agility = targetChar.status.combatJob.agility,
				vitality = targetChar.status.combatJob.vitality,
				spirit = targetChar.status.combatJob.spirit,
				lucky = targetChar.status.combatJob.lucky
			},
			velocities = new VELOCITIES
			{
				ratio = 1.0f,
				run = 5.0f,
				runAccel = 10.0f,
				walk = 2.5f,
				walkAccel = 5.0f,
				backwalk = 1.5f,
				backwalkAccel = 3.0f,
				swim = 3.0f,
				swimAccel = 6.0f,
				backSwim = 2.0f,
				backSwimAccel = 4.0f
			},
			collectSucessRate = entityStatus.LifeJobStats?.CollectSuccessRate ?? 0,
			collectionIncreaseRate = entityStatus.LifeJobStats?.CollectionIncreaseRate ?? 0,
			makeTimeDecreaseAmount = entityStatus.LifeJobStats?.MakeTimeDecrease ?? 0
		};

		// Send target player's status info
		using PacketWriter pw = new();
		pw.Write(new SC_PC_STATUS
		{
			objInstID = packet.objInstID,
			status = statusPc
		});
		pc.Send(pw.ToPacket(), default).Wait();
	}
}
