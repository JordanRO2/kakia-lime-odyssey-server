using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_PRIVATE_CHATROOM_SAY - Send message in private chatroom packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_PRIVATE_CHATROOM_SAY
/// IDA Structure Size: 4 bytes (base size, variable message)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - message is a variable-length ASCII string
/// - Similar to CS_SAY_PC but for private chatroom context
///
/// Type Mappings (IDA -> C#):
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CS_PRIVATE_CHATROOM_SAY
{
	public string message;
}
