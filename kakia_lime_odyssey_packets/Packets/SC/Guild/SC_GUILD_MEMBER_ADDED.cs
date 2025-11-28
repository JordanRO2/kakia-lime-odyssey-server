using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying that a new member has joined the guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_MEMBER_ADDED
/// Size: 74 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: GUILD_MEMBER member (64 bytes)
/// - 0x42: int loginMember (4 bytes)
/// - 0x46: int totalMember (4 bytes)
/// Triggered by: CS_GUILD_JOIN, guild join approval
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_MEMBER_ADDED : IPacketFixed
{
	/// <summary>New guild member information (offset 0x02)</summary>
	public GUILD_MEMBER member;

	/// <summary>Number of members currently logged in (offset 0x42)</summary>
	public int loginMember;

	/// <summary>Total number of guild members (offset 0x46)</summary>
	public int totalMember;
}
