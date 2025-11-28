using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet when player enters a private chatroom.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PRIVATE_CHATROOM_ENTERED
/// Size: 13 bytes total (header + fixed fields)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 masterID (8 bytes)
/// - 0x0C: char type (1 byte)
/// Note: Variable-length member list follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PRIVATE_CHATROOM_ENTERED : IPacketVar
{
	/// <summary>Chatroom owner's instance ID (offset 0x04)</summary>
	public long masterID;

	/// <summary>Chatroom type/category (offset 0x0C)</summary>
	public byte type;

	// Note: Variable-length member list follows, handled separately
}
