using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_CHANGE_TO_NPC_SHAPE - Transform entity to NPC appearance
/// IDA Structure: PACKET_SC_CHANGE_TO_NPC_SHAPE (14 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when an entity (PC or other) transforms into an NPC appearance.
/// Used for transformation/disguise mechanics.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: __int64 objInstID - entity instance ID being transformed
/// 0x0A: int modelTypeID - NPC model type ID to transform into
/// Total: 14 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CHANGE_TO_NPC_SHAPE : IPacketFixed
{
    public long objInstID;
    public int modelTypeID;
}
