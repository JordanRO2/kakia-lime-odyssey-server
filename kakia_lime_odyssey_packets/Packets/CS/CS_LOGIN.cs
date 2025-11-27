/// <summary>
/// Client->Server login packet for user authentication.
/// </summary>
/// <remarks>
/// IDA Verified: Yes
/// IDA Struct: PACKET_CS_LOGIN
/// Size: 60 bytes (62 with PACKET_FIX header)
/// Response: SC_LOGIN_RESULT, SC_PC_LIST
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_LOGIN
{
	/// <summary>Session encryption key</summary>
	public int key;

	/// <summary>Account ID (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string id;

	/// <summary>Password (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string pw;

	/// <summary>Client revision number (should be 211 for REV211)</summary>
	public int revision;
}
