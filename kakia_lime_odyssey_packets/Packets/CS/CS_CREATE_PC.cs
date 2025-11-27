using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_CREATE_PC - Client to Server character creation packet
/// Total size: 45 bytes (including 2-byte PACKET_FIX header)
/// IDA verified: 2025-11-26
/// Structure name in client: PACKET_CS_CREATE_PC
/// </summary>
/// <remarks>
/// This packet is sent when a player creates a new character.
/// Contains character name and all appearance/customization data.
/// Server responds with SC_CREATED_PC on success.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_CREATE_PC
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

