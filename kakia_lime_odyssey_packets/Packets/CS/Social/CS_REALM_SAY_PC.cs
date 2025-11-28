using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server realm-wide chat message packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_REALM_SAY_PC
/// Size: Variable (12+ bytes with PACKET_VAR header)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned int maintainTime (4 bytes)
/// - 0x08: int type (4 bytes)
/// - 0x0C: variable-length message text
/// Response: SC_REALM_SAY (broadcast to realm)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_REALM_SAY_PC : IPacketVar
{
	/// <summary>Message maintain time (offset 0x04)</summary>
	public uint maintainTime;

	/// <summary>Chat message type (offset 0x08)</summary>
	public int type;

	// Variable-length message text follows this header
}
