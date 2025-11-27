/// <summary>
/// Server->Client notification that player joined a party.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_JOINED
/// Size: 50 bytes (54 with PACKET_VAR header)
/// Triggered by: CS_PARTY_JOIN
/// Variable-length packet containing party member list.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_JOINED
{
	/// <summary>Party name (max 40 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
	public string name;

	/// <summary>This player's party member index</summary>
	public uint myIdx;

	/// <summary>Party leader's member index</summary>
	public uint leaderIndex;

	/// <summary>Party options/flags</summary>
	public byte option;

	// Note: Variable-length PARTY_MEMBER array follows this header
}
