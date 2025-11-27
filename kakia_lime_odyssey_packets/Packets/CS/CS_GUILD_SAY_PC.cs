using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet for guild chat message.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_GUILD_SAY_PC
/// Size: 12 bytes total (4 byte PACKET_VAR header + 4 byte maintainTime + 4 byte type)
/// Note: Variable length - actual message follows these fixed fields
/// Response: SC_GUILD_SAY (broadcast to guild members)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct CS_GUILD_SAY_PC
{
	/// <summary>Message maintain time (TTL)</summary>
	public uint maintainTime;

	/// <summary>Chat message type</summary>
	public int type;

	// Variable length message data follows (not included in struct)
}
