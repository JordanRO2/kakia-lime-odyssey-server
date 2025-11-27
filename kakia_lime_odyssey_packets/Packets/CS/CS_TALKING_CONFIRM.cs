using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_TALKING_CONFIRM - Confirm NPC dialog
/// IDA Structure: PACKET_CS_TALKING_CONFIRM (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player confirms/proceeds with NPC dialog.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// Total: 2 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_TALKING_CONFIRM
{
    // Header only packet - no additional fields
}
