using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying guild option change.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_CHANGED_OPTION
/// Size: 3 bytes total (2 byte header + 1 byte type)
/// Triggered by: CS_GUILD_CHANGE_OPTION
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_CHANGED_OPTION
{
	/// <summary>Guild option type/flags</summary>
	public byte type;
}
