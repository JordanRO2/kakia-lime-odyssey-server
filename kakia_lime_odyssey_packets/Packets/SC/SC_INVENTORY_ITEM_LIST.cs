using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_INVENTORY_ITEM_LIST @ 9 bytes (header only)
/// Sends inventory metadata. Actual inventory items are sent in separate packets.
/// This is a variable-length packet (PACKET_VAR header).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_INVENTORY_ITEM_LIST
{
	public int maxCount;
	public byte inventoryGrade;

	// Note: Inventory items are sent separately as individual INVENTORY_ITEM packets
	// The array below is for convenience in the C# implementation
	public INVENTORY_ITEM[] inventory;
}
