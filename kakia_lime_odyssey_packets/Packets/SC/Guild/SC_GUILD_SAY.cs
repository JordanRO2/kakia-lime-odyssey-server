using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for guild chat message broadcast.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_SAY
/// Size: 16 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned int idx (4 bytes)
/// - 0x08: unsigned int maintainTime (4 bytes)
/// - 0x0C: int type (4 bytes)
/// Note: Variable length - actual message follows these fixed fields
/// Triggered by: CS_GUILD_SAY_PC
/// Broadcast to: All online guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_SAY : IPacketVar
{
	/// <summary>Member database index who sent the message (offset 0x04)</summary>
	public uint idx;

	/// <summary>Message maintain time/TTL (offset 0x08)</summary>
	public uint maintainTime;

	/// <summary>Chat message type (offset 0x0C)</summary>
	public int type;

	// Variable length message data follows (not included in struct)
}
