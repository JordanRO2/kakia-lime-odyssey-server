using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_QUEST_REPORT_TALK - Quest report dialog
/// IDA Structure: PACKET_SC_QUEST_REPORT_TALK (16 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this to display quest report/turn-in dialog with NPC.
/// Variable-length packet with dialog data appended after the base structure.
///
/// Structure layout:
/// 0x00: PACKET_VAR (4 bytes) - base packet header with variable data
/// 0x04: unsigned int typeID - quest type ID
/// 0x08: __int64 objInstID - NPC instance ID
/// Variable: dialog data follows
/// Total: 16 bytes + dialog data
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_QUEST_REPORT_TALK : IPacketVar
{
    public uint typeID;
    public long objInstID;
}
