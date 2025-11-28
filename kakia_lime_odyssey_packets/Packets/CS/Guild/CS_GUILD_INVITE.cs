using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to invite a player to guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_GUILD_INVITE
/// Size: 28 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[26] name (26 bytes)
/// Response: SC_GUILD_INVITED (sent to target player)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_GUILD_INVITE : IPacketFixed
{
	/// <summary>Target player name (max 25 chars + null terminator) (offset 0x02)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;
}
