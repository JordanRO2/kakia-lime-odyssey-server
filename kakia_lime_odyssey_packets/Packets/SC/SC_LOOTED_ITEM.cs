/// <summary>
/// Server->Client confirmation that an item was successfully looted.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_LOOTED_ITEM
/// Size: 14 bytes (12 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - slot: Inventory slot where item was placed (4 bytes at offset 0x02)
///   - count: Amount looted (8 bytes at offset 0x06)
/// Triggered by: CS_LOOT_ITEM
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LOOTED_ITEM : IPacketFixed
{
	/// <summary>Inventory slot where item was placed</summary>
	public int slot;

	/// <summary>Amount looted</summary>
	public long count;
}
