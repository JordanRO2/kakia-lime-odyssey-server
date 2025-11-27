using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_QUEST_LIST - Quest list summary packet
/// IDA Structure: PACKET_SC_QUEST_LIST (20 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// This packet contains quest completion statistics only.
/// Individual quest details are sent via separate SC_QUEST_ADD packets.
///
/// Structure layout:
/// 0x00: PACKET_VAR (4 bytes) - base packet header
/// 0x04: int count - total number of active quests
/// 0x08: int completedMain - number of completed main quests
/// 0x0C: int completedSub - number of completed sub quests
/// 0x10: int completedNormal - number of completed normal quests
/// Total: 20 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_QUEST_LIST : IPacketVar
{
	public int count;
	public int completedMain;
	public int completedSub;
	public int completedNormal;
}