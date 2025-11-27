using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_FINISH_USABLE_STREAM - Stream is no longer available/usable
/// IDA Structure: PACKET_SC_FINISH_USABLE_STREAM (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when a stream ability is no longer available for use.
/// This could indicate cooldown, resource depletion, or state change that
/// prevents the stream from being used.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header (ushort header)
/// Total: 2 bytes
///
/// IDA Analysis:
/// - Structure name: PACKET_SC_FINISH_USABLE_STREAM
/// - Total size: 2 bytes
/// - Member count: 1 (PACKET_FIX base)
/// - Contains only the base header, no additional fields
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_FINISH_USABLE_STREAM
{
    // Header only packet - no additional fields
}
