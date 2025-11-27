using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Guild information structure
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: GUILD
/// Size: 200 bytes
/// Used in: SC_GUILD_INFO
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct GUILD
{
	/// <summary>Guild database ID</summary>
	public uint dbid;

	/// <summary>Guild name (max 50 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] name;

	/// <summary>Padding for alignment</summary>
	private byte padding1;

	/// <summary>Guild leader member ID</summary>
	public uint leader;

	/// <summary>Guild fame/reputation</summary>
	public int fame;

	/// <summary>Guild points</summary>
	public int point;

	/// <summary>Guild grade/rank</summary>
	public int grade;

	/// <summary>Guild options/settings</summary>
	public byte option;

	/// <summary>Guild notice/announcement (max 100 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)]
	public byte[] notice;

	/// <summary>Padding for alignment</summary>
	private byte padding2;
	private ushort padding3;

	/// <summary>Guild creation date/time</summary>
	public DB_TIME since;

	/// <summary>Number of members currently logged in</summary>
	public int loginMember;

	/// <summary>Total number of guild members</summary>
	public int totalMember;
}
