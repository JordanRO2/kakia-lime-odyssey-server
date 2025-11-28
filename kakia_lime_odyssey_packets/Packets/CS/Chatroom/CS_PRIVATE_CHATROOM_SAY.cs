using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server private chatroom message packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PRIVATE_CHATROOM_SAY
/// Size: Variable (4+ bytes with PACKET_VAR header)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: variable-length message text
/// Response: SC_PRIVATE_CHATROOM_SAY (broadcast to chatroom)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PRIVATE_CHATROOM_SAY : IPacketVar
{
	/// <summary>Chatroom message content (offset 0x04)</summary>
	public string message;
}
