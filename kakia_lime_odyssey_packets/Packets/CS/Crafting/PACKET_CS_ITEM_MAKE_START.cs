/// <summary>
/// Client packet to start crafting an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_ITEM_MAKE_START
/// Size: 2 bytes
/// Ordinal: 2626
/// Initiates the crafting process (casting time begins).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_ITEM_MAKE_START
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
