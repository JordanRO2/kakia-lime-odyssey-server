using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet with guild notice/announcement.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_NOTICE
/// Size: 103 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[101] notice (101 bytes)
/// Triggered by: CS_GUILD_NOTICE
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_NOTICE : IPacketFixed
{
	/// <summary>Guild notice text (offset 0x02, 101 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)]
	public byte[] notice;
}
