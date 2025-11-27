using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying that a new member has joined the guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_MEMBER_ADDED
/// Size: 74 bytes total (2 byte header + 64 byte GUILD_MEMBER + 4 byte loginMember + 4 byte totalMember)
/// Triggered by: CS_GUILD_JOIN, guild join approval
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_MEMBER_ADDED
{
	/// <summary>New guild member information</summary>
	public GUILD_MEMBER member;

	/// <summary>Number of members currently logged in</summary>
	public int loginMember;

	/// <summary>Total number of guild members</summary>
	public int totalMember;
}
