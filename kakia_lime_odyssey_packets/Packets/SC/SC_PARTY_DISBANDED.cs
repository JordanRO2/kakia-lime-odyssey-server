/// <summary>
/// Server->Client notification that party was disbanded.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_DISBANDED
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Triggered by: CS_PARTY_DISBAND or leader logout
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_DISBANDED
{
	// Empty packet - header only
}
