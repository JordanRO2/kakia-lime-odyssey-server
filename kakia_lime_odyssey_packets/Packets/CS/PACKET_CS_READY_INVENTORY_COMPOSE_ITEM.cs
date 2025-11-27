/// <summary>
/// Client packet requesting to prepare for item composition.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_READY_INVENTORY_COMPOSE_ITEM
/// Size: 6 bytes
/// Ordinal: 2670
/// Sent when opening the item composition interface for a specific item.
/// Server responds with SC_READY_INVENTORY_COMPOSE_ITEM.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_READY_INVENTORY_COMPOSE_ITEM
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Inventory slot containing the item to compose</summary>
	public int slot;
}
