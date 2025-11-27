/// <summary>
/// Server->Client party chat message.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_SAY
/// Size: 12 bytes (16 with PACKET_VAR header)
/// Triggered by: CS_PARTY_SAY_PC
/// Variable-length packet with message text appended.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_SAY
{
	/// <summary>Party member index who sent message</summary>
	public uint idx;

	/// <summary>Message maintain time in milliseconds</summary>
	public uint maintainTime;

	/// <summary>Chat type/flags</summary>
	public int type;

	// Note: Variable-length message text follows this header
}
