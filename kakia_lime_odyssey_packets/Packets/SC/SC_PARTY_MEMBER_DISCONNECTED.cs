/// <summary>
/// Server->Client notification that a party member logged out.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_DISCONNECTED
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Triggered by: Party member logout
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_DISCONNECTED
{
	/// <summary>Party member index</summary>
	public uint idx;
}
