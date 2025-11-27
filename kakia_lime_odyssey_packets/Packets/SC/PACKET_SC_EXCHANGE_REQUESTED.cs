/// <summary>
/// Server packet sent to notify player that someone wants to trade with them.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_EXCHANGE_REQUESTED
/// Size: 2 bytes (header only)
/// Ordinal: 2739
/// Sent to the target player when they receive a trade request.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_EXCHANGE_REQUESTED : IPacketFixed
{
}
