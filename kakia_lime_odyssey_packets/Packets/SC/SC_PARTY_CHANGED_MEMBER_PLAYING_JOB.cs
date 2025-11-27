/// <summary>
/// Server->Client notification that party member switched active job.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_CHANGED_MEMBER_PLAYING_JOB
/// Size: 5 bytes (7 with PACKET_FIX header)
/// Triggered by: Member job switch
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_CHANGED_MEMBER_PLAYING_JOB
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Job class (0=combat, 1=life)</summary>
	public byte jobClass;
}
