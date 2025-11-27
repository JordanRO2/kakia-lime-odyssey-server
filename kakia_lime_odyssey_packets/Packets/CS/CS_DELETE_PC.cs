using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server character deletion packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_DELETE_PC
/// Size: 3 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned char charNum (1 byte)
/// Response: SC_DELETED_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DELETE_PC : IPacketFixed
{
	/// <summary>
	/// Character slot number to delete (0-based index)
	/// Offset: 0x02 (after PACKET_FIX header)
	/// Type: unsigned __int8 (byte)
	/// Valid range: 0-7 (typically max 8 character slots)
	/// </summary>
	public byte charNum;
}
