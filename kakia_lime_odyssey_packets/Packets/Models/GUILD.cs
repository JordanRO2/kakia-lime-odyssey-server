using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Guild information structure.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: GUILD
/// Size: 200 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned int dbid (4 bytes)
/// - 0x04: char[51] name (51 bytes)
/// - 0x37: padding (1 byte)
/// - 0x38: unsigned int leader (4 bytes)
/// - 0x3C: int fame (4 bytes)
/// - 0x40: int point (4 bytes)
/// - 0x44: int grade (4 bytes)
/// - 0x48: unsigned __int8 option (1 byte)
/// - 0x49: char[101] notice (101 bytes)
/// - 0xAE: padding (2 bytes)
/// - 0xB0: DB_TIME since (16 bytes)
/// - 0xC0: int loginMember (4 bytes)
/// - 0xC4: int totalMember (4 bytes)
/// Used in: SC_GUILD_INFO
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GUILD
{
	/// <summary>Guild database ID (offset 0x00)</summary>
	public uint dbid;

	/// <summary>Guild name (offset 0x04, 51 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] name;

	/// <summary>Padding for alignment (offset 0x37)</summary>
	public byte padding1;

	/// <summary>Guild leader member ID (offset 0x38)</summary>
	public uint leader;

	/// <summary>Guild fame/reputation (offset 0x3C)</summary>
	public int fame;

	/// <summary>Guild points (offset 0x40)</summary>
	public int point;

	/// <summary>Guild grade/rank (offset 0x44)</summary>
	public int grade;

	/// <summary>Guild options/settings (offset 0x48)</summary>
	public byte option;

	/// <summary>Guild notice/announcement (offset 0x49, 101 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)]
	public byte[] notice;

	/// <summary>Padding for alignment (offset 0xAE)</summary>
	public byte padding2;

	/// <summary>Padding for alignment (offset 0xAF)</summary>
	public byte padding3;

	/// <summary>Guild creation date/time (offset 0xB0)</summary>
	public DB_TIME since;

	/// <summary>Number of members currently logged in (offset 0xC0)</summary>
	public int loginMember;

	/// <summary>Total number of guild members (offset 0xC4)</summary>
	public int totalMember;
}
