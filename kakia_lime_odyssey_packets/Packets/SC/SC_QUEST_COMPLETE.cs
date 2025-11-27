using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for quest completed and turned in.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_QUEST_COMPLETE
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// - 0x06: int completedMain (4 bytes)
/// - 0x0A: int completedSub (4 bytes)
/// - 0x0E: int completedNormal (4 bytes)
/// Triggered by: Quest turn-in success
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_QUEST_COMPLETE : IPacketFixed
{
	/// <summary>Quest type ID (offset 0x02)</summary>
	public uint typeID;

	/// <summary>Updated count of completed main quests (offset 0x06)</summary>
	public int completedMain;

	/// <summary>Updated count of completed sub quests (offset 0x0A)</summary>
	public int completedSub;

	/// <summary>Updated count of completed normal quests (offset 0x0E)</summary>
	public int completedNormal;
}
