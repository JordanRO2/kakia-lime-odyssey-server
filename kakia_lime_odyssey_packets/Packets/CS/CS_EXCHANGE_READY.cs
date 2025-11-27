/// <summary>
/// Client->Server packet to mark ready status in active trade window.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_EXCHANGE_READY
/// Size: 2 bytes (header-only)
/// Triggered by: Client clicking ready/lock button in trade window
/// Response: SC_EXCHANGE_READY
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EXCHANGE_READY : IPacketFixed
{
    // Header-only packet - no additional fields
}
