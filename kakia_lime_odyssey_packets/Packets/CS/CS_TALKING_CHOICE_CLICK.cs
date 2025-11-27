using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_TALKING_CHOICE_CLICK - Select dialog choice
/// IDA Structure: PACKET_CS_TALKING_CHOICE_CLICK (4 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player selects a choice in NPC dialog menu.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: unsigned __int16 choiceNum - selected choice index
/// Total: 4 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_TALKING_CHOICE_CLICK
{
    public ushort choiceNum;
}
