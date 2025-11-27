/// <summary>
/// Server packet sent to notify that the other player has marked ready.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_EXCHANGE_READY
/// Size: 2 bytes (header only)
/// Ordinal: 2750
/// Sent when the other player clicks the ready button.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_EXCHANGE_READY : IPacketFixed
{
}
