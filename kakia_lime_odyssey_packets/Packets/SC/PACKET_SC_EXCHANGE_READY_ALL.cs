/// <summary>
/// Server packet sent when both players have marked ready.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_EXCHANGE_READY_ALL
/// Size: 2 bytes (header only)
/// Ordinal: 2751
/// Sent to both players when both have clicked ready, allowing final confirmation.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_EXCHANGE_READY_ALL : IPacketFixed
{
}
