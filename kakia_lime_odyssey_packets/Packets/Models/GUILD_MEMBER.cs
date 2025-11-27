using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Guild member information
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: GUILD_MEMBER
/// Size: 64 bytes
/// Note: 1 byte padding after isConnected (offset 0x1E->0x20) for alignment
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct GUILD_MEMBER
{
	/// <summary>Member database index</summary>
	public uint idx;

	/// <summary>Character name (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;

	/// <summary>Is member currently connected</summary>
	public bool isConnected;

	/// <summary>Padding for 8-byte alignment</summary>
	private byte padding;

	/// <summary>Character instance ID</summary>
	public long instID;

	/// <summary>Member state (job levels, rank, etc.)</summary>
	public GUILD_MEMBER_STATE state;
}
