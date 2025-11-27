using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet updating a guild member's life job level.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_UPDATE_MEMBER_LIFE_JOB_LV
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: int lv (4 bytes)
/// Triggered by: Member life job level up
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_UPDATE_MEMBER_LIFE_JOB_LV : IPacketFixed
{
	/// <summary>Member database index (offset 0x02)</summary>
	public uint idx;

	/// <summary>New life job level (offset 0x06)</summary>
	public int lv;
}
