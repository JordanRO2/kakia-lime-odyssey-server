using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet sent when client finishes warp/teleport animation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_FINISH_WARP
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// Response: None (acknowledgment only)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_FINISH_WARP : IPacketFixed
{
	// Header only - no payload
}
