using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_LEAVE_SIGHT_QUEST_BOARD - Quest board leaves sight
/// IDA Structure: PACKET_SC_LEAVE_SIGHT_QUEST_BOARD (10 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when a quest board leaves the player's view range.
///
/// Structure layout:
/// 0x00-0x09: PACKET_SC_LEAVE_ZONEOBJ (10 bytes) - base zone object leave data
///   - 0x00: PACKET_FIX (2 bytes) - base packet header
///   - 0x02: __int64 objInstID (8 bytes) - quest board instance ID
/// Total: 10 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LEAVE_SIGHT_QUEST_BOARD : IPacketFixed
{
    public SC_LEAVE_ZONEOBJ baseData;
}
