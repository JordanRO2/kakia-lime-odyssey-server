/// <summary>
/// Client packet sent when player selects a combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CHOICED_COMBAT_JOB
/// Size: 3 bytes
/// Ordinal: 2862
/// Player chooses a combat job from available options.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_CHOICED_COMBAT_JOB
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Index of the selected combat job</summary>
	public byte index;
}
