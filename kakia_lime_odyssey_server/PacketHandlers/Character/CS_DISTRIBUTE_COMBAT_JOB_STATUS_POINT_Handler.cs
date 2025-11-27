/// <summary>
/// Handles CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT packet - player distributes combat stat points.
/// </summary>
/// <remarks>
/// Triggered by: Player allocating stat points from combat job level up
/// Response packets: SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
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

[PacketHandlerAttr(PacketType.CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT)]
class CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var distribute = PacketConverter.Extract<CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Calculate total points being distributed
		int totalPointsUsed = distribute.supplyedSTR + distribute.supplyedINL +
		                      distribute.supplyedDEX + distribute.supplyedAGI +
		                      distribute.supplyedVIT + distribute.supplyedSPI +
		                      distribute.supplyedLUK;

		Logger.Log($"[STATS] {playerName} distributing {totalPointsUsed} combat stat points", LogLevel.Debug);

		// Get current character
		var character = pc.GetCurrentCharacter();
		if (character == null) return;

		// TODO: Validate player has enough stat points available
		// For now, just apply the distribution

		// Apply stat increases to combat job stats
		character.status.combatJob.strength += distribute.supplyedSTR;
		character.status.combatJob.intelligence += distribute.supplyedINL;
		character.status.combatJob.dexterity += distribute.supplyedDEX;
		character.status.combatJob.agility += distribute.supplyedAGI;
		character.status.combatJob.vitality += distribute.supplyedVIT;
		character.status.combatJob.spirit += distribute.supplyedSPI;
		character.status.combatJob.lucky += distribute.supplyedLUK;

		// Get new values
		ushort newStr = character.status.combatJob.strength;
		ushort newInl = character.status.combatJob.intelligence;
		ushort newDex = character.status.combatJob.dexterity;
		ushort newAgi = character.status.combatJob.agility;
		ushort newVit = character.status.combatJob.vitality;
		ushort newSpi = character.status.combatJob.spirit;
		ushort newLuk = character.status.combatJob.lucky;

		// TODO: Deduct points from available pool and get remaining
		ushort remainingPoints = character.status.combatJob.statusPoint;

		// Send confirmation
		SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT response = new()
		{
			str = newStr,
			inl = newInl,
			dex = newDex,
			agi = newAgi,
			vit = newVit,
			spi = newSpi,
			luk = newLuk,
			point = remainingPoints
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();

		Logger.Log($"[STATS] {playerName} stats updated - STR:{newStr} INL:{newInl} DEX:{newDex} AGI:{newAgi} VIT:{newVit} SPI:{newSpi} LUK:{newLuk}", LogLevel.Information);
	}
}
