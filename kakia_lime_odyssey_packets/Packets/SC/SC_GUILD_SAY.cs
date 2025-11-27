using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for guild chat message broadcast.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_SAY
/// Size: 16 bytes total (4 byte PACKET_VAR header + 4 byte idx + 4 byte maintainTime + 4 byte type)
/// Note: Variable length - actual message follows these fixed fields
/// Triggered by: CS_GUILD_SAY_PC
/// Broadcast to: All online guild members
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_GUILD_SAY
{
	/// <summary>Member database index who sent the message</summary>
	public uint idx;

	/// <summary>Message maintain time (TTL)</summary>
	public uint maintainTime;

	/// <summary>Chat message type</summary>
	public int type;

	// Variable length message data follows (not included in struct)
}
