using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_PRIVATE_CHATROOM_DESTROY - Destroy private chatroom packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_PRIVATE_CHATROOM_DESTROY
/// IDA Structure Size: 2 bytes
///
/// IDA Structure Layout:
/// +0x00: PACKET_FIX (header: ushort) - 2 bytes [handled by framework]
///
/// C# Implementation Notes:
/// - PACKET_FIX header (2 bytes) is stripped by RawPacket.ParsePackets
/// - This is an empty packet with only the header
/// - Used by the chatroom owner to destroy their chatroom
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PRIVATE_CHATROOM_DESTROY
{
	// Empty packet - header only
}
