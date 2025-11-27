using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet with guild notice/announcement.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_NOTICE
/// Size: 103 bytes total (2 byte header + 101 byte notice)
/// Triggered by: CS_GUILD_NOTICE
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_NOTICE
{
	/// <summary>Guild notice text (max 100 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 101)]
	public byte[] notice;
}
