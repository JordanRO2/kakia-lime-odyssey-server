/// <summary>
/// Client->Server request to join another player's party.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_REQUEST_JOIN
/// Size: 41 bytes (43 with PACKET_FIX header)
/// Response: None (target receives join request)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_REQUEST_JOIN
{
	/// <summary>Target party name (max 40 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
	public string name;
}
