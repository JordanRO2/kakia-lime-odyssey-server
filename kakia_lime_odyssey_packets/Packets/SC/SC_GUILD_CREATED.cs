using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming guild has been created.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_CREATED
/// Size: 2 bytes (header only, no payload)
/// Triggered by: Successful guild creation after CS_GUILD_CREATE
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_CREATED
{
	// Empty payload - header only packet
}
