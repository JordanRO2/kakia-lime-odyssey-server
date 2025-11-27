using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to set guild notice/announcement.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_GUILD_NOTICE
/// Size: 103 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[101] notice (101 bytes)
/// Response: SC_GUILD_NOTICE (broadcast to guild)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_GUILD_NOTICE : IPacketFixed
{
	/// <summary>Guild notice text (max 100 chars + null terminator) (offset 0x02)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)]
	public byte[] notice;
}
