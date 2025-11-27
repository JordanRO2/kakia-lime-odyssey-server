using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_QUEST_SUCCESS - Quest objectives completed
/// IDA Structure: PACKET_SC_QUEST_SUCCESS (6 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when all quest objectives are completed (ready to turn in).
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: unsigned int typeID - quest type ID
/// Total: 6 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_QUEST_SUCCESS : IPacketFixed
{
    public uint typeID;
}
