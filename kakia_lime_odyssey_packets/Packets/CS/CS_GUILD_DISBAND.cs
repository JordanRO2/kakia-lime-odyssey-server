using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to disband a guild (leader only).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_DISBAND
/// Size: 2 bytes (header only, no payload)
/// Response: SC_GUILD_DISBANDED
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_DISBAND
{
	// Empty payload - header only packet
}
