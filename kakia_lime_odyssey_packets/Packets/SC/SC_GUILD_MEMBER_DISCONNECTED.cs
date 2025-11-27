using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying that a guild member has logged out.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_MEMBER_DISCONNECTED
/// Size: 14 bytes total (2 byte header + 4 byte idx + 4 byte loginMember + 4 byte totalMember)
/// Triggered by: Member logout
/// Broadcast to: All online guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_MEMBER_DISCONNECTED
{
	/// <summary>Member database index who disconnected</summary>
	public uint idx;

	/// <summary>Number of members currently logged in</summary>
	public int loginMember;

	/// <summary>Total number of guild members</summary>
	public int totalMember;
}
