using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing complete guild information.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_INFO
/// Size: 208 bytes total (4 byte PACKET_VAR header + 4 byte myID + 200 byte GUILD)
/// Triggered by: Guild join, guild info request
/// Note: Variable length packet - guild member list follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_GUILD_INFO
{
	/// <summary>Requesting player's member ID in the guild</summary>
	public uint myID;

	/// <summary>Guild information</summary>
	public GUILD guildInfo;

	// Variable length guild member list follows (array of GUILD_MEMBER)
}
