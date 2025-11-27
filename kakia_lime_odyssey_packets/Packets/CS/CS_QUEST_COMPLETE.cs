using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to complete quest and claim reward.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_QUEST_COMPLETE
/// Size: 42 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int[10] choiceItems (40 bytes) - selected reward items
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_QUEST_COMPLETE : IPacketFixed
{
	/// <summary>Selected reward item IDs (offset 0x02)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
	public int[] choiceItems;
}
