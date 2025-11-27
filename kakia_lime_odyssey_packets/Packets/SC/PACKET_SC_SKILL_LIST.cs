/// <summary>
/// Server->Client: Complete list of character's skills.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_SKILL_LIST
/// Size: 4 bytes (variable-length header only)
/// Sent on login/zone entry with all learned skills.
/// Variable-length packet - followed by array of skill entries.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SKILL_LIST
{
	/// <summary>Variable-length packet header</summary>
	public PACKET_VAR header;

	// Followed by variable-length array of skill entries
}
