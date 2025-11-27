using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_QUEST_COMPLETE - Complete quest and claim reward
/// IDA Structure: PACKET_CS_QUEST_COMPLETE (42 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player turns in a completed quest.
/// Includes reward choice items if quest offers multiple reward options.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: REWARD_CHOICE_ITEMS (40 bytes) - selected reward items
///   - int[10] choiceItems - array of 10 item IDs for reward choices
/// Total: 42 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_QUEST_COMPLETE
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] choiceItems;
}
