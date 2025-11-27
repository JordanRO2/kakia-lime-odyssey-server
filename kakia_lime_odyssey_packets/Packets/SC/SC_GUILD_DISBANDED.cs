using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying that guild has been disbanded.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_DISBANDED
/// Size: 2 bytes (header only, no payload)
/// Triggered by: CS_GUILD_DISBAND
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_DISBANDED
{
	// Empty payload - header only packet
}
