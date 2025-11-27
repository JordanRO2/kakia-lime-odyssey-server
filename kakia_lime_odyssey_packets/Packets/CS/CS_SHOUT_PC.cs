using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server zone-wide shout message packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_SHOUT_PC
/// Size: Variable (4+ bytes with PACKET_VAR header)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: variable-length message text
/// Response: SC_SHOUT (broadcast to zone)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SHOUT_PC : IPacketVar
{
	/// <summary>Shout message content (offset 0x04)</summary>
	public string message;
}
