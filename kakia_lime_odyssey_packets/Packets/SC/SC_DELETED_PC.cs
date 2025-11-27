using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client character deletion confirmation packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DELETED_PC
/// Size: 3 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int8 charNum (1 byte)
/// Triggered by: CS_DELETE_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DELETED_PC : IPacketFixed
{
	/// <summary>
	/// Character slot number that was deleted (0-based index)
	/// Offset: 0x02 (after PACKET_FIX header)
	/// Type: unsigned __int8 (byte)
	/// This matches the charNum sent in CS_DELETE_PC request
	/// </summary>
	public byte charNum;
}
