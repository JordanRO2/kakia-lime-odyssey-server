using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Guild member information
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: GUILD_MEMBER
/// Size: 64 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned int idx (4 bytes)
/// - 0x04: char[26] name (26 bytes)
/// - 0x1E: bool isConnected (1 byte)
/// - 0x1F: padding (1 byte) - for alignment before 8-byte instID
/// - 0x20: __int64 instID (8 bytes)
/// - 0x28: GUILD_MEMBER_STATE state (20 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GUILD_MEMBER
{
	/// <summary>Member database index (offset 0x00)</summary>
	public uint idx;

	/// <summary>Character name (offset 0x04, 26 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;

	/// <summary>Is member currently connected (offset 0x1E)</summary>
	public bool isConnected;

	/// <summary>Padding for alignment (offset 0x1F)</summary>
	public byte padding;

	/// <summary>Character instance ID (offset 0x20)</summary>
	public long instID;

	/// <summary>Member state - job levels, rank, etc. (offset 0x28)</summary>
	public GUILD_MEMBER_STATE state;
}
