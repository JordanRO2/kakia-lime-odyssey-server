using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet with stream gauge minigame result (fishing/gathering).
/// </summary>
/// <remarks>
/// IDA Verified: Partial (2025-11-27)
/// Size: 3 bytes total
/// Memory Layout:
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int8 result (1 byte)
/// Result values:
/// - 0: Failure
/// - 1: Success (normal quality)
/// - 2: Great success (high quality)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_USE_STREAM_GAUGE_RESULT : IPacketFixed
{
	/// <summary>Minigame result (offset 0x02): 0=fail, 1=success, 2=great</summary>
	public byte result;
}
