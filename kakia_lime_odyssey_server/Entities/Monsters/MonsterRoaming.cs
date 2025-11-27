using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Entities.Monsters;

public partial class Monster : INpc
{
	private void Roam(uint serverTick, ReadOnlySpan<PlayerClient> playerClients)
	{
		if (IsMoving)
		{
			MoveTowardsDestination(serverTick, playerClients);
			return;
		}

		// Wait until the next roam decision time (serverTick is milliseconds since server start)
		if (serverTick < _nextRoamDecisionTick)
			return;

		// Random wait until next roam decision (500ms - 2500ms)
		int waitMs = 500 + _rng.Next(0, 2000);
		_nextRoamDecisionTick = serverTick + (uint)waitMs;

		// Pick a new destination within radius around the original spawn
		FPOS newDestination;
		int tries = 0;
		do
		{
			newDestination = _originalPosition.GetRandomPositionWithinRadius(25);
			if (++tries > 8) // avoid infinite loops
				break;
		}
		// Ensure the new destination isn't the exact current position
		while (newDestination.Compare(Position));

		SetNewDestination(newDestination, serverTick);
	}
}
