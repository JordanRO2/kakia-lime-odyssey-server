/// <summary>
/// Server->Client notification that party member lost a buff/debuff.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_DEL_DEF
/// Size: 8 bytes (10 with PACKET_FIX header)
/// Triggered by: Member loses status effect
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_DEL_DEF
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Effect instance ID to remove</summary>
	public uint defInstID;
}
