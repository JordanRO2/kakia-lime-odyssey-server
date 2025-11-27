using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Entities.Monsters;
using kakia_lime_odyssey_server.Entities.Npcs;
using kakia_lime_odyssey_server.Interfaces;

namespace kakia_lime_odyssey_server.Services.World;

/// <summary>
/// Interface for world management services including zones, spawns, and entity lookups.
/// </summary>
public interface IWorldService
{
	/// <summary>
	/// Gets all NPCs in a specific zone.
	/// </summary>
	IEnumerable<Npc> GetNpcsInZone(uint zoneId);

	/// <summary>
	/// Gets all monsters in a specific zone.
	/// </summary>
	IEnumerable<Monster> GetMonstersInZone(uint zoneId);

	/// <summary>
	/// Adds an NPC or monster to the world.
	/// </summary>
	bool AddEntity(INpc entity);

	/// <summary>
	/// Removes an entity from the world.
	/// </summary>
	bool RemoveEntity(long entityId);

	/// <summary>
	/// Finds an entity by its ID.
	/// </summary>
	IEntity? GetEntity(long entityId);

	/// <summary>
	/// Gets entities within a radius of a position.
	/// </summary>
	IEnumerable<IEntity> GetEntitiesInRange(uint zoneId, FPOS position, float radius);

	/// <summary>
	/// Gets the current server tick.
	/// </summary>
	uint GetCurrentTick();

	/// <summary>
	/// Generates a unique object instance ID.
	/// </summary>
	uint GenerateUniqueId();
}
