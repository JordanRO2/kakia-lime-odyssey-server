using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to kick/ban a member from guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_BAN
/// Size: 6 bytes total (2 byte header + 4 byte member idx)
/// Response: SC_GUILD_SECEDED (broadcast to guild)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_BAN
{
	/// <summary>Guild member database index to kick</summary>
	public uint idx;
}
