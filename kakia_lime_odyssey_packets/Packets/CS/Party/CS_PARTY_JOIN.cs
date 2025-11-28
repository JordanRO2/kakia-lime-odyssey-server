using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server accept party invitation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PARTY_JOIN
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// Response: SC_PARTY_JOINED
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PARTY_JOIN : IPacketFixed
{
	// Header only - no payload
}
