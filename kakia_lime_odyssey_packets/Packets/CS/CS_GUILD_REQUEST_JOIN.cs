using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to request to join a guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_REQUEST_JOIN
/// Size: 53 bytes total (2 byte header + 51 byte guild name)
/// Response: Guild leader receives join request notification
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_REQUEST_JOIN
{
	/// <summary>Guild name to request to join (max 50 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] name;
}
