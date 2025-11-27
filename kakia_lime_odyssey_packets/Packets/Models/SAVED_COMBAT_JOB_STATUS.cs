using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Combat job status structure - stores combat class information and stats.
/// </summary>
/// <remarks>
/// IDA Verified: Pending
/// Size: 22 bytes (with Pack=4)
/// Used in: SAVED_STATUS_PC_KR
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SAVED_COMBAT_JOB_STATUS
{
	/// <summary>Combat job type ID (0 = none, 1+ = specific class)</summary>
	public byte typeID;
	/// <summary>Combat job level</summary>
	public byte lv;
	/// <summary>Combat job experience points</summary>
	public uint exp;
	/// <summary>Available stat points to distribute</summary>
	public ushort statusPoint;
	/// <summary>Strength stat</summary>
	public ushort strength;
	/// <summary>Intelligence stat</summary>
	public ushort intelligence;
	/// <summary>Dexterity stat</summary>
	public ushort dexterity;
	/// <summary>Agility stat</summary>
	public ushort agility;
	/// <summary>Vitality stat</summary>
	public ushort vitality;
	/// <summary>Spirit stat</summary>
	public ushort spirit;
	/// <summary>Lucky stat</summary>
	public ushort lucky;
}

public class ModCombatJobStatus
{
	public byte typeID;
	public byte lv;
	public uint exp;
	public ushort statusPoint;
	public ushort strength;
	public ushort intelligence;
	public ushort dexterity;
	public ushort agility;
	public ushort vitality;
	public ushort spirit;
	public ushort lucky;

	public ModCombatJobStatus(SAVED_COMBAT_JOB_STATUS status)
	{
		typeID = status.typeID;
		lv = status.lv;
		exp = status.exp;
		statusPoint = status.statusPoint;
		strength = status.strength;
		intelligence = status.intelligence;
		dexterity = status.dexterity;
		agility = status.agility;
		vitality = status.vitality;
		spirit = status.spirit;
		lucky = status.lucky;
	}

	public SAVED_COMBAT_JOB_STATUS AsStruct()
	{
		return new SAVED_COMBAT_JOB_STATUS()
		{
			typeID = typeID,
			lv = lv,
			exp = exp,
			statusPoint = statusPoint,
			strength = strength,
			intelligence = intelligence,
			dexterity = dexterity,
			agility = agility,
			vitality = vitality,
			spirit = spirit,
			lucky = lucky
		};
	}
}