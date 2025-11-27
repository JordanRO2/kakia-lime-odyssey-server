using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_REQUEST_BOARD_QUESTS - Request quest board list
/// IDA Structure: PACKET_CS_REQUEST_BOARD_QUESTS (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this to request available quests from a quest board.
/// Used when quest board is already selected (as opposed to CS_SELECT_AND_REQUEST_BOARD_QUESTS).
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// Total: 2 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_BOARD_QUESTS
{
    // Header only packet - no additional fields
}
