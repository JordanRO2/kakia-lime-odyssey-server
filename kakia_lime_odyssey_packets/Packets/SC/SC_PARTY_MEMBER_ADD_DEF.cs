/// <summary>
/// Server->Client notification that party member gained a buff/debuff.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_ADD_DEF
/// Size: 20 bytes (22 with PACKET_FIX header)
/// Triggered by: Member gains status effect
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_ADD_DEF
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Buff/debuff effect definition</summary>
	public DEF def;
}
