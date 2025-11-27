/// <summary>
/// Client->Server request to leave current party.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_SECEDE
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Response: SC_PARTY_SECEDED
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_SECEDE
{
	// Empty packet - header only
}
