using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_PRIVATE_CHATROOM_CREATE - Create private chatroom packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_PRIVATE_CHATROOM_CREATE
/// IDA Structure Size: 54 bytes
///
/// IDA Structure Layout:
/// +0x00: PACKET_FIX (header: ushort) - 2 bytes [handled by framework]
/// +0x02: name (char[41]) - 41 bytes
/// +0x2B: password (char[9]) - 9 bytes
/// +0x34: maxPersons (char) - 1 byte
/// +0x35: type (char) - 1 byte
///
/// C# Implementation Notes:
/// - PACKET_FIX header (2 bytes) is stripped by RawPacket.ParsePackets
/// - name is a fixed-length ASCII string (41 bytes)
/// - password is a fixed-length ASCII string (9 bytes)
/// - maxPersons is the maximum number of participants allowed
/// - type is the chatroom type/category
///
/// Type Mappings (IDA -> C#):
/// - char[41] -> byte[41] (name)
/// - char[9] -> byte[9] (password)
/// - char -> byte (maxPersons, type)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PRIVATE_CHATROOM_CREATE
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
	public byte[] name;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
	public byte[] password;

	public byte maxPersons;
	public byte type;
}
