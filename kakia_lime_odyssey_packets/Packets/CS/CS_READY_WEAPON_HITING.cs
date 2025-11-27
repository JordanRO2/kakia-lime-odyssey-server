/// <summary>
/// Client->Server packet indicating weapon hit is ready.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_READY_WEAPON_HITING
/// Size: 0 bytes (2 with PACKET_FIX header)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_READY_WEAPON_HITING
{
	// Header only - no payload
}
