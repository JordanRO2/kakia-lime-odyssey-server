using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_SELECT_AND_REQUEST_BOARD_QUESTS - Select and request quest board
/// IDA Structure: PACKET_CS_SELECT_AND_REQUEST_BOARD_QUESTS (10 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player selects a quest board and requests available quests.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: __int64 objInstID - quest board instance ID
/// Total: 10 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_SELECT_AND_REQUEST_BOARD_QUESTS
{
    public long objInstID;
}
