using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_PRIVATE_CHATROOM_DESTROYED - Private chatroom destroyed notification
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_PRIVATE_CHATROOM_DESTROYED
/// IDA Structure Size: 2 bytes
///
/// IDA Structure Layout:
/// +0x00: PACKET_FIX (header: ushort) - 2 bytes [handled by framework]
///
/// C# Implementation Notes:
/// - PACKET_FIX header (2 bytes) is stripped by RawPacket.ParsePackets
/// - This is an empty packet with only the header
/// - Sent to all chatroom members when the owner destroys the chatroom
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PRIVATE_CHATROOM_DESTROYED
{
	// Empty packet - header only
}
