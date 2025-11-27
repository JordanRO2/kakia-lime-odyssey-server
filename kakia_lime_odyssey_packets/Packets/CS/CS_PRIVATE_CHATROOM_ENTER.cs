using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_PRIVATE_CHATROOM_ENTER - Enter private chatroom packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_PRIVATE_CHATROOM_ENTER
/// IDA Structure Size: 19 bytes
///
/// IDA Structure Layout:
/// +0x00: PACKET_FIX (header: ushort) - 2 bytes [handled by framework]
/// +0x02: instID (__int64) - 8 bytes
/// +0x0A: password (char[9]) - 9 bytes
///
/// C# Implementation Notes:
/// - PACKET_FIX header (2 bytes) is stripped by RawPacket.ParsePackets
/// - instID is the chatroom instance ID to enter
/// - password is a fixed-length ASCII string (9 bytes)
///
/// Type Mappings (IDA -> C#):
/// - __int64 -> long (instID)
/// - char[9] -> byte[9] (password)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PRIVATE_CHATROOM_ENTER
{
	public long instID;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
	public byte[] password;
}
