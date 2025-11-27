/// <summary>
/// Server->Client update of party member's LP (Life Points).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_LP
/// Size: 12 bytes (14 with PACKET_FIX header)
/// Triggered by: Member LP change
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_UPDATE_MEMBER_LP
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Current LP</summary>
	public int lp;

	/// <summary>Maximum LP</summary>
	public int mlp;
}
