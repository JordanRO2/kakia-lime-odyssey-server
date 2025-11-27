/// <summary>
/// Server->Client update of party member's name.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_NAME
/// Size: 30 bytes (32 with PACKET_FIX header)
/// Triggered by: Member name change
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_NAME
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>New character name (max 25 chars + null)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;
}
