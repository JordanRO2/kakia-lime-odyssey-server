using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client to update an item in a specific slot.
/// Used when item count or properties change in inventory/bank.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_UPDATE_SLOT_ITEM
/// Size: 15 bytes (2 header + 5 ITEM_SLOT + 8 count)
/// Packet Type: PACKET_FIX (fixed length header, 2 bytes)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes
/// - 0x02: ITEM_SLOT slot - 5 bytes (slot type and index)
/// - 0x07: __int64 count - 8 bytes (item count/stack size)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPDATE_SLOT_ITEM : IPacketFixed
{
	public ITEM_SLOT slot;
	public long count;
}
