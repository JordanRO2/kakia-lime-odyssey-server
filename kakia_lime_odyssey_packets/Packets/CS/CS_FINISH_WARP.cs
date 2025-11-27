/// <summary>
/// Client->Server packet sent when client finishes warp/teleport animation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_FINISH_WARP
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Response: None (acknowledgment only)
/// Note: This is an empty packet containing only the PACKET_FIX header.
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_FINISH_WARP : IPacketFixed
{
    // Empty packet - only contains PACKET_FIX header (2 bytes)
}
