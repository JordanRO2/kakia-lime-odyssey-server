using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_START_RESTING_STREAM - Begin resting stream (HP/MP regeneration state)
/// IDA Structure: PACKET_SC_START_RESTING_STREAM (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when player enters resting state, which typically provides
/// enhanced HP/MP/SP regeneration. This is likely triggered by sitting down or
/// entering a rest zone.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header (ushort header)
/// Total: 2 bytes
///
/// IDA Analysis:
/// - Structure name: PACKET_SC_START_RESTING_STREAM
/// - Total size: 2 bytes
/// - Member count: 1 (PACKET_FIX base)
/// - Contains only the base header, no additional fields
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_START_RESTING_STREAM
{
    // Header only packet - no additional fields
}
