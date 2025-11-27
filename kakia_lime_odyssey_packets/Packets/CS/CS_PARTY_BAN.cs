using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server kick a party member (leader only).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PARTY_BAN
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// Response: SC_PARTY_MEMBER_BANNED
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PARTY_BAN : IPacketFixed
{
	/// <summary>Party member index to kick (offset 0x02)</summary>
	public uint idx;
}
