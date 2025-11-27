/// <summary>
/// Server->Client update of party member's combat job level.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_COMBAT_JOB_LV
/// Size: 8 bytes (10 with PACKET_FIX header)
/// Triggered by: Member combat job level up
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_UPDATE_MEMBER_COMBAT_JOB_LV
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>New combat job level</summary>
	public int lv;
}
