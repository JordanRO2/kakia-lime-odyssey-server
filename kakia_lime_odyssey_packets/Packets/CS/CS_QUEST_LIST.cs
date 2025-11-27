using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_QUEST_LIST - Request quest list
/// IDA Structure: PACKET_CS_QUEST_LIST (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this to request the current quest list from the server.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// Total: 2 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_QUEST_LIST
{
    // Header only packet - no additional fields
}
