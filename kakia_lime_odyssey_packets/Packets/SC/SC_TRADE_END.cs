/// <summary>
/// Server->Client packet confirming trade window closed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_TRADE_END
/// Size: 0 bytes (2 with PACKET_FIX header only)
/// Triggered by: CS_TRADE_END
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_TRADE_END
{
	// Empty packet - header only
}
