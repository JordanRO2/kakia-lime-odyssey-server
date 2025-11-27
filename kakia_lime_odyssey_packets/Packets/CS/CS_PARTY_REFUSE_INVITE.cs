/// <summary>
/// Client->Server decline party invitation.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_REFUSE_INVITE
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Response: None (invitation is simply declined)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_REFUSE_INVITE
{
	// Empty packet - header only
}
