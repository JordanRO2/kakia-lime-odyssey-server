/// <summary>
/// Handles CS_ESCAPE packet - player uses escape to return to safe location.
/// </summary>
/// <remarks>
/// Triggered by: Player using escape command/item (return to town)
/// Response packets: SC_WARP
/// Note: Warps the player to a predetermined safe location (town spawn point).
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

[PacketHandlerAttr(PacketType.CS_ESCAPE)]
class CS_ESCAPE_Handler : PacketHandler
{
	// Default safe spawn location (town center)
	// TODO: Load spawn points from zone data
	private static readonly FPOS DefaultSpawnPoint = new()
	{
		x = 7490.0f,
		y = 175.0f,
		z = 8960.0f
	};

	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[MOVEMENT] {playerName} using escape to return to safe location", LogLevel.Debug);

		// Get spawn point for current zone
		// TODO: Get actual spawn point based on zone
		FPOS spawnPoint = GetSpawnPointForZone(pc.GetZone());

		// Update player position
		pc.UpdatePosition(spawnPoint);

		// Send warp packet to player and broadcast to others
		SC_WARP warp = new()
		{
			objInstID = pc.GetObjInstID(),
			pos = spawnPoint,
			dir = pc.GetDirection()
		};

		using PacketWriter pw = new();
		pw.Write(warp);
		pc.Send(pw.ToPacket(), default).Wait();
		pc.SendGlobalPacket(pw.ToPacket(), default).Wait();

		Logger.Log($"[MOVEMENT] {playerName} escaped to spawn point ({spawnPoint.x}, {spawnPoint.y}, {spawnPoint.z})", LogLevel.Debug);
	}

	/// <summary>
	/// Gets the spawn point for a given zone.
	/// </summary>
	/// <param name="zoneId">The zone ID</param>
	/// <returns>Spawn point position</returns>
	private static FPOS GetSpawnPointForZone(uint zoneId)
	{
		// TODO: Implement zone-specific spawn points from zone data
		// For now, return default spawn point
		return DefaultSpawnPoint;
	}
}
