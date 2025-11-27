using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating guild creation is ready/allowed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_CREATE_READY
/// Size: 2 bytes (header only, no payload)
/// Triggered by: CS_GUILD_CREATE validation
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_CREATE_READY
{
	// Empty payload - header only packet
}
