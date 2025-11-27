/// <summary>
/// Client->Server invite a player to party by character name.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_INVITE
/// Size: 26 bytes (28 with PACKET_FIX header)
/// Response: SC_PARTY_INVITED
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_INVITE
{
	/// <summary>Target character name (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;
}
