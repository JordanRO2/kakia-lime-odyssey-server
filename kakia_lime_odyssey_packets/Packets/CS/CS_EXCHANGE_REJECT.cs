/// <summary>
/// Client->Server packet to reject an incoming exchange/trade request.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_EXCHANGE_REJECT
/// Size: 2 bytes (header-only)
/// Triggered by: Client rejecting trade request from another player
/// Response: SC_EXCHANGE_REJECT
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EXCHANGE_REJECT : IPacketFixed
{
    // Header-only packet - no additional fields
}
