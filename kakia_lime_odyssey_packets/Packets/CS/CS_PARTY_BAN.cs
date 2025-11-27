/// <summary>
/// Client->Server kick a party member (leader only).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_BAN
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Response: SC_PARTY_MEMBER_BANNED
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_BAN
{
	/// <summary>Party member index to kick</summary>
	public uint idx;
}
