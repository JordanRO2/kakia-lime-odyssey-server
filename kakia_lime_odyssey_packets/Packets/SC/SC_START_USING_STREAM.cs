using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_START_USING_STREAM - Entity begins using a stream
/// IDA Structure: PACKET_SC_START_USING_STREAM (10 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when an entity (player, NPC, mob) begins using a stream ability.
/// The instID identifies which entity is performing the stream action.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header (ushort header)
/// 0x02: __int64 instID (8 bytes) - instance ID of entity using stream
/// Total: 10 bytes
///
/// IDA Analysis:
/// - Structure name: PACKET_SC_START_USING_STREAM
/// - Total size: 10 bytes
/// - Member count: 2 (PACKET_FIX base + instID field)
/// - Field layout verified:
///   [0] offset 0x00, size 2, PACKET_FIX (base header)
///   [1] offset 0x02, size 8, __int64 instID
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_START_USING_STREAM
{
    /// <summary>Instance ID of the entity starting to use the stream</summary>
    public long instID;
}
