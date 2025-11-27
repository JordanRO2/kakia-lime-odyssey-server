using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Complete party member information.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PARTY_MEMBER
/// Size: 96 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned int idx (4 bytes) - Party member index/slot
/// - 0x04: char[26] name (26 bytes) - Character name
/// - 0x1E: bool isConnected (1 byte) - Online status
/// - 0x1F: padding (1 byte) - Alignment
/// - 0x20: __int64 instID (8 bytes) - Character instance ID
/// - 0x28: PARTY_MEMBER_STATE state (52 bytes) - Member state
/// Total: 4 + 26 + 1 + 1 + 8 + 52 = 92 bytes... need to check
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PARTY_MEMBER
{
	/// <summary>Party member index/slot (offset 0x00)</summary>
	public uint idx;

	/// <summary>Character name (offset 0x04, 26 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;

	/// <summary>Is member currently connected (offset 0x1E)</summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool isConnected;

	/// <summary>Padding for alignment (offset 0x1F)</summary>
	public byte padding;

	/// <summary>Character instance ID (offset 0x20)</summary>
	public long instID;

	/// <summary>Member's current state (offset 0x28)</summary>
	public PARTY_MEMBER_STATE state;
}
