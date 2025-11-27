/// <summary>
/// Client->Server packet to select a life job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CHOICED_LIFE_JOB
/// Size: 1 byte (3 with PACKET_FIX header)
/// Response: SC_SELECTED_LIFE_JOB
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_CHOICED_LIFE_JOB
{
	/// <summary>Index of the life job being selected</summary>
	public byte index;
}
