/// <summary>
/// Handles CS_ESCAPE packet - player uses escape to return to safe location.
/// </summary>
/// <remarks>
/// Triggered by: Player using escape command/item (return to town)
/// Response packets: SC_WARP, SC_RESURRECTED (if dead)
/// Note: Warps the player to a predetermined safe location (town spawn point).
/// If the player is dead, this also resurrects them with reduced HP.
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
	// Spawn points loaded from zone data when available
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
		bool wasDead = pc.IsDead();

		Logger.Log($"[ESCAPE] {playerName} using escape to return to safe location (wasDead: {wasDead})", LogLevel.Debug);

		// Get spawn point for current zone
		FPOS spawnPoint = GetSpawnPointForZone(pc.GetZone());

		// If player was dead, resurrect them first
		if (wasDead)
		{
			// Resurrect with 10% HP (less than in-place resurrection)
			var status = pc.GetStatus();
			uint resurrectHP = Math.Max(1, status.mhp / 10);

			pc.Resurrect(resurrectHP);

			// Send resurrection packet
			SC_RESURRECTED resPacket = new()
			{
				objInstID = pc.GetObjInstID(),
				hp = resurrectHP
			};

			using PacketWriter resPw = new();
			resPw.Write(resPacket);
			var resBytes = resPw.ToPacket();
			pc.Send(resBytes, default).Wait();
			pc.SendGlobalPacket(resBytes, default).Wait();

			Logger.Log($"[ESCAPE] {playerName} resurrected at town with {resurrectHP} HP", LogLevel.Information);
		}

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

		Logger.Log($"[ESCAPE] {playerName} escaped to spawn point ({spawnPoint.x}, {spawnPoint.y}, {spawnPoint.z})", LogLevel.Debug);
	}

	/// <summary>
	/// Gets the spawn point for a given zone.
	/// </summary>
	/// <param name="zoneId">The zone ID</param>
	/// <returns>Spawn point position</returns>
	private static FPOS GetSpawnPointForZone(uint zoneId)
	{
		// Zone-specific spawn points to be loaded from ZoneInfo.xml
		// Returns default spawn point for now
		return DefaultSpawnPoint;
	}
}
