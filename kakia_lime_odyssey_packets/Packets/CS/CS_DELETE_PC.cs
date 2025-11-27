using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_DELETE_PC - Client to Server character deletion packet
/// Total size: 3 bytes (including 2-byte PACKET_FIX header)
/// IDA verified: 2025-11-26
/// Structure name in client: PACKET_CS_DELETE_PC
/// </summary>
/// <remarks>
/// This packet is sent when a player deletes a character from their character list.
/// The charNum field represents the slot number (0-based index) of the character to delete.
/// Server responds with SC_DELETED_PC on success.
/// IDA Details:
/// - Total size: 3 bytes
/// - Member count: 2 (PACKET_FIX header + charNum)
/// - Field at offset 0x02: unsigned __int8 charNum
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DELETE_PC
{
	/// <summary>
	/// Character slot number to delete (0-based index)
	/// Offset: 0x02 (after PACKET_FIX header)
	/// Type: unsigned __int8 (byte)
	/// Valid range: 0-7 (typically max 8 character slots)
	/// </summary>
	public byte charNum;
}
