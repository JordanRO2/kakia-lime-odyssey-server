using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Complete party member information.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// Size: 96 bytes
/// Full party member data including identity and state.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PARTY_MEMBER
{
	/// <summary>Party member index (slot)</summary>
	public uint idx;

	/// <summary>Character name (max 25 chars + null)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;

	/// <summary>Is member currently connected</summary>
	public bool isConnected;

	/// <summary>Character instance ID</summary>
	public long instID;

	/// <summary>Member's current state</summary>
	public PARTY_MEMBER_STATE state;
}
