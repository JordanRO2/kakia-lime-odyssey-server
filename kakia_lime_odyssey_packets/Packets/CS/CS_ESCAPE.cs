using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to escape from current location (return to town/respawn).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ESCAPE
/// Size: 0 bytes (2 bytes with PACKET_FIX header only)
/// Response: SC_WARP (warps player to safe location)
/// Typically used after death or when using escape scrolls/items.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_ESCAPE
{
	// No fields - header-only packet
}
