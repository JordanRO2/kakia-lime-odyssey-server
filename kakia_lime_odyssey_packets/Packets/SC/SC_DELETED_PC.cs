using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_DELETED_PC - Server to Client character deletion confirmation packet
/// Total size: 3 bytes (including 2-byte PACKET_FIX header)
/// IDA verified: 2025-11-26
/// Structure name in client: PACKET_SC_DELETED_PC
/// </summary>
/// <remarks>
/// This packet is sent by the server to confirm successful character deletion.
/// The charNum field indicates which character slot was deleted.
/// Client should remove the character from the character selection UI.
/// IDA Details:
/// - Total size: 3 bytes
/// - Member count: 2 (PACKET_FIX header + charNum)
/// - Field at offset 0x02: unsigned __int8 charNum
/// Triggered by: CS_DELETE_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DELETED_PC
{
	/// <summary>
	/// Character slot number that was deleted (0-based index)
	/// Offset: 0x02 (after PACKET_FIX header)
	/// Type: unsigned __int8 (byte)
	/// This matches the charNum sent in CS_DELETE_PC request
	/// </summary>
	public byte charNum;
}
