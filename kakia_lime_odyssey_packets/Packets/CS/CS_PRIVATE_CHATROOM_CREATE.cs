using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to create a private chatroom.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PRIVATE_CHATROOM_CREATE
/// Size: 54 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[41] name (41 bytes)
/// - 0x2B: char[9] password (9 bytes)
/// - 0x34: char maxPersons (1 byte)
/// - 0x35: char type (1 byte)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PRIVATE_CHATROOM_CREATE : IPacketFixed
{
	/// <summary>Chatroom name (offset 0x02)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
	public byte[] name;

	/// <summary>Chatroom password (offset 0x2B)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
	public byte[] password;

	/// <summary>Maximum participants (offset 0x34)</summary>
	public byte maxPersons;

	/// <summary>Chatroom type (offset 0x35)</summary>
	public byte type;
}
