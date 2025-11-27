using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet updating a guild member's state.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_MEMBER_STATE
/// Size: 26 bytes total (2 byte header + 4 byte idx + 20 byte GUILD_MEMBER_STATE)
/// Triggered by: Member job/level/rank change
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_MEMBER_STATE
{
	/// <summary>Member database index</summary>
	public uint idx;

	/// <summary>Updated member state</summary>
	public GUILD_MEMBER_STATE state;
}
