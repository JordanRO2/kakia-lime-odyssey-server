/// <summary>
/// Client->Server accept party invitation.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_JOIN
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Response: SC_PARTY_JOINED
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_JOIN
{
	// Empty packet - header only
}
