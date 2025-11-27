/// <summary>
/// Client->Server packet to cancel an active trade session.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_EXCHANGE_CANCEL
/// Size: 2 bytes (header-only)
/// Triggered by: Client canceling trade window
/// Response: SC_EXCHANGE_CANCEL
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EXCHANGE_CANCEL : IPacketFixed
{
    // Header-only packet - no additional fields
}
