using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying mission zone creation progress.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CREATING_MISSION_ZONE
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int order (4 bytes)
/// - 0x06: int waiting (4 bytes)
/// - 0x0A: int created (4 bytes)
/// - 0x0E: int createLimit (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CREATING_MISSION_ZONE : IPacketFixed
{
	/// <summary>Player's position in queue (offset 0x02)</summary>
	public int order;

	/// <summary>Number of players waiting in queue (offset 0x06)</summary>
	public int waiting;

	/// <summary>Number of zones currently created (offset 0x0A)</summary>
	public int created;

	/// <summary>Maximum number of zones that can be created (offset 0x0E)</summary>
	public int createLimit;
}
