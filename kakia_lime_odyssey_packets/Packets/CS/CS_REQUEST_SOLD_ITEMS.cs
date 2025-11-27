/// <summary>
/// Client->Server packet to request list of items the player has sold to the NPC.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_REQUEST_SOLD_ITEMS
/// Size: 0 bytes (2 with PACKET_FIX header only)
/// Response: SC_SOLD_ITEM_LIST
/// Note: NPCs can buy back recently sold items from players
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_REQUEST_SOLD_ITEMS
{
	// Empty packet - header only
}
