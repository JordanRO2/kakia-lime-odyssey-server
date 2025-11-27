using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_QUEST_COMPLETE - Quest completed and turned in
/// IDA Structure: PACKET_SC_QUEST_COMPLETE (18 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when quest is successfully completed and rewards claimed.
/// Updates the completion counters for quest statistics.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: unsigned int typeID - quest type ID
/// 0x06: int completedMain - updated count of completed main quests
/// 0x0A: int completedSub - updated count of completed sub quests
/// 0x0E: int completedNormal - updated count of completed normal quests
/// Total: 18 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_QUEST_COMPLETE : IPacketFixed
{
    public uint typeID;
    public int completedMain;
    public int completedSub;
    public int completedNormal;
}
