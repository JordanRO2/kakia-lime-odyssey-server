/// <summary>
/// Server->Client update of party member's HP.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_HP
/// Size: 12 bytes (14 with PACKET_FIX header)
/// Triggered by: Member HP change
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_UPDATE_MEMBER_HP
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Current HP</summary>
	public int hp;

	/// <summary>Maximum HP</summary>
	public int mhp;
}
