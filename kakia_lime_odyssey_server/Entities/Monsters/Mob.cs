using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;

namespace kakia_lime_odyssey_server.Entities.Monsters;

public class Mob : INpc
{
	public int Id { get; set; }
	public COMMON_STATUS Status { get; set; }
	public uint ZoneId { get; set; }
	public FPOS Pos { get; set; }
	public FPOS Dir { get; set; }
	public APPEARANCE_MONSTER Appearance { get; set; }
	public int RaceRelationState;
	public byte StopType;

	public NpcType GetNpcType()
	{
		return NpcType.Mob;
	}

	public SC_ENTER_SIGHT_MONSTER GetEnterSight()
	{
		return new()
		{
			enter_zone = new()
			{
				objInstID = Id,
				pos = Pos,
				dir = Dir
			},
			appearance = Appearance,
			raceRelationState = RaceRelationState,
			stopType = StopType
		};
	}

	public SC_LEAVE_SIGHT_MONSTER GetLeaveSight()
	{
		return new()
		{
			leave_zone = new()
			{
				objInstID = Id
			}
		};
	}
}

public enum MobState
{
	Roaming,
	Chasing,
	Attacking
}
