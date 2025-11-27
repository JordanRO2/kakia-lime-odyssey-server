using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet updating a guild member's life job level.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_UPDATE_MEMBER_LIFE_JOB_LV
/// Size: 10 bytes total (2 byte header + 4 byte idx + 4 byte lv)
/// Triggered by: Member life job level up
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_UPDATE_MEMBER_LIFE_JOB_LV
{
	/// <summary>Member database index</summary>
	public uint idx;

	/// <summary>New life job level</summary>
	public int lv;
}
