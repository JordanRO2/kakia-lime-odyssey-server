using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_USE_STREAM_GAUGE - Use stream gauge ability
/// IDA Structure: PACKET_CS_USE_STREAM_GAUGE (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player uses a stream gauge ability.
/// Stream gauges appear to be a weapon/combat mechanic where the player
/// charges or builds up energy (gauge) before releasing an attack.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header (ushort header)
/// Total: 2 bytes
///
/// IDA Analysis:
/// - Structure name: PACKET_CS_USE_STREAM_GAUGE
/// - Total size: 2 bytes
/// - Member count: 1 (PACKET_FIX base)
/// - Contains only the base header, no additional fields
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_USE_STREAM_GAUGE
{
    // Header only packet - no additional fields
    // The stream gauge state/amount is likely tracked server-side
}
