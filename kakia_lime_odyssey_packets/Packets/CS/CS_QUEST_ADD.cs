using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_QUEST_ADD - Accept quest
/// IDA Structure: PACKET_CS_QUEST_ADD (6 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player accepts a quest.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: unsigned int typeID - quest type ID to accept
/// Total: 6 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_QUEST_ADD
{
    public uint typeID;
}
