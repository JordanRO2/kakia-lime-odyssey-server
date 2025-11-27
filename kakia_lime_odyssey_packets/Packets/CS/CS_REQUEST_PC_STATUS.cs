/// <summary>
/// Client->Server packet to request another player's status.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_REQUEST_PC_STATUS
/// Size: 8 bytes (10 with PACKET_FIX header)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_PC_STATUS
{
	/// <summary>Object instance ID of the player to query</summary>
	public long objInstID;
}
