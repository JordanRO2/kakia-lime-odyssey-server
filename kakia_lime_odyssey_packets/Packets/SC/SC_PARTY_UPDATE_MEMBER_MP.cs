/// <summary>
/// Server->Client update of party member's MP.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_MP
/// Size: 12 bytes (14 with PACKET_FIX header)
/// Triggered by: Member MP change
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_UPDATE_MEMBER_MP
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Current MP</summary>
	public int mp;

	/// <summary>Maximum MP</summary>
	public int mmp;
}
