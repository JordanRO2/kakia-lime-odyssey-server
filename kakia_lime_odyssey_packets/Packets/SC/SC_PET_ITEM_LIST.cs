using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_PET_ITEM_LIST @ 8 bytes (header only)
/// Sent when pet inventory is accessed.
/// Contains pet inventory capacity. Actual items follow in separate INVENTORY_ITEM packets.
/// This is a variable-length packet (PACKET_VAR header).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PET_ITEM_LIST
{
	public int maxCount;

	// Note: Pet inventory items are sent separately as individual INVENTORY_ITEM packets
	// The array below is for convenience in the C# implementation
	public INVENTORY_ITEM[] petInventory;
}
