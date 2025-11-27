using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying player of guild invitation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_INVITED
/// Size: 79 bytes total (2 byte header + 26 byte pcName + 51 byte guildName)
/// Triggered by: CS_GUILD_INVITE
/// Sent to: Invited player only
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_INVITED
{
	/// <summary>Name of player who sent the invitation (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] pcName;

	/// <summary>Guild name (max 50 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] guildName;
}
