/// <summary>
/// Client->Server request to create a new party with specified name.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_CREATE
/// Size: 41 bytes (43 with PACKET_FIX header)
/// Response: SC_PARTY_CREATED
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_CREATE
{
	/// <summary>Party name (max 40 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
	public string name;
}
