/// <summary>
/// Server->Client confirmation that party was successfully created.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_CREATED
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Triggered by: CS_PARTY_CREATE
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_CREATED
{
	// Empty packet - header only
}
