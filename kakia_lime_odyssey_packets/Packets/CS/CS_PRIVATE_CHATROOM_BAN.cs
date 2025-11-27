using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_PRIVATE_CHATROOM_BAN - Ban/kick player from private chatroom packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_PRIVATE_CHATROOM_BAN
/// IDA Structure Size: 10 bytes
///
/// IDA Structure Layout:
/// +0x00: PACKET_FIX (header: ushort) - 2 bytes [handled by framework]
/// +0x02: instID (__int64) - 8 bytes
///
/// C# Implementation Notes:
/// - PACKET_FIX header (2 bytes) is stripped by RawPacket.ParsePackets
/// - instID is the instance ID of the player to kick from the chatroom
/// - Only chatroom owner can ban/kick members
///
/// Type Mappings (IDA -> C#):
/// - __int64 -> long (instID)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PRIVATE_CHATROOM_BAN
{
	public long instID;
}
