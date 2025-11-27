using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client about the number of available item slots.
/// Used to update inventory/bank slot capacity.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_UPDATE_ITEM_SLOT_COUNT
/// Size: 7 bytes (IDA) = 2 bytes header + 5 bytes data
/// C# Size: 5 bytes (header handled by IPacketFixed)
/// Packet Type: PACKET_FIX (fixed length header, 2 bytes)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes [handled by IPacketFixed]
/// - 0x02: unsigned __int8 type - 1 byte (inventory type: 0=inventory, 1=bank, etc.)
/// - 0x03: int count - 4 bytes (number of available slots)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPDATE_ITEM_SLOT_COUNT : IPacketFixed
{
	public byte type;
	public int count;
}
