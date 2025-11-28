using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet indicating weapon hit is ready.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_READY_WEAPON_HITING
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_READY_WEAPON_HITING : IPacketFixed
{
	// Header only - no payload
}
