using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_PRIVATE_CHATROOM_ENTERED - Successfully entered private chatroom
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_PRIVATE_CHATROOM_ENTERED
/// IDA Structure Size: 13 bytes (base size, variable member list)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: masterID (__int64) - 8 bytes
/// +0x0C: type (char) - 1 byte
/// +0x0D: memberList (variable length data)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - masterID is the chatroom owner's instance ID
/// - type is the chatroom type/category
/// - memberList contains information about current chatroom members (variable length)
///
/// Type Mappings (IDA -> C#):
/// - __int64 -> long (masterID)
/// - char -> byte (type)
/// - variable data -> byte[] (memberList)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_PRIVATE_CHATROOM_ENTERED
{
	public long masterID;
	public byte type;
	public byte[] memberList;
}
