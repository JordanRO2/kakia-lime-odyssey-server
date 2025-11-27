using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to select a combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_CHOICED_COMBAT_JOB
/// Size: 3 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned char index (1 byte)
/// Response: SC_SELECTED_COMBAT_JOB
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_CHOICED_COMBAT_JOB : IPacketFixed
{
	/// <summary>Index of the combat job being selected (offset 0x02)</summary>
	public byte index;
}
