using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to invite a player to guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_INVITE
/// Size: 28 bytes total (2 byte header + 26 byte name)
/// Response: SC_GUILD_INVITED (sent to target player)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_INVITE
{
	/// <summary>Target player name (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;
}
