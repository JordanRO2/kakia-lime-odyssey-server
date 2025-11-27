/// <summary>
/// Server->Client packet containing list of lootable items.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_LOOTABLE_ITEM_LIST
/// Size: 4 bytes + variable (PACKET_VAR)
/// Fields:
///   - count: Number of items in list (ushort, 2 bytes)
///   - lootTable: Array of INVENTORY_ITEM structures
/// Triggered by: CS_SELECT_TARGET_REQUEST_START_LOOTING
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

public struct SC_LOOTABLE_ITEM_LIST : IPacketVar
{
	/// <summary>Number of items in loot table</summary>
	public ushort count;

	/// <summary>Array of lootable items</summary>
	public INVENTORY_ITEM[] lootTable;
}
