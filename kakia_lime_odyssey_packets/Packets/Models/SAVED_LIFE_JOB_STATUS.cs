using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Life job status structure - stores crafting profession information and stats.
/// </summary>
/// <remarks>
/// IDA Verified: Pending
/// Size: 18 bytes (with Pack=4)
/// Used in: SAVED_STATUS_PC_KR
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SAVED_LIFE_JOB_STATUS
{
	/// <summary>Life job type ID (0 = none, 1+ = specific profession)</summary>
	public byte typeID;
	/// <summary>Life job level</summary>
	public byte lv;
	/// <summary>Life job experience points</summary>
	public uint exp;
	/// <summary>Available stat points to distribute</summary>
	public ushort statusPoint;
	/// <summary>Idea stat (crafting creativity)</summary>
	public ushort idea;
	/// <summary>Mind stat (crafting focus)</summary>
	public ushort mind;
	/// <summary>Craft stat (crafting skill)</summary>
	public ushort craft;
	/// <summary>Sense stat (resource gathering)</summary>
	public ushort sense;
}


public class ModLifeJobStatus
{
	public byte typeID;
	public byte lv;
	public uint exp;
	public ushort statusPoint;
	public ushort idea;
	public ushort mind;
	public ushort craft;
	public ushort sense;

	public ModLifeJobStatus(SAVED_LIFE_JOB_STATUS status)
	{
		typeID = status.typeID;
		lv = status.lv;
		exp = status.exp;
		statusPoint = status.statusPoint;
		idea = status.idea;
		mind = status.mind;
		craft = status.craft;
		sense = status.sense;
	}

	public SAVED_LIFE_JOB_STATUS AsStruct()
	{
		return new SAVED_LIFE_JOB_STATUS()
		{
			typeID = typeID,
			lv = lv,
			exp = exp,
			statusPoint = statusPoint,
			idea = idea,
			mind = mind,
			craft = craft,
			sense = sense
		};
	}
}