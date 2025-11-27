/// <summary>
/// Server->Client notification that a new member joined the party.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_ADDED
/// Size: 96 bytes (98 with PACKET_FIX header)
/// Triggered by: New member joining party
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_ADDED
{
	/// <summary>Complete party member information</summary>
	public PARTY_MEMBER member;
}
