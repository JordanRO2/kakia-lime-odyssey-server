using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server login packet for user authentication.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_LOGIN
/// Size: 62 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int key (4 bytes)
/// - 0x06: char[26] id (26 bytes)
/// - 0x20: char[26] pw (26 bytes)
/// - 0x3A: int revision (4 bytes)
/// Response: SC_LOGIN_RESULT, SC_PC_LIST
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_LOGIN : IPacketFixed
{
	/// <summary>Session encryption key (offset 0x02)</summary>
	public int key;

	/// <summary>Account ID (max 25 chars + null terminator) (offset 0x06)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string id;

	/// <summary>Password (max 25 chars + null terminator) (offset 0x20)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string pw;

	/// <summary>Client revision number (should be 211 for REV211) (offset 0x3A)</summary>
	public int revision;
}
