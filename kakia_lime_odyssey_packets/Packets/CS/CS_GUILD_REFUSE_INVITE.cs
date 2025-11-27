using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to refuse a guild invitation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_REFUSE_INVITE
/// Size: 2 bytes (header only, no payload)
/// Response: None (silent decline)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_REFUSE_INVITE
{
	// Empty payload - header only packet
}
