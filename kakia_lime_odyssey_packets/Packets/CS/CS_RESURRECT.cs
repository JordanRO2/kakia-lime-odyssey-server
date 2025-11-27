using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to resurrect at current location.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_RESURRECT
/// Size: 0 bytes (2 bytes with PACKET_FIX header only)
/// Response: SC_RESURRECTED
/// Sent when player chooses to resurrect in place (e.g., using resurrection stone/skill).
/// Different from CS_ESCAPE which returns to town.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_RESURRECT
{
	// No fields - header-only packet
}
