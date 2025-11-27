/// <summary>
/// Client packet to select a life job from the choice dialog.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CHOICED_LIFE_JOB
/// Size: 3 bytes
/// Ordinal: 2846
/// Player selects which life job they want from the available options.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_CHOICED_LIFE_JOB
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Index of the selected life job</summary>
	public byte index;
}
