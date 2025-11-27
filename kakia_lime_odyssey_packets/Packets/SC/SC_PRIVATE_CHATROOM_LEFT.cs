using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_PRIVATE_CHATROOM_LEFT - Member left chatroom notification
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_PRIVATE_CHATROOM_LEAVED
/// IDA Structure Size: 10 bytes
///
/// IDA Structure Layout:
/// +0x00: PACKET_FIX (header: ushort) - 2 bytes [handled by framework]
/// +0x02: instID (__int64) - 8 bytes
///
/// C# Implementation Notes:
/// - PACKET_FIX header (2 bytes) is stripped by RawPacket.ParsePackets
/// - instID is the instance ID of the player who left/was kicked from the chatroom
/// - Sent to all remaining chatroom members when someone leaves or is banned
///
/// Type Mappings (IDA -> C#):
/// - __int64 -> long (instID)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PRIVATE_CHATROOM_LEFT
{
	public long instID;
}
