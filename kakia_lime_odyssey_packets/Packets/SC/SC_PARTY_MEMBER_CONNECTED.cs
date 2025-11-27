/// <summary>
/// Server->Client notification that a party member logged in.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_CONNECTED
/// Size: 38 bytes (40 with PACKET_FIX header)
/// Triggered by: Party member login
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_CONNECTED
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Character instance ID</summary>
	public long instID;

	/// <summary>Character name (max 25 chars + null)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;

	/// <summary>Member state (stats, position, etc.)</summary>
	public PARTY_MEMBER_STATE state;
}
