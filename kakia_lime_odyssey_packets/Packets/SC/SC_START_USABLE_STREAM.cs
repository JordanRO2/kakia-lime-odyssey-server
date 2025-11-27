using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_START_USABLE_STREAM - Stream becomes available/usable
/// IDA Structure: PACKET_SC_START_USABLE_STREAM (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when a stream ability becomes available for use.
/// Streams appear to be special abilities or states (like channeled abilities,
/// gathering, crafting states, or rest modes) that can be started/stopped.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header (ushort header)
/// Total: 2 bytes
///
/// IDA Analysis:
/// - Structure name: PACKET_SC_START_USABLE_STREAM
/// - Total size: 2 bytes
/// - Member count: 1 (PACKET_FIX base)
/// - Contains only the base header, no additional fields
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_START_USABLE_STREAM
{
    // Header only packet - no additional fields
}
