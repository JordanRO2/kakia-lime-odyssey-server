/// <summary>
/// Client->Server packet to prepare for material gathering/processing.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_READY
/// Size: 4 bytes (6 with PACKET_FIX header)
/// Response: SC_STUFF_MAKE_READY_SUCCESS
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_STUFF_MAKE_READY
{
	/// <summary>Type ID of the gathering/processing action</summary>
	public uint typeID;
}
