/// <summary>
/// Client->Server packet to unready/unlock trade window after being ready.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_EXCHANGE_AGAIN
/// Size: 2 bytes (header-only)
/// Triggered by: Client unlocking trade after previously being ready
/// Response: SC_EXCHANGE_AGAIN
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_EXCHANGE_AGAIN : IPacketFixed
{
    // Header-only packet - no additional fields
}
