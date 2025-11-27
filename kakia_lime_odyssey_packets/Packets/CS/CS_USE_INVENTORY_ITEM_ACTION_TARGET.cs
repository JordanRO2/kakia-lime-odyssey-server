using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_USE_INVENTORY_ITEM_ACTION_TARGET - Client uses an inventory item on the selected action target
/// IDA Verification: PACKET_CS_USE_INVENTORY_ITEM_ACTION_TARGET
/// Size: 6 bytes (2 byte header + 4 byte slot)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_USE_INVENTORY_ITEM_ACTION_TARGET
{
	public int slot;
}
