using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_USE_STREAM_GAUGE_RESULT - Result of stream gauge minigame (fishing/gathering)
/// </summary>
/// <remarks>
/// Result values:
/// - 0: Failure
/// - 1: Success (normal quality)
/// - 2: Great success (high quality)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_USE_STREAM_GAUGE_RESULT
{
	public byte result;
}
