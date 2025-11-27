/// <summary>
/// Server->Client notification that a member was kicked from party.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_BANNED
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Triggered by: CS_PARTY_BAN
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_BANNED
{
	/// <summary>Party member index who was kicked</summary>
	public uint idx;
}
