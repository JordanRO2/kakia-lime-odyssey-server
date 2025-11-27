/// <summary>
/// Server->Client update of party member's level and max stats.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_LV
/// Size: 24 bytes (26 with PACKET_FIX header)
/// Triggered by: Member level up
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_UPDATE_MEMBER_LV
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>New level</summary>
	public int lv;

	/// <summary>New maximum HP</summary>
	public int mhp;

	/// <summary>New maximum MP</summary>
	public int mmp;

	/// <summary>New maximum LP</summary>
	public int mlp;

	/// <summary>New maximum SP (Stamina Points)</summary>
	public int msp;
}
