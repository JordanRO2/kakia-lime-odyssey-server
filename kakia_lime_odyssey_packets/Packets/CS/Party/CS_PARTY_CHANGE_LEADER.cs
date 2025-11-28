using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server transfer party leadership to another member (leader only).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PARTY_CHANGE_LEADER
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// Response: SC_PARTY_CHANGED_LEADER
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PARTY_CHANGE_LEADER : IPacketFixed
{
	/// <summary>Party member index to promote to leader (offset 0x02)</summary>
	public uint idx;
}
