/// <summary>
/// Handles CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT packet - player distributes life job stat points.
/// </summary>
/// <remarks>
/// Triggered by: Player allocating stat points from life job level up
/// Response packets: SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
/// Database: characters (update stats)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Character;

[PacketHandlerAttr(PacketType.CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT)]
class CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var distribute = PacketConverter.Extract<CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Calculate total points being distributed
		int totalPointsUsed = distribute.supplyedIdea + distribute.supplyedSense +
		                      distribute.supplyedMind + distribute.supplyedCraft;

		Logger.Log($"[STATS] {playerName} distributing {totalPointsUsed} life job stat points", LogLevel.Debug);

		// Get current character
		var character = pc.GetCurrentCharacter();
		if (character == null) return;

		// Validate player has enough stat points available
		if (character.status.lifeJob.statusPoint < totalPointsUsed)
		{
			Logger.Log($"[STATS] {playerName} doesn't have enough life stat points ({character.status.lifeJob.statusPoint} < {totalPointsUsed})", LogLevel.Warning);
			return;
		}

		// Deduct used points
		character.status.lifeJob.statusPoint -= (ushort)totalPointsUsed;

		// Apply stat increases to life job stats
		character.status.lifeJob.idea += distribute.supplyedIdea;
		character.status.lifeJob.sense += distribute.supplyedSense;
		character.status.lifeJob.mind += distribute.supplyedMind;
		character.status.lifeJob.craft += distribute.supplyedCraft;

		// Get new values
		ushort newIdea = character.status.lifeJob.idea;
		ushort newSense = character.status.lifeJob.sense;
		ushort newMind = character.status.lifeJob.mind;
		ushort newCraft = character.status.lifeJob.craft;

		ushort remainingPoints = character.status.lifeJob.statusPoint;

		// Send confirmation
		SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT response = new()
		{
			idea = newIdea,
			sense = newSense,
			mind = newMind,
			craft = newCraft,
			point = remainingPoints
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();

		Logger.Log($"[STATS] {playerName} life stats updated - IDEA:{newIdea} SENSE:{newSense} MIND:{newMind} CRAFT:{newCraft}", LogLevel.Information);
	}
}
