/// <summary>
/// Client->Server packet to select a combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CHOICED_COMBAT_JOB
/// Size: 1 byte (3 with PACKET_FIX header)
/// Response: SC_SELECTED_COMBAT_JOB
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_CHOICED_COMBAT_JOB
{
	/// <summary>Index of the combat job being selected</summary>
	public byte index;
}
