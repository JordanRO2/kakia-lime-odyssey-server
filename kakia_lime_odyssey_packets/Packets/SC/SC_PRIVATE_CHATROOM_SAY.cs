using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_PRIVATE_CHATROOM_SAY - Private chatroom message broadcast
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_PRIVATE_CHATROOM_SAY
/// IDA Structure Size: 12 bytes (base size, variable message)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: instID (__int64) - 8 bytes
/// +0x0C: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - instID is the sender's instance ID
/// - message is a variable-length ASCII string
/// - Broadcast to all chatroom members when someone sends a message
///
/// Type Mappings (IDA -> C#):
/// - __int64 -> long (instID)
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_PRIVATE_CHATROOM_SAY
{
	public long instID;
	public string message;
}
