/// <summary>
/// Server->Client update of party member's state (HP, MP, position, etc).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_STATE
/// Size: 56 bytes (58 with PACKET_FIX header)
/// Triggered by: Member state changes
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_STATE
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Updated state information</summary>
	public PARTY_MEMBER_STATE state;
}
