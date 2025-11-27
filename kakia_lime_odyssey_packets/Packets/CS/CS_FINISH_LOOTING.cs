/// <summary>
/// Client->Server notification that the player has finished looting.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_FINISH_LOOTING
/// Size: 2 bytes (PACKET_FIX header only, no additional fields)
/// Response: SC_FINISH_LOOTING
/// Note: Sent when player closes loot window.
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_FINISH_LOOTING : IPacketFixed
{
	// This packet has no fields beyond the PACKET_FIX header
}
