using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet updating a guild member's state.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_MEMBER_STATE
/// Size: 26 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: GUILD_MEMBER_STATE state (20 bytes)
/// Triggered by: Member job/level/rank change
/// Broadcast to: All guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_MEMBER_STATE : IPacketFixed
{
	/// <summary>Member database index (offset 0x02)</summary>
	public uint idx;

	/// <summary>Updated member state (offset 0x06)</summary>
	public GUILD_MEMBER_STATE state;
}
