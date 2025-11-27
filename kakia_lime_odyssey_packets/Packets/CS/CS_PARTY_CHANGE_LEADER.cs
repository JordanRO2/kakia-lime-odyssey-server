/// <summary>
/// Client->Server transfer party leadership to another member (leader only).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_CHANGE_LEADER
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Response: SC_PARTY_CHANGED_LEADER
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_CHANGE_LEADER
{
	/// <summary>Party member index to promote to leader</summary>
	public uint idx;
}
