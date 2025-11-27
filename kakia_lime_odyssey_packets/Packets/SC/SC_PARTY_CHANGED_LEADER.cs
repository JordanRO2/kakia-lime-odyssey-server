/// <summary>
/// Server->Client notification that party leadership changed.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_CHANGED_LEADER
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Triggered by: CS_PARTY_CHANGE_LEADER
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_CHANGED_LEADER
{
	/// <summary>New leader's party member index</summary>
	public uint idx;
}
