using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to use stream gauge ability (fishing/gathering minigame).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_USE_STREAM_GAUGE
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// Note: Stream gauge state is tracked server-side
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_STREAM_GAUGE : IPacketFixed
{
	// Header only - no payload
}
