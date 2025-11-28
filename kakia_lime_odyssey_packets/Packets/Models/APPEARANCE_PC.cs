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
/// <summary>
/// Character appearance data - IDA verified structure with explicit padding.
/// Total size: 152 bytes (0x98)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_PC_KR
{
	/// <summary>Character name (26 bytes) at offset 0x00</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;

	/// <summary>Padding after name (2 bytes) at offset 0x1A</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	public byte[] _padding1;

	/// <summary>Race type ID (4 bytes) at offset 0x1C</summary>
	public uint raceTypeID;

	/// <summary>Life job type ID (1 byte) at offset 0x20</summary>
	public byte lifeJobTypeID;

	/// <summary>Combat job type ID (1 byte) at offset 0x21</summary>
	public byte combatJobTypeID;

	/// <summary>Gender type (1 byte bool) at offset 0x22</summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool genderType;

	/// <summary>Head type (1 byte) at offset 0x23</summary>
	public byte headType;

	/// <summary>Hair type (1 byte) at offset 0x24</summary>
	public byte hairType;

	/// <summary>Eye type (1 byte) at offset 0x25</summary>
	public byte eyeType;

	/// <summary>Ear type (1 byte) at offset 0x26</summary>
	public byte earType;

	/// <summary>Playing job class (1 byte) at offset 0x27</summary>
	public byte playingJobClass;

	/// <summary>Underwear type (2 bytes) at offset 0x28</summary>
	public short underwearType;

	/// <summary>Padding after underwearType (2 bytes) at offset 0x2A</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	public byte[] _padding2;

	/// <summary>Equipped items array (80 bytes = 20 ints) at offset 0x2C</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	public int[] equiped;

	/// <summary>Family name type (1 byte) at offset 0x7C</summary>
	public byte familyNameType;

	/// <summary>Padding after familyNameType (3 bytes) at offset 0x7D</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	public byte[] _padding3;

	/// <summary>Action ID (4 bytes) at offset 0x80</summary>
	public uint action;

	/// <summary>Action start tick (4 bytes) at offset 0x84</summary>
	public uint actionStartTick;

	/// <summary>Character scale (4 bytes float) at offset 0x88</summary>
	public float scale;

	/// <summary>Transparency (4 bytes float) at offset 0x8C</summary>
	public float transparent;

	/// <summary>Show helm flag (1 byte bool) at offset 0x90</summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool showHelm;

	/// <summary>Character color (3 bytes RGB) at offset 0x91</summary>
	public COLOR color;

	/// <summary>Skin color type (1 byte) at offset 0x94</summary>
	public byte skinColorType;

	/// <summary>Hair color type (1 byte) at offset 0x95</summary>
	public byte hairColorType;

	/// <summary>Eye color type (1 byte) at offset 0x96</summary>
	public byte eyeColorType;

	/// <summary>Eyebrow color type (1 byte) at offset 0x97</summary>
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
			_padding1 = new byte[2],
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
			_padding2 = new byte[2],
			equiped = equiped?.AsArray() ?? new int[20],
			familyNameType = familyNameType,
			_padding3 = new byte[3],
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