using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying guild leadership change.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_CHANGED_LEADER
/// Size: 6 bytes total (2 byte header + 4 byte idx)
/// Triggered by: CS_GUILD_CHANGE_LEADER
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_CHANGED_LEADER
{
	/// <summary>New leader member database index</summary>
	public uint idx;
}
