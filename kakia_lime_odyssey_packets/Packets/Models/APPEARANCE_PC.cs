using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Character appearance data for the character selection screen (Korean CBT3 version).
/// Contains character name, race, gender, job, visual customization, and equipped items.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: APPEARANCE_PC
/// Size: 152 bytes
/// Memory Layout (IDA):
/// - 0x00-0x19: char[26] name (26 bytes) - Character name
/// - 0x1C: unsigned int raceTypeID (4 bytes)
/// - 0x20: char lifeJobTypeID (1 byte)
/// - 0x21: char combatJobTypeID (1 byte)
/// - 0x22: bool genderType (1 byte)
/// - 0x23: char headType (1 byte)
/// - 0x24: char hairType (1 byte)
/// - 0x25: char eyeType (1 byte)
/// - 0x26: char earType (1 byte)
/// - 0x27: unsigned __int8 playingJobClass (1 byte)
/// - 0x28: __int16 underwearType (2 bytes)
/// - 0x2C: int[20] equiped (80 bytes) - Array of equipped item IDs
/// - 0x7C: char familyNameType (1 byte)
/// - 0x80: unsigned int action (4 bytes)
/// - 0x84: unsigned int actionStartTick (4 bytes)
/// - 0x88: float scale (4 bytes)
/// - 0x8C: float transparent (4 bytes)
/// - 0x90: bool showHelm (1 byte)
/// - 0x91: COLOR color (3 bytes) - RGB color
/// - 0x94: unsigned __int8 skinColorType (1 byte)
/// - 0x95: unsigned __int8 hairColorType (1 byte)
/// - 0x96: unsigned __int8 eyeColorType (1 byte)
/// - 0x97: unsigned __int8 eyeBrowColorType (1 byte)
/// Total: 152 bytes (0x98)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_PC_KR
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;
	public uint raceTypeID;
	public byte lifeJobTypeID;
	public byte combatJobTypeID;
	[MarshalAs(UnmanagedType.U1)]
	public bool genderType;
	public byte headType;
	public byte hairType;
	public byte eyeType;
	public byte earType;
	public byte playingJobClass;
	public short underwearType;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	public int[] equiped;
	public byte familyNameType;
	public uint action;
	public uint actionStartTick;
	public float scale;
	public float transparent;
	[MarshalAs(UnmanagedType.U1)]
	public bool showHelm;
	public COLOR color;
	public byte skinColorType;
	public byte hairColorType;
	public byte eyeColorType;
	public byte eyeBrowColorType;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_PC
{
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;
	public byte raceTypeID;
	public byte lifeJobTypeID;
	public byte combatJobTypeID;
	[MarshalAs(UnmanagedType.U1)]
	public bool genderType;
	public byte headType;
	public byte hairType;
	public byte eyeType;
	public byte earType;
	public byte playingJobClass;
	public byte unk;
	public short underwearType;
	public int unk1;
	public byte unk2;
	public EQUIPPED equiped;
	public byte familyNameType;
	public uint action;
	public uint actionStartTick;
	public float scale;
	public float transparent;
	[MarshalAs(UnmanagedType.U1)]
	public bool showHelm;
	public COLOR color;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] unk3;

	public byte skinColorType;
	public byte hairColorType;
	public byte eyeColorType;
	public byte eyeBrowType;
	public byte eyeBrowColorType;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	public byte[] unk4;
}


public class ModAppearance
{
	public string name;
	public uint raceTypeID;
	public byte lifeJobTypeID;
	public byte combatJobTypeID;
	public bool genderType;
	public byte headType;
	public byte hairType;
	public byte eyeType;
	public byte earType;
	public byte playingJobClass;
	public short underwearType;
	public ModEquipped equiped { get; set; }
	public byte familyNameType;
	public uint action;
	public uint actionStartTick;
	public float scale;
	public float transparent;
	public bool showHelm;
	public COLOR color;
	public byte skinColorType;
	public byte hairColorType;
	public byte eyeColorType;
	public byte eyeBrowColorType;


	public ModAppearance(APPEARANCE_PC_KR other)
	{
		this.name = System.Text.Encoding.ASCII.GetString(other.name).TrimEnd('\0');
		this.raceTypeID = other.raceTypeID;
		this.lifeJobTypeID = other.lifeJobTypeID;
		this.combatJobTypeID = other.combatJobTypeID;
		this.genderType = other.genderType;
		this.headType = other.headType;
		this.hairType = other.hairType;
		this.eyeType = other.eyeType;
		this.earType = other.earType;
		this.playingJobClass = other.playingJobClass;
		this.underwearType = other.underwearType;
		// Convert int[20] array to ModEquipped
		this.equiped = new ModEquipped(new EQUIPPED
		{
			NONE = other.equiped[0],
			MAIN_EQUIP = other.equiped[1],
			SUB_EQUIP = other.equiped[2],
			RANGE_MAIN_EQUIP = other.equiped[3],
			SPENDING = other.equiped[4],
			HEAD = other.equiped[5],
			FOREHEAD = other.equiped[6],
			EYE = other.equiped[7],
			MOUTH = other.equiped[8],
			NECK = other.equiped[9],
			SHOULDER = other.equiped[10],
			UPPER_BODY = other.equiped[11],
			HAND = other.equiped[12],
			WAIST = other.equiped[13],
			LOWER_BODY = other.equiped[14],
			FOOT = other.equiped[15],
			RELIC = other.equiped[16],
			RING_1 = other.equiped[17],
			RING_2 = other.equiped[18],
			ACCESSORY_1 = other.equiped[19]
		});
		this.familyNameType = other.familyNameType;
		this.action = other.action;
		this.actionStartTick = other.actionStartTick;
		this.scale = other.scale;
		this.transparent = other.transparent;
		this.showHelm = other.showHelm;
		this.color = other.color;
		this.skinColorType = other.skinColorType;
		this.hairColorType = other.hairColorType;
		this.eyeColorType = other.eyeColorType;
		this.eyeBrowColorType = other.eyeBrowColorType;
	}

	public APPEARANCE_PC_KR AsStruct()
	{
		var nameBytes = new byte[26];
		var nameData = System.Text.Encoding.ASCII.GetBytes(name);
		Array.Copy(nameData, nameBytes, Math.Min(nameData.Length, 25));

		return new APPEARANCE_PC_KR()
		{
			name = nameBytes,
			raceTypeID = raceTypeID,
			lifeJobTypeID = lifeJobTypeID,
			combatJobTypeID = combatJobTypeID,
			genderType = genderType,
			headType = headType,
			hairType = hairType,
			eyeType = eyeType,
			earType = earType,
			playingJobClass = playingJobClass,
			underwearType = underwearType,
			equiped = equiped?.AsArray() ?? new int[20],
			familyNameType = familyNameType,
			action = action,
			actionStartTick = actionStartTick,
			scale = scale,
			transparent = transparent,
			showHelm = showHelm,
			color = color,
			skinColorType = skinColorType,
			hairColorType = hairColorType,
			eyeColorType = eyeColorType,
			eyeBrowColorType = eyeBrowColorType
		};
	}
}