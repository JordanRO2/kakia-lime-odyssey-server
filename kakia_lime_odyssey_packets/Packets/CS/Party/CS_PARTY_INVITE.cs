using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server invite a player to party by character name.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_PARTY_INVITE
/// Size: 28 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[26] name (26 bytes)
/// Response: SC_PARTY_INVITED
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_PARTY_INVITE : IPacketFixed
{
	/// <summary>Target character name (max 25 chars + null terminator) (offset 0x02)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;
}
