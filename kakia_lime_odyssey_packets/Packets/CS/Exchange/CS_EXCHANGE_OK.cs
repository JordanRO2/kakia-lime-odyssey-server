/// <summary>
/// Client->Server packet to finalize and complete the trade.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_EXCHANGE_OK
/// Size: 2 bytes (header-only)
/// Triggered by: Client confirming final trade when both parties are ready
/// Response: SC_EXCHANGE_OK
/// Database: inventory (items transferred), characters (gold updated)
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EXCHANGE_OK : IPacketFixed
{
    // Header-only packet - no additional fields
}
