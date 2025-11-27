using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_BOARD_CANCEL - Close quest board
/// IDA Structure: PACKET_CS_BOARD_CANCEL (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player closes the quest board UI.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// Total: 2 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_BOARD_CANCEL
{
    // Header only packet - no additional fields
}
