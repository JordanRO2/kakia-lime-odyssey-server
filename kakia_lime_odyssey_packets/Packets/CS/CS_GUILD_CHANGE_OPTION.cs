using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to change guild options/settings.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_CHANGE_OPTION
/// Size: 3 bytes total (2 byte header + 1 byte option type)
/// Response: SC_GUILD_CHANGED_OPTION (broadcast to guild)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_CHANGE_OPTION
{
	/// <summary>Guild option type/flags</summary>
	public byte type;
}
