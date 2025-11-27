/// <summary>
/// Server->Client update of party member's position.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_POS
/// Size: 20 bytes (22 with PACKET_FIX header)
/// Triggered by: Member moves to new zone or significant position change
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_UPDATE_MEMBER_POS
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Zone ID where member is located</summary>
	public uint zoneID;

	/// <summary>Position in world (float-based)</summary>
	public FPOS pos;
}
