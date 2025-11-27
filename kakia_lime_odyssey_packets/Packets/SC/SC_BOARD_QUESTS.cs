using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_BOARD_QUESTS - Quest board available quests
/// IDA Structure: PACKET_SC_BOARD_QUESTS (4 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this with list of available quests from a quest board.
/// Variable-length packet with quest list data appended after the base structure.
///
/// Structure layout:
/// 0x00: PACKET_VAR (4 bytes) - base packet header with variable data
/// Variable: quest list data follows
/// Total: 4 bytes + quest data
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_BOARD_QUESTS : IPacketVar
{
    // Header only base structure - quest data appended as variable data
}
