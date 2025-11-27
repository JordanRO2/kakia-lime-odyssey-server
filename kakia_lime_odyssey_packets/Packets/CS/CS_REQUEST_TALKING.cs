using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_REQUEST_TALKING - Continue NPC dialog
/// IDA Structure: PACKET_CS_REQUEST_TALKING (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this to continue/request next dialog with already selected NPC.
/// Used when NPC is already targeted (as opposed to CS_SELECT_AND_REQUEST_TALKING).
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// Total: 2 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_TALKING
{
    // Header only packet - no additional fields
}
