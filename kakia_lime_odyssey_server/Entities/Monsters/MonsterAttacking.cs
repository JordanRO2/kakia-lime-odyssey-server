using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Combat;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Entities.Monsters;

public partial class Monster : INpc
{
	private bool _startedAttacking = false;

	private void AttackPlayer(uint serverTick, ReadOnlySpan<PlayerClient> playerClients)
	{
		if (CurrentTarget == null)
		{
			ReturnHome(serverTick);
			_currentState = MobState.Roaming;
			return;
		}

		var currentPosition = GetMobCurrentPosition(serverTick);
		var distanceToPlayer = CurrentTarget.GetPosition().CalculateDistance(currentPosition);
		if (distanceToPlayer >= 10)
		{
			_currentState = MobState.Chasing;
			return;
		}

		using (PacketWriter pw = new())
		{
			SC_START_COMBATING sc_start_combat = new()
			{
				instID = Id
			};
			pw.Write(sc_start_combat);
			SendToNearbyPlayers(pw.ToPacket(), playerClients);
		}

		if ( !_startedAttacking || (_startedAttacking && (serverTick - _actionStartTick > 2000)))
		{
			_startedAttacking = true;
			_actionStartTick = serverTick;

			using (PacketWriter pw = new())
			{
				SC_DO_ACTION actionSkill = new()
				{
					objInstID = Id,
					type = 2140501,
					loopCount = 0
				};
				pw.Write(actionSkill);
				SendToNearbyPlayers(pw.ToPacket(), playerClients);
			}

			var damage = DamageHandler.DealWeaponHitDamage(this, CurrentTarget);
			if (damage.Packet is null)
				return;

			// Send bullet packet first for ranged attacks
			if (damage.IsRanged && damage.BulletPacket != null)
			{
				SendToNearbyPlayers(damage.BulletPacket, playerClients);
			}

			SendToNearbyPlayers(damage.Packet, playerClients);
			var result = (CurrentTarget as PlayerClient)!.TakeDamage((int)damage.Damage);

			if (result.TargetKilled)
			{
				CurrentTarget = null;
				ReturnHome(serverTick);
			}
		}
	}
}
