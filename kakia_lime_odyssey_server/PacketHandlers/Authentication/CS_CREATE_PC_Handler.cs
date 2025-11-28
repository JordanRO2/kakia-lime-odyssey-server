using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Authentication;

[PacketHandlerAttr(PacketType.CS_CREATE_PC)]
class CS_CREATE_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		Logger.LogPck(p.Payload);
		var character = PacketConverter.Extract<CS_CREATE_PC>(p.Payload);

		var nameBytes = new byte[26];
		var nameData = global::System.Text.Encoding.ASCII.GetBytes(character.name);
		global::System.Array.Copy(nameData, nameBytes, global::System.Math.Min(nameData.Length, 25));

		CLIENT_PC newCharacter = new()
		{
			status = new SAVED_STATUS_PC_KR()
			{

				hp = 120,
				mp = 120,
				lp = 120,
				streamPoint = 1,
				breath = 1,
				fatigue = 1,

				lifeJob = new SAVED_LIFE_JOB_STATUS()
				{
					lv = 1,
					exp = 1,
					statusPoint = 0,
					idea = 0,
					mind = 0,
					craft = 0,
					sense = 0
				},

				combatJob = new SAVED_COMBAT_JOB_STATUS()
				{
					lv = 1,
					exp = 1,
					strength = 1,
					intelligence = 1,
					dexterity = 1,
					agility = 1,
					vitality = 1,
					spirit = 1,
					lucky = 1
				}
			},

			appearance = new()
			{
				name = nameBytes,
				_padding1 = new byte[2],
				raceTypeID = character.raceTypeID,
				lifeJobTypeID = character.lifeJobTypeID,
				combatJobTypeID = 1,
				genderType = character.genderType,
				headType = character.headType,
				hairType = character.hairType,
				eyeType = character.eyeType,
				earType = character.earType,
				playingJobClass = 0,
				underwearType = character.underwearType,
				_padding2 = new byte[2],
				equiped = new int[20],
				familyNameType = character.familyNameType,
				_padding3 = new byte[3],
				action = 0,
				actionStartTick = 0,
				scale = 1,
				transparent = 1,
				showHelm = true,
				color = new()
				{
					r = 1,
					g = 1,
					b = 1
				},
				skinColorType = character.skinColorType,
				hairColorType = character.hairColorType,
				eyeColorType = character.eyeColorType,
				eyeBrowColorType = character.eyeBrowColorType
			}
		};

		var db = DatabaseFactory.Instance;
		db.StoreAppearance(client.GetAccountId(), character.name, newCharacter.appearance);
		db.StoreSavedStatusPC(client.GetAccountId(), character.name, newCharacter.status);

		// Set starting zone and position (Muris Village - Zone 1)
		var startingZoneId = 1u;
		var spawnPoint = LimeServer.ZoneTransferService.GetSpawnPoint(startingZoneId);
		var startingPosition = new WorldPosition
		{
			ZoneID = startingZoneId,
			Position = spawnPoint,
			Direction = new FPOS { x = 0.9f, y = 0, z = 0 }
		};
		db.SaveWorldPosition(client.GetAccountId(), character.name, startingPosition);

		Logger.Log($"[CREATE_PC] Created character '{character.name}' at Zone {startingZoneId} ({spawnPoint.x:F0}, {spawnPoint.y:F0}, {spawnPoint.z:F0})", LogLevel.Information);

		SC_CREATED_PC sc_created_pc = new()
		{
			client_pc = newCharacter
		};

		using PacketWriter pw = new();
		pw.Write(sc_created_pc);

		client.Send(pw.ToSizedPacket(), default);
	}
}
