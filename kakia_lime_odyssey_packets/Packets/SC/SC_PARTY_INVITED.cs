/// <summary>
/// Server->Client party invitation received.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_INVITED
/// Size: 67 bytes (69 with PACKET_FIX header)
/// Triggered by: CS_PARTY_INVITE
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_INVITED
{
	/// <summary>Inviter's character name (max 25 chars + null)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string pcName;

	/// <summary>Party name (max 40 chars + null)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
	public string partyName;
}
