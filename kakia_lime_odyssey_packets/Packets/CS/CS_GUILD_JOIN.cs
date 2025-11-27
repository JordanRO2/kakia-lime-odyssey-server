using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to accept a guild invitation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_JOIN
/// Size: 2 bytes (header only, no payload)
/// Response: SC_GUILD_MEMBER_ADDED, SC_GUILD_INFO
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_JOIN
{
	// Empty payload - header only packet
}
