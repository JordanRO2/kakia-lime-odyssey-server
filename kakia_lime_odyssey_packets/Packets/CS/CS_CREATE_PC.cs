using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server character creation packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_CREATE_PC
/// Size: 45 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[26] name (26 bytes)
/// - 0x1C: unsigned int raceTypeID (4 bytes)
/// - 0x20: bool genderType (1 byte)
/// - 0x21: unsigned char lifeJobTypeID (1 byte)
/// - 0x22-0x2C: appearance fields (11 bytes)
/// Response: SC_CREATED_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_CREATE_PC : IPacketFixed
{
	/// <summary>
	/// Character name - null-terminated string, max 26 bytes including null terminator
	/// Offset: 0x02 (after PACKET_FIX header)
	/// </summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;

	/// <summary>
	/// Race type identifier (4 bytes)
	/// Offset: 0x1C
	/// Examples: Human, Elf, Dwarf, etc. from RaceInfo.xml
	/// </summary>
	public uint raceTypeID;

	/// <summary>
	/// Gender type (1 byte boolean)
	/// Offset: 0x20
	/// 0 = Male, 1 = Female
	/// </summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool genderType;

	/// <summary>
	/// Life job type identifier (1 byte)
	/// Offset: 0x21
	/// Examples: Gatherer, Crafter, etc. from JobInfo.xml
	/// </summary>
	public byte lifeJobTypeID;

	/// <summary>
	/// Head/face type (1 byte)
	/// Offset: 0x22
	/// Index into available head models for the race
	/// </summary>
	public byte headType;

	/// <summary>
	/// Hair style type (1 byte)
	/// Offset: 0x23
	/// Index into available hair styles for the race
	/// </summary>
	public byte hairType;

	/// <summary>
	/// Eye type (1 byte)
	/// Offset: 0x24
	/// Index into available eye types for the race
	/// </summary>
	public byte eyeType;

	/// <summary>
	/// Ear type (1 byte)
	/// Offset: 0x25
	/// Index into available ear types for the race
	/// </summary>
	public byte earType;

	/// <summary>
	/// Underwear type (2 bytes)
	/// Offset: 0x26
	/// Starting underwear/clothing item ID
	/// </summary>
	public short underwearType;

	/// <summary>
	/// Family name type (1 byte)
	/// Offset: 0x28
	/// Surname or family name identifier
	/// </summary>
	public byte familyNameType;

	/// <summary>
	/// Skin color type (1 byte)
	/// Offset: 0x29
	/// Index into available skin colors for the race
	/// </summary>
	public byte skinColorType;

	/// <summary>
	/// Hair color type (1 byte)
	/// Offset: 0x2A
	/// Index into available hair colors
	/// </summary>
	public byte hairColorType;

	/// <summary>
	/// Eye color type (1 byte)
	/// Offset: 0x2B
	/// Index into available eye colors
	/// </summary>
	public byte eyeColorType;

	/// <summary>
	/// Eyebrow color type (1 byte)
	/// Offset: 0x2C
	/// Index into available eyebrow colors
	/// </summary>
	public byte eyeBrowColorType;
}

