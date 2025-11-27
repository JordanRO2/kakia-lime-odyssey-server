using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_CHANGE_QUEST_SUBJECT - Quest subject/objective changed
/// IDA Structure: PACKET_SC_CHANGE_QUEST_SUBJECT (8 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when a quest objective/subject changes state.
/// Used to track progress through multi-stage quests.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: unsigned int typeID - quest type ID
/// 0x06: unsigned __int8 subjectNum - subject/objective number
/// 0x07: bool isSuccessed - whether the subject was completed successfully
/// Total: 8 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CHANGE_QUEST_SUBJECT : IPacketFixed
{
    public uint typeID;
    public byte subjectNum;
    public bool isSuccessed;
}
