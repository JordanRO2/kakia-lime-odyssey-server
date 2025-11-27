/// <summary>
/// Client->Server send party chat message.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_SAY_PC
/// Size: 8 bytes (12 with PACKET_VAR header)
/// Response: SC_PARTY_SAY
/// Variable-length packet with message text appended.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_SAY_PC
{
	/// <summary>Message maintain time in milliseconds</summary>
	public uint maintainTime;

	/// <summary>Chat type/flags</summary>
	public int type;

	// Note: Variable-length message text follows this header
}
