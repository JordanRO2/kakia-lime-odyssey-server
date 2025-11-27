/// <summary>
/// Client->Server packet to stop continuous crafting.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_CONTINUALLY_STOP
/// Size: 0 bytes (2 with PACKET_FIX header)
/// Response: SC_ITEM_MAKE_FINISH
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_ITEM_MAKE_CONTINUALLY_STOP
{
	// No fields - header only packet
}
