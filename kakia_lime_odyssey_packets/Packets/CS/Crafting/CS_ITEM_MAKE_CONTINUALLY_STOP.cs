using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to stop continuous crafting.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_ITEM_MAKE_CONTINUALLY_STOP
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// Response: SC_ITEM_MAKE_FINISH
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_ITEM_MAKE_CONTINUALLY_STOP : IPacketFixed
{
	// Header only - no payload
}
