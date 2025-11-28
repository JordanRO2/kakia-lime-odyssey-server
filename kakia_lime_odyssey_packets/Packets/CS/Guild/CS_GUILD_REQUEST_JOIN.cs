using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to request to join a guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_GUILD_REQUEST_JOIN
/// Size: 53 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[51] name (51 bytes)
/// Response: Guild leader receives join request notification
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_GUILD_REQUEST_JOIN : IPacketFixed
{
	/// <summary>Guild name to request to join (max 50 chars + null terminator) (offset 0x02)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] name;
}
