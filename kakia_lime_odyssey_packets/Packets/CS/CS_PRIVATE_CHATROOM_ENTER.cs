using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to enter a private chatroom.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PRIVATE_CHATROOM_ENTER
/// Size: 19 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 instID (8 bytes)
/// - 0x0A: char[9] password (9 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PRIVATE_CHATROOM_ENTER : IPacketFixed
{
	/// <summary>Chatroom instance ID to enter (offset 0x02)</summary>
	public long instID;

	/// <summary>Chatroom password (offset 0x0A)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
	public byte[] password;
}
