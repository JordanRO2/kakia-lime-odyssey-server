using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming trade window closed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_TRADE_END
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// Triggered by: CS_TRADE_END
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TRADE_END : IPacketFixed
{
	// Empty packet - header only
}
