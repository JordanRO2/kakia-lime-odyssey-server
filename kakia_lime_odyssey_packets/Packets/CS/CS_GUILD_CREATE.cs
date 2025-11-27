using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to create a new guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_CREATE
/// Size: 53 bytes total (2 byte header + 51 byte name)
/// Response: SC_GUILD_CREATE_READY, SC_GUILD_CREATED
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_GUILD_CREATE
{
	/// <summary>Guild name (max 50 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] name;
}
