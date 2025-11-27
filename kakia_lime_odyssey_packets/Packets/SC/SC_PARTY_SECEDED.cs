/// <summary>
/// Server->Client notification that a member left the party.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_SECEDED
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Triggered by: CS_PARTY_SECEDE or member logout
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_SECEDED
{
	/// <summary>Party member index who left</summary>
	public uint idx;
}
