using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying that a guild member has logged out.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_MEMBER_DISCONNECTED
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: int loginMember (4 bytes)
/// - 0x0A: int totalMember (4 bytes)
/// Triggered by: Member logout
/// Broadcast to: All online guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_MEMBER_DISCONNECTED : IPacketFixed
{
	/// <summary>Member database index who disconnected (offset 0x02)</summary>
	public uint idx;

	/// <summary>Number of members currently logged in (offset 0x06)</summary>
	public int loginMember;

	/// <summary>Total number of guild members (offset 0x0A)</summary>
	public int totalMember;
}
