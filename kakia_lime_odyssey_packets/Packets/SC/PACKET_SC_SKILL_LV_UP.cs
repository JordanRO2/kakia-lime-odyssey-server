/// <summary>
/// Server->Client: Skill level increased.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_SKILL_LV_UP
/// Size: 4 bytes (variable-length header only)
/// Sent when a skill's level increases via skill point distribution.
/// Variable-length packet - followed by skill data.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SKILL_LV_UP
{
	/// <summary>Variable-length packet header</summary>
	public PACKET_VAR header;

	// Followed by variable-length skill data
}
