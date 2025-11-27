using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server request to join another player's party.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PARTY_REQUEST_JOIN
/// Size: 43 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[41] name (41 bytes)
/// Response: None (target receives join request)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PARTY_REQUEST_JOIN : IPacketFixed
{
	/// <summary>Target party name (max 40 chars + null terminator) (offset 0x02)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
	public string name;
}
