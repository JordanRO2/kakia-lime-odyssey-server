using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_QUEST_ADD - Quest accepted
/// IDA Structure: PACKET_SC_QUEST_ADD (8 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when a quest is accepted/added to player's quest log.
/// Variable-length packet with quest data appended after the base structure.
///
/// Structure layout:
/// 0x00: PACKET_VAR (4 bytes) - base packet header with variable data
/// 0x04: unsigned int typeID - quest type ID
/// Variable: quest data follows
/// Total: 8 bytes + quest data
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_QUEST_ADD : IPacketVar
{
    public uint typeID;
}
