using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Entities.Monsters;
using kakia_lime_odyssey_server.Entities.Npcs;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.World;

/// <summary>
/// Service for world management including zones, entity spawning, and spatial queries.
/// </summary>
public class WorldService : IWorldService
{
	private readonly Dictionary<uint, List<Npc>> _npcs;
	private readonly Dictionary<uint, List<Monster>> _mobs;
	private readonly DateTime _startTime;
	private uint _currentObjInstId = 200000;
	private readonly object _idLock = new();

	public WorldService(Dictionary<uint, List<Npc>> npcs, Dictionary<uint, List<Monster>> mobs, DateTime startTime)
	{
		_npcs = npcs;
		_mobs = mobs;
		_startTime = startTime;
	}

	/// <inheritdoc/>
	public IEnumerable<Npc> GetNpcsInZone(uint zoneId)
	{
		if (_npcs.TryGetValue(zoneId, out var npcs))
			return npcs;
		return Enumerable.Empty<Npc>();
	}

	/// <inheritdoc/>
	public IEnumerable<Monster> GetMonstersInZone(uint zoneId)
	{
		if (_mobs.TryGetValue(zoneId, out var mobs))
			return mobs;
		return Enumerable.Empty<Monster>();
	}

	/// <inheritdoc/>
	public bool AddEntity(INpc entity)
	{
		switch (entity.GetNpcType())
		{
			case NpcType.Mob when entity is Monster monster:
				if (!_mobs.ContainsKey(monster.Zone))
					_mobs[monster.Zone] = new List<Monster>();
				_mobs[monster.Zone].Add(monster);
				return true;

			case NpcType.Npc when entity is Npc npc:
				if (!_npcs.ContainsKey(npc.ZoneId))
					_npcs[npc.ZoneId] = new List<Npc>();
				_npcs[npc.ZoneId].Add(npc);
				return true;

			default:
				return false;
		}
	}

	/// <inheritdoc/>
	public bool RemoveEntity(long entityId)
	{
		foreach (var zone in _mobs.Values)
		{
			var mob = zone.FirstOrDefault(m => m.Id == entityId);
			if (mob != null)
			{
				zone.Remove(mob);
				return true;
			}
		}

		foreach (var zone in _npcs.Values)
		{
			var npc = zone.FirstOrDefault(n => n.Id == entityId);
			if (npc != null)
			{
				zone.Remove(npc);
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	public IEntity? GetEntity(long entityId)
	{
		// Check players first
		var player = LimeServer.PlayerClients.FirstOrDefault(pc => pc.GetObjInstID() == entityId);
		if (player != null)
			return player;

		// Check mobs
		var mob = _mobs.SelectMany(pair => pair.Value)
			.FirstOrDefault(m => m.Id == entityId);
		if (mob != null)
			return mob;

		// NPCs don't implement IEntity currently
		return null;
	}

	/// <inheritdoc/>
	public IEnumerable<IEntity> GetEntitiesInRange(uint zoneId, FPOS position, float radius)
	{
		var entities = new List<IEntity>();
		float radiusSq = radius * radius;

		// Add players in range
		foreach (var player in LimeServer.PlayerClients)
		{
			if (player.GetZone() != zoneId)
				continue;

			var playerPos = player.GetPosition();
			if (DistanceSquared(position, playerPos) <= radiusSq)
				entities.Add(player);
		}

		// Add monsters in range
		if (_mobs.TryGetValue(zoneId, out var mobs))
		{
			foreach (var mob in mobs)
			{
				if (DistanceSquared(position, mob.Position) <= radiusSq)
					entities.Add(mob);
			}
		}

		return entities;
	}

	/// <inheritdoc/>
	public uint GetCurrentTick()
	{
		return (uint)DateTime.Now.Subtract(_startTime).TotalMilliseconds;
	}

	/// <inheritdoc/>
	public uint GenerateUniqueId()
	{
		lock (_idLock)
		{
			return ++_currentObjInstId;
		}
	}

	private static float DistanceSquared(FPOS a, FPOS b)
	{
		float dx = a.x - b.x;
		float dy = a.y - b.y;
		float dz = a.z - b.z;
		return dx * dx + dy * dy + dz * dz;
	}
}
